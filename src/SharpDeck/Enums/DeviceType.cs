namespace SharpDeck.Enums
{
    /// <summary>
    /// Provides an enumeration of devices.
    /// See <see href="https://developer.elgato.com/documentation/stream-deck/sdk/manifest/#device-type">Elgato Developer Documentation</see> for more information.
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// Stream Deck, comprised of 15 customizable LCD keys in a 5 x 3 layout.
        /// </summary>
        StreamDeck = 0,

        /// <summary>
        /// Stream Deck Mini, comprised of 6 customizable LCD keys in a 3 x 2 layout.
        /// </summary>
        StreamDeckMini = 1,

        /// <summary>
        /// Stream Deck XL, comprised of 32 customizable LCD keys in an 8 x 4 layout.
        /// </summary>
        StreamDeckXL = 2,

        /// <summary>
        /// Stream Deck Mobile, for iOS and Android.
        /// </summary>
        StreamDeckMobile = 3,

        /// <summary>
        /// Corsair G Keys, available on select Corsair keyboards.
        /// </summary>
        CorsairGKeys = 4,

        /// <summary>
        /// Stream Deck Pedal, comprised of 3 customizable pedals.
        /// </summary>
        StreamDeckPedal = 5,

        /// <summary>
        /// Corsair Voyager laptop, comprising 10 buttons in a horizontal line above the keyboard.
        /// </summary>
        CorsairVoyager = 6,

        /// <summary>
        /// Stream Deck +, comprised of 8 customizable LCD keys in a 4 x 2 layout, a touch strip, and 4 dials.
        /// </summary>
        StreamDeckPlus = 7,

        /// <summary>
        /// SCUF controller G keys, available on select SCUF controllers, for example SCUF Envision.
        /// </summary>
        SCUFController = 8,

        /// <summary>
        /// Stream Deck Neo, comprised of 8 customizable LCD keys in a 4 x 2 layout, an info bar, and 2 touch points for page navigation.
        /// </summary>
        StreamDeckNeo = 9,

        /// <summary>
        /// Stream Deck Studio, comprised of 32 customizable LCD keys in a 16 x 2 layout, and 2 dials (1 on either side).
        /// </summary>
        StreamDeckStudio = 10,

        /// <summary>
        /// Virtual Stream Deck, comprised of 1 to 64 action (on-screen) on a scalable canvas, with a maximum layout of 8 x 8.
        /// </summary>
        VirtualStreamDeck = 11,

        /// <summary>
        /// High-performance gaming keyboard, with a built-in Stream Deck comprised of 12 customizable LCD keys in a 3 x 4 layout, an LCD screen, and 2 dials.
        /// </summary>
        Galleon100SD = 12,

        /// <summary>
        /// Stream Deck + XL, comprised of 36 customizable LCD keys in a 9 x 4 layout, a touch strip, and 6 dials.
        /// </summary>
        StreamDeckPlusXL = 13,
    }
}
