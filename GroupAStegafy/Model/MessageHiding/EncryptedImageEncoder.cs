using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using GroupAStegafy.Extensions;
using GroupAStegafy.Model.DataContainers;
using GroupAStegafy.Model.MessageHiding.Encryption;
using GroupAStegafy.Model.MessageHiding.Helpers.EventHelpers;
using GroupAStegafy.Utility;

namespace GroupAStegafy.Model.MessageHiding
{
    /// <summary>
    ///     An Encoder that encodes an encrypted image into another image.
    /// </summary>
    /// <seealso cref="GroupAStegafy.Model.MessageHiding.ImageEncoder" />
    public class EncryptedImageEncoder : ImageEncoder
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the encrypter.
        /// </summary>
        /// <value>
        ///     The encrypter.
        /// </value>
        public ImageEncrypter Encrypter { get; }

        private WriteableBitmap NonEncryptedMessage { get; set; }

        private WriteableBitmap EncryptedMessage { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EncryptedImageEncoder" /> class.
        ///     Precondition: primaryImage != null and secondaryImage != null and Encrypter != null
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <param name="secondaryImage">The secondary image.</param>
        /// <param name="encrypter">The Encrypter.</param>
        /// <exception cref="ArgumentNullException">Encrypter</exception>
        protected EncryptedImageEncoder(ImageData primaryImage, ImageData secondaryImage, ImageEncrypter encrypter) :
            base(primaryImage, secondaryImage)
        {
            this.Encrypter = encrypter ?? throw new ArgumentNullException(nameof(encrypter));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates an EncryptedImageEncoder asynchronously in order to ensure that PrimaryImage and
        ///     SecondaryImage have all of their data initialized. It is important to create an EncryptedImageEncoder
        ///     in this manner because there is a considerable amount of data that relies on this file I/O to work
        ///     properly.
        ///     Precondition: primaryImage != null and secondaryImage != null and Encrypter != null
        ///     Post-condition: PrimaryImage and SecondaryImage of the returned EncryptedImageEncoder have valid data.
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <param name="secondaryImage">The secondary image.</param>
        /// <param name="encrypter">The Encrypter.</param>
        /// <returns>An EncryptedImageEncoder with its PrimaryImage and SecondaryImage validated.</returns>
        public static async Task<EncryptedImageEncoder> CreateAsync(ImageData primaryImage,
            ImageData secondaryImage, ImageEncrypter encrypter)
        {
            var encoder = new EncryptedImageEncoder(primaryImage, secondaryImage, encrypter);
            await encoder.PrimaryImage.InitializeImageData();
            await encoder.SecondaryImage.InitializeImageData();
            encoder.Encrypter.Height = (int) primaryImage.Height;

            return encoder;
        }

        /// <summary>
        ///     Encodes the encrypted SecondaryImage within the PrimaryImage.
        ///     Precondition: The images must be compatible. The width and height of the SecondaryImage
        ///     must both be less than or equal to the width and height, respectively, of the PrimaryImage.
        ///     Post-condition: PrimaryImage now has the message encrypted within it. EmbedImage contains the same image.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the Images are incompatible with each other.</exception>
        public override async Task EncodeMessage()
        {
            if (AreImagesIncompatible())
            {
                throw new InvalidOperationException();
            }

            this.NonEncryptedMessage =
                await SecondaryImage.Pixels.ToWriteableBitmapAsync((int) SecondaryImage.Width,
                    (int) SecondaryImage.Height);
            this.expandSecondaryDataSizeToPrimary();
            SecondaryImage.Pixels = this.Encrypter.Encrypt(SecondaryImage.Pixels);
            this.EncryptedMessage =
                await SecondaryImage.Pixels.ToWriteableBitmapAsync((int) SecondaryImage.Width,
                    (int) SecondaryImage.Height);

            WriteHeader();
            PrimaryImage.ForEachPixel(EncodeMonochromeAt, IsNotHeaderPixel);
            EmbedImage =
                await PrimaryImage.Pixels.ToWriteableBitmapAsync((int) PrimaryImage.Width,
                    (int) PrimaryImage.Height);

            OnEncodingComplete(new ImageEncodingEventArgs {
                EmbedImage = EmbedImage,
                EncryptedMessage = this.EncryptedMessage,
                NonEncryptedMessage = this.NonEncryptedMessage
            });
        }

        private void expandSecondaryDataSizeToPrimary()
        {
            var newPixels = new byte[PrimaryImage.Pixels.Length];

            PrimaryImage.ForEachPixel((x, y) =>
            {
                if (x < SecondaryImage.Height && y < SecondaryImage.Width)
                {
                    ByteAnalysis.SetPixelBgra8(newPixels, x, y,
                        ByteAnalysis.GetPixelBgra8(SecondaryImage.Pixels, x, y, SecondaryImage.Width),
                        PrimaryImage.Width);
                }
                else
                {
                    ByteAnalysis.SetPixelBgra8(newPixels, x, y, Color.FromArgb(0, 255, 255, 255), PrimaryImage.Width);
                }
            }, (x, y) => true);

            SecondaryImage.Pixels = newPixels;
            SecondaryImage.Width = PrimaryImage.Width;
            SecondaryImage.Height = PrimaryImage.Height;
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