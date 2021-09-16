﻿namespace SharpDeck.Interactivity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SharpDeck.Connectivity;
    using SharpDeck.Events.Received;
    using SharpDeck.Extensions;
    using SharpDeck.Resources;

    /// <summary>
    /// Provides functionality for showing the items of type <typeparamref name="T"/> as part of a drill down.
    /// </summary>
    /// <typeparam name="T">The type of the items within the drill down.</typeparam>
    public class DrillDown<T> : IDrillDown<T>
    {
        /// <summary>
        /// The offset that represents the presence of the close-button.
        /// </summary>
        private const int CLOSE_BUTTON_OFFSET = 1;

        /// <summary>
        /// The synchronization root.
        /// </summary>
        private readonly SemaphoreSlim _syncRoot = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="DrillDown{T}" /> class.
        /// </summary>
        /// <param name="context">The context that provides information about where and how the drill down will be shown.</param>
        /// <param name="controller">The controller that provides functionality for handling a selection, or rendering items.</param>
        /// <param name="logger">The optional logger.</param>
        internal DrillDown(DrillDownContext<T> context, IDrillDownController<T> controller, ILogger<DrillDown<T>> logger = null)
        {
            this.Context = context;
            this.Controller = controller;
            this.Logger = logger;

            this.Buttons = new MonitoredButtonCollection(this.Context.Connection, this.Context.Device);
            this.Context.Connection.KeyUp += Connection_KeyUp;
            this.Context.DrillDown = this;
        }

        /// <summary>
        /// Gets the available buttons for the device.
        /// </summary>
        private MonitoredButtonCollection Buttons { get; }

        /// <summary>
        /// Get or sets the context that provides information about where and how the drill down will be shown.
        /// </summary>
        private DrillDownContext<T> Context { get; set; }

        /// <summary>
        /// Gets the action responsible for handling the display and selection of an item
        /// </summary>
        private IDrillDownController<T> Controller { get; }

        /// <summary>
        /// Gets the data source containing the items to show.
        /// </summary>
        private IEnumerable<T> DataSource { get; set; } = Enumerable.Empty<T>();

        /// <summary>
        /// Gets or sets a value indicating whether this instance is disposed.
        /// </summary>
        private bool IsDisposed { get; set; } = false;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Gets or sets the cancellation token source that is cancelled upon a page changing.
        /// </summary>
        private CancellationTokenSource PageChangingCancellationTokenSource { get; set; }

        /// <summary>
        /// Gets or sets the pager.
        /// </summary>
        private DevicePager<T> Pager { get; set; }

        /// <summary>
        /// Gets the task completion source that represents the result of this drill down.
        /// </summary>
        private TaskCompletionSource<DrillDownResult<T>> Result { get; } = new TaskCompletionSource<DrillDownResult<T>>();

        /// <summary>
        /// Closes the drill down, and switches back to the previous profile.
        /// </summary>
        public void Close()
        {
            using (this._syncRoot.Lock())
            {
                this.Result.TrySetResult(DrillDownResult<T>.None);
                this.Dispose(false);
            }
        }

        /// <summary>
        /// Closes the drill down, and switches back to the previous profile; the result of the drill down is set to the specified <paramref name="result" />.
        /// </summary>
        /// <param name="result">The result of the drill down.</param>
        public void CloseWithResult(T result)
        {
            using (this._syncRoot.Lock())
            {
                this.Result.TrySetResult(new DrillDownResult<T>(true, result));
                this.Dispose(false);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            using (this._syncRoot.Lock())
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Shows the drill down with the given items asynchronously.
        /// </summary>
        /// <param name="items">The items to show.</param>
        /// <returns>The result of the drill down.</returns>
        public async Task<DrillDownResult<T>> ShowAsync(IEnumerable<T> items)
        {
            using (await this._syncRoot.LockAsync())
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                this.PageChangingCancellationTokenSource?.Cancel();

                // Switch to the profile, and await a full layout.
                await this.Context.Connection.SwitchToProfileAsync(this.Context.PluginUUID, this.Context.Device.Id, this.Context.Profile);
                await this.Buttons.WaitFullLayoutAsync();
                await this.Buttons[0].SetDisplayAsync(image: Images.Close);

                // Listen to buttons disappearing, this allows us to dispose of the drill-down if the profile changes for a reason out of our control.
                this.Context.Connection.WillDisappear -= this.Connection_WillDisappear;
                this.Context.Connection.WillDisappear += this.Connection_WillDisappear;

                this.DataSource = items;
                this.Pager = new DevicePager<T>(this.Buttons, this.DataSource);
                await this.ShowCurrentPageAsync();
            }

            return await this.Result.Task;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.Buttons.Dispose();
            this.Context.Connection.KeyUp -= this.Connection_KeyUp;
            this.Context.Connection.WillDisappear -= this.Connection_WillDisappear;

            this.PageChangingCancellationTokenSource?.Cancel();
            this.PageChangingCancellationTokenSource?.Dispose();

            this.Context.Connection.SwitchToProfileAsync(this.Context.PluginUUID, this.Context.Device.Id)
                .Forget(this.Logger);

            this.Context = null;
            this.DataSource = null;
            this.Pager = null;
            this.Result.TrySetResult(DrillDownResult<T>.None);

            this.IsDisposed = true;
        }

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.KeyUp"/> of the <see cref="IStreamDeckConnection"/> of the <see cref="Context"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActionEventArgs{KeyPayload}"/> instance containing the event data.</param>
        private void Connection_KeyUp(object sender, ActionEventArgs<KeyPayload> e)
        {
            using (this._syncRoot.Lock())
            {
                if (this.IsDisposed)
                {
                    return;
                }

                switch (e.Context)
                {
                    // Close button.
                    case string ctx when ctx == this.Buttons[0]?.Context:
                        this.Dispose(false);
                        break;

                    // Next button.
                    case string ctx when ctx == this.Pager.NextButton?.Context:
                        this.Pager.MoveNext();
                        this.ShowCurrentPageAsync().Forget(this.Logger);
                        break;

                    // Previous button.
                    case string ctx when ctx == this.Pager.PreviousButton?.Context:
                        this.Pager.MovePrevious();
                        this.ShowCurrentPageAsync().Forget(this.Logger);
                        break;

                    // Default; this may not be an item depending on how many items the current page has.
                    default:
                        if (this.Buttons.TryGetIndex(e.Device, e.Payload.Coordinates, out var index))
                        {
                            index -= CLOSE_BUTTON_OFFSET;
                            if (index < this.Pager.Items.Length)
                            {
                                var item = this.Pager.Items[index];
                                _ = Task.Run(() => this.Controller.OnSelectedAsync(this.Context, item).Forget(this.Logger));
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="IStreamDeckConnection.WillDisappear"/> event for an action; this is a safety precaution in the event an action switch profiles away from the drill down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ActionEventArgs{AppearancePayload}"/> instance containing the event data.</param>
        private void Connection_WillDisappear(object sender, ActionEventArgs<AppearancePayload> e)
            => this.Dispose();

        /// <summary>
        /// Shows the current page's items contained within <see cref="DevicePager{T}.Items"/>.
        /// </summary>
        /// <returns>The task of showing the current page.</returns>
        private Task ShowCurrentPageAsync()
        {
            if (this.IsDisposed)
            {
                return Task.CompletedTask;
            }

            // The cancellation token allows us to cancel any initialization that is still occuring from a previous page load.
            this.PageChangingCancellationTokenSource?.Cancel();
            this.PageChangingCancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = this.PageChangingCancellationTokenSource.Token;
            var tasks = new List<Task>();

            // Iterate over all available buttons.
            for (var buttonIndex = CLOSE_BUTTON_OFFSET; buttonIndex < this.Buttons.Length - this.Pager.NavigationButtonCount; buttonIndex++)
            {
                var itemIndex = buttonIndex - CLOSE_BUTTON_OFFSET;
                if (itemIndex < this.Pager.Items.Length)
                {
                    // The button has an item, so initiate it.
                    tasks.Add(this.Controller.OnShowAsync(this.Context, new StreamDeckButton(this.Context.Connection, this.Buttons[buttonIndex].Context), this.Pager.Items[itemIndex], cancellationToken));
                }
                else
                {
                    // The button does not have an item, so reset it to an empty button.
                    tasks.Add(this.Buttons[buttonIndex].SetDisplayAsync(image: Images.None, cancellationToken: cancellationToken));
                }
            }

            // Finally, set the navigation buttons; these may be null.
            tasks.Add(this.Pager.NextButton?.SetDisplayAsync(image: Images.Right, cancellationToken: cancellationToken));
            tasks.Add(this.Pager.PreviousButton?.SetDisplayAsync(image: Images.Left, cancellationToken: cancellationToken));

            return Task.WhenAll(tasks);
        }
    }
}