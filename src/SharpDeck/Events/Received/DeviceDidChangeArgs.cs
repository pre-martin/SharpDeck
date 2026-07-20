namespace SharpDeck.Events.Received
{
    /// <summary>
    /// Arguments of the DeviceDidChange event.
    /// </summary>
    public class DeviceDidChangeArgs : StreamDeckEventArgs
    {
        /// <summary>
        /// Unique identifier of the Stream Deck device that this event is associated with.
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Information about the device that changed.
        /// </summary>
        public DeviceInfo DeviceInfo { get; set; }

    }
}
