namespace SharpDeck.Enums
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Provides an enumeration of devices.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceType
    {
        /// <summary>
        /// Defines a Stream Deck: kESDSDKDeviceType_StreamDeck.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_StreamDeck")]
        StreamDeck = 0,

        /// <summary>
        /// Defines a Stream Deck Mini: kESDSDKDeviceType_StreamDeckMini.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_StreamDeckMini")]
        StreamDeckMini = 1,

        /// <summary>
        /// Defines a Stream Deck XL: kESDSDKDeviceType_StreamDeckXL.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_StreamDeckXL")]
        StreamDeckXL = 2,

        /// <summary>
        /// Defines a Stream Deck Mobile: kESDSDKDeviceType_StreamDeckMobile.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_StreamDeckMobile")]
        StreamDeckMobile = 3,

        /// <summary>
        /// Defines a Corsair G-Key compatible keyboard: kESDSDKDeviceType_CorsairGKeys.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_CorsairGKeys")]
        CorsairGKeys = 4,

        /// <summary>
        /// Defines a Stream Deck Pedal: kESDSDKDeviceType_StreamDeckPedal.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_StreamDeckPedal")]
        StreamDeckPedal = 5,

        /// <summary>
        /// Defines a Corsair Voyager: kESDSDKDeviceType_CorsairVoyager.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_CorsairVoyager")]
        CorsairVoyager = 6,

        /// <summary>
        /// Defines a Stream Deck Plus: kESDSDKDeviceType_StreamDeckPlus.
        /// </summary>
        [EnumMember(Value = "kESDSDKDeviceType_StreamDeckPlus")]
        StreamDeckPlus = 7,
    }
}
