using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using GroupAStegafy.Extensions;
using GroupAStegafy.Model.DataContainers;
using GroupAStegafy.Model.MessageHiding.Encryption;
using GroupAStegafy.Model.MessageHiding.Helpers.EventHelpers;

namespace GroupAStegafy.Model.MessageHiding
{
    /// <summary>
    ///     A Decoder that decodes a message and decrypts the results.
    /// </summary>
    /// <seealso cref="GroupAStegafy.Model.MessageHiding.ImageDecoder" />
    public class EncryptedImageDecoder : ImageDecoder
    {
        #region Data members

        private readonly ImageEncrypter decrypter;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the raw image.
        /// </summary>
        /// <value>
        ///     The raw image.
        /// </value>
        public WriteableBitmap RawImage { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EncryptedImageDecoder" /> class.
        ///     Precondition: primaryImage != null and decrypter != null
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <param name="decrypter">The decrypter.</param>
        /// <exception cref="ArgumentNullException">decrypter</exception>
        public EncryptedImageDecoder(ImageData primaryImage, ImageEncrypter decrypter) : base(primaryImage)
        {
            this.decrypter = decrypter ?? throw new ArgumentNullException(nameof(decrypter));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Decodes the message and decrypts the results.
        ///     Post-condition: DecodedImage contains valid WriteableBitmap of the decoded image.
        /// </summary>
        public override async Task DecodeMessage()
        {
            var decodedPixels = DecodePixels();
            this.RawImage =
                await decodedPixels.ToWriteableBitmapAsync((int) PrimaryImage.Width, (int) PrimaryImage.Height);
            decodedPixels = this.decrypter.Decrypt(decodedPixels);

            DecodedImage =
                await decodedPixels.ToWriteableBitmapAsync((int) PrimaryImage.Width, (int) PrimaryImage.Height);
            OnDecodingComplete(new ImageDecodingEventArgs {
                DecodedImage = DecodedImage,
                RawImage = this.RawImage
            });
        }

        #endregion
    }
}