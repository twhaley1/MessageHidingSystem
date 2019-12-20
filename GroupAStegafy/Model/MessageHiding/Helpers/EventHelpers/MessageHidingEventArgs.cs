using System;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupAStegafy.Model.MessageHiding.Helpers.EventHelpers
{
    /// <summary>
    ///     Event Arguments that are passed when text encoding completes.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TextEncodingEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the embed image.
        /// </summary>
        /// <value>
        ///     The embed image.
        /// </value>
        public WriteableBitmap EmbedImage { get; set; }

        /// <summary>
        ///     Gets or sets the non encrypted message.
        /// </summary>
        /// <value>
        ///     The non encrypted message.
        /// </value>
        public string NonEncryptedMessage { get; set; }

        /// <summary>
        ///     Gets or sets the encrypted message.
        /// </summary>
        /// <value>
        ///     The encrypted message.
        /// </value>
        public string EncryptedMessage { get; set; }

        #endregion
    }

    /// <summary>
    ///     Event Arguments that are passed when text decoding completes.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TextDecodingEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the decoded text.
        /// </summary>
        /// <value>
        ///     The decoded text.
        /// </value>
        public string DecodedText { get; set; }

        /// <summary>
        ///     Gets or sets the raw text.
        /// </summary>
        /// <value>
        ///     The raw text.
        /// </value>
        public string RawText { get; set; }

        /// <summary>
        ///     Gets or sets the keyword.
        /// </summary>
        /// <value>
        ///     The keyword.
        /// </value>
        public string Keyword { get; set; }

        #endregion
    }

    /// <summary>
    ///     Event Arguments that are passed when image encoding completes.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ImageEncodingEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the non encrypted message.
        /// </summary>
        /// <value>
        ///     The non encrypted message.
        /// </value>
        public WriteableBitmap NonEncryptedMessage { get; set; }

        /// <summary>
        ///     Gets or sets the encrypted message.
        /// </summary>
        /// <value>
        ///     The encrypted message.
        /// </value>
        public WriteableBitmap EncryptedMessage { get; set; }

        /// <summary>
        ///     Gets or sets the embed image.
        /// </summary>
        /// <value>
        ///     The embed image.
        /// </value>
        public WriteableBitmap EmbedImage { get; set; }

        #endregion
    }

    /// <summary>
    ///     Event Arguments that are passed when image decoding completes.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ImageDecodingEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the decoded image.
        /// </summary>
        /// <value>
        ///     The decoded image.
        /// </value>
        public WriteableBitmap DecodedImage { get; set; }

        /// <summary>
        ///     Gets or sets the raw image.
        /// </summary>
        /// <value>
        ///     The raw image.
        /// </value>
        public WriteableBitmap RawImage { get; set; }

        #endregion
    }
}