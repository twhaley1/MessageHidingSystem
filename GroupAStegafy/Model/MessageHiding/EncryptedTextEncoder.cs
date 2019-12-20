using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GroupAStegafy.Extensions;
using GroupAStegafy.Model.DataContainers;
using GroupAStegafy.Model.MessageHiding.Encryption;
using GroupAStegafy.Model.MessageHiding.Helpers.EventHelpers;

namespace GroupAStegafy.Model.MessageHiding
{
    /// <summary>
    ///     An Encoder that encodes encrypted text within an image.
    /// </summary>
    /// <seealso cref="GroupAStegafy.Model.MessageHiding.TextEncoder" />
    public class EncryptedTextEncoder : TextEncoder
    {
        #region Data members

        private readonly TextEncrypter encrypter;

        #endregion

        #region Properties

        private string EncryptedMessage { get; set; }

        private string NonEncryptedMessage { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EncryptedTextEncoder" /> class.
        ///     Precondition: primaryImage != null and message != null and Encrypter != null
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <param name="message">The message.</param>
        /// <param name="bitsPerColorChannel">The bits per color channel.</param>
        /// <param name="encrypter">The Encrypter.</param>
        /// <exception cref="ArgumentNullException">Encrypter</exception>
        protected EncryptedTextEncoder(ImageData primaryImage, string message, int bitsPerColorChannel,
            TextEncrypter encrypter) :
            base(primaryImage, message, bitsPerColorChannel)
        {
            this.encrypter = encrypter ?? throw new ArgumentNullException(nameof(encrypter));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates an EncryptedTextEncoder asynchronously to ensure tha the PrimaryImage has had time to have its data
        ///     initialized.
        ///     Precondition: primaryImage != null and message != null and Encrypter != null
        ///     Post-condition: PrimaryImage has valid data.
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <param name="message">The message.</param>
        /// <param name="bitsPerColorChannel">The bits per color channel.</param>
        /// <param name="encrypter">The Encrypter.</param>
        /// <returns></returns>
        public static async Task<EncryptedTextEncoder> CreateAsync(ImageData primaryImage, string message,
            int bitsPerColorChannel, TextEncrypter encrypter)
        {
            var encoder = new EncryptedTextEncoder(primaryImage, message, bitsPerColorChannel, encrypter);
            await encoder.PrimaryImage.InitializeImageData();

            return encoder;
        }

        /// <summary>
        ///     Encodes the encrypted message within the primary image.
        /// </summary>
        public override async Task EncodeMessage()
        {
            this.NonEncryptedMessage = OriginalMessage;
            this.EncryptedMessage = this.encrypter.Encrypt(Regex.Replace(this.NonEncryptedMessage, @"\W", ""));

            OriginalMessage = this.encrypter.Keyword + Settings.EncryptionTag + this.EncryptedMessage +
                              Settings.EndOfMessageCode;
            MessageQueue = BuildMessageEncodingQueueFrom(Encoding.ASCII.GetBytes(OriginalMessage));

            await EncodeText();

            OnEncodingComplete(new TextEncodingEventArgs {
                EmbedImage = EmbedImage,
                NonEncryptedMessage = this.NonEncryptedMessage,
                EncryptedMessage = this.EncryptedMessage
            });
        }

        /// <summary>
        ///     Setups the header.
        /// </summary>
        protected override void SetupHeader()
        {
            base.SetupHeader();

            var secondPrimaryPixel = Header.HiddenMessageDescriber;
            var red = secondPrimaryPixel.R.SetBit(0, true);

            Header.HiddenMessageDescriber =
                Color.FromArgb(0, red, secondPrimaryPixel.G, secondPrimaryPixel.B);
        }

        #endregion
    }
}