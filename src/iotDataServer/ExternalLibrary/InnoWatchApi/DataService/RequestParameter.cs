// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestParameter.cs" company="Innotive Inc. Korea">
//   Copyright (c) Innotive Corporation.  All rights reserved.
// </copyright>
// <summary>
//   The create type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace InnoWatchApi.DataService
{
    /// <summary>
    /// The request parameter.
    /// </summary>
    public class RequestParameter
    {
        /// <summary>
        /// Gets or sets Url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets EncodingOption.
        /// </summary>
        public string EncodingOption { get; set; }

        /// <summary>
        /// Gets or sets PostMessage.
        /// </summary>
        public string PostMessage { get; set; }

        /// <summary>
        /// Gets or sets PostByteData.
        /// </summary>
        public byte[] PostByteData { get; set; }

        /// <summary>
        /// The character set encoding option.
        /// </summary>
        public enum CharacterSetEncodingOption
        {
            /// <summary>
            /// The ascII.
            /// </summary>
            ASCII = 0,

            /// <summary>
            /// The big endian unicode.
            /// </summary>
            BigEndianUnicode = 1,

            /// <summary>
            /// The unicode.
            /// </summary>
            Unicode = 2,

            /// <summary>
            /// The utf32.
            /// </summary>
            UTF32 = 3,

            /// <summary>
            /// The utf7.
            /// </summary>
            UTF7 = 4,

            /// <summary>
            /// The utf8.
            /// </summary>
            UTF8 = 5,

            /// <summary>
            /// The default.
            /// </summary>
            Default = 6,
        }
    }
}
