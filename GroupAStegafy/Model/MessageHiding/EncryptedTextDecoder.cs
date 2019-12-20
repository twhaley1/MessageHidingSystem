using System;
using System.Threading.Tasks;
using GroupAStegafy.Model.DataContainers;
using GroupAStegafy.Model.MessageHiding.Encryption;
using GroupAStegafy.Model.MessageHiding.Helpers.EventHelpers;

namespace GroupAStegafy.Model.MessageHiding
{
    /// <summary>
    ///     A Decoder that decodes encrypted text from within an image.
    /// </summary>
    /// <seealso cref="GroupAStegafy.Model.MessageHiding.TextDecoder" />
    public class EncryptedTextDecoder : TextDecoder
    {
        #region Data members

        private readonly TextEncrypter decrypter;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the keyword.
        /// </summary>
        /// <value>
        ///     The keyword.
        /// </value>
        public string Keyword { get; private set; }

        /// <summary>
        ///     Gets the raw text.
        /// </summary>
        /// <value>
        ///     The raw text.
        /// </value>
        public string RawText { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EncryptedTextDecoder" /> class.
        ///     Precondition: primaryImage != null and decrypter != null
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <param name="bitsPerColorChannel">The bits per color channel.</param>
        /// <param name="decrypter">The decrypter.</param>
        /// <exception cref="ArgumentNullException">decrypter</exception>
        public EncryptedTextDecoder(ImageData primaryImage, int bitsPerColorChannel, TextEncrypter decrypter) : base(
            primaryImage, bitsPerColorChannel)
        {
            this.decrypter = decrypter ?? throw new ArgumentNullException(nameof(decrypter));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Decodes the encrypted text from within the message.
        /// </summary>
        /// <returns>Task.Completed.</returns>
        public override Task DecodeMessage()
        {
            DecodeText();
            var keywordResultPair = DecodedText.Split(Settings.EncryptionTag);
            this.Keyword = keywordResultPair[0];
            this.RawText = keywordResultPair[1];

            this.decrypter.Keyword = this.Keyword;
            DecodedText = this.decrypter.Decrypt(this.RawText);

            OnDecodingComplete(new TextDecodingEventArgs {
                DecodedText = DecodedText,
                RawText = this.RawText,
                Keyword = this.Keyword
            });
            return Task.CompletedTask;
        }

        #endregion
    }
}