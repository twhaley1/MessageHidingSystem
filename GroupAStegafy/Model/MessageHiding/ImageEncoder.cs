using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using GroupAStegafy.Extensions;
using GroupAStegafy.Model.DataContainers;
using GroupAStegafy.Model.MessageHiding.Helpers.EventHelpers;
using GroupAStegafy.Utility;

namespace GroupAStegafy.Model.MessageHiding
{
    /// <summary>
    ///     An Encoder that encodes an image within another image.
    /// </summary>
    /// <seealso cref="GroupAStegafy.Model.MessageHiding.Encoder" />
    public class ImageEncoder : Encoder
    {
        #region Properties

        /// <summary>
        ///     Gets the secondary image.
        /// </summary>
        /// <value>
        ///     The secondary image.
        /// </value>
        public ImageData SecondaryImage { get; }

        /// <summary>
        ///     Gets or sets the embed image.
        /// </summary>
        /// <value>
        ///     The embed image.
        /// </value>
        protected WriteableBitmap EmbedImage { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageEncoder" /> class.
        ///     Precondition: primaryImage != null and secondaryImage != null
        ///     Post-condition: PrimaryImage.Equals(primaryImage) and SecondaryImage.Equals(secondaryImage)
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <param name="secondaryImage">The secondary image.</param>
        /// <exception cref="ArgumentNullException">secondaryImage</exception>
        protected ImageEncoder(ImageData primaryImage, ImageData secondaryImage) : base(primaryImage)
        {
            this.SecondaryImage = secondaryImage ?? throw new ArgumentNullException(nameof(secondaryImage));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates an ImageEncoder asynchronously. This is important because it allows the PrimaryImage and
        ///     SecondaryImage to have their image data initialized.
        ///     Precondition: primaryImage != null and secondaryImage != null
        ///     Post-condition: The PrimaryImage and SecondaryImage have their image data initialized within the
        ///     returned ImageEncoder
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <param name="secondaryImage">The secondary image.</param>
        /// <returns>An ImageEncoder that has its PrimaryImage and SecondaryImage with valid data.</returns>
        public static async Task<ImageEncoder> CreateAsync(ImageData primaryImage,
            ImageData secondaryImage)
        {
            var encoder = new ImageEncoder(primaryImage, secondaryImage);
            await encoder.PrimaryImage.InitializeImageData();
            await encoder.SecondaryImage.InitializeImageData();

            return encoder;
        }

        /// <summary>
        ///     Encodes the SecondaryImage into the PrimaryImage.
        ///     Precondition: The images must be compatible with each other. The width and height of the
        ///     SecondaryImage must be less than or equal to the width and height of the PrimaryImage.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the images are incompatible.</exception>
        public override async Task EncodeMessage()
        {
            if (this.AreImagesIncompatible())
            {
                throw new InvalidOperationException();
            }

            WriteHeader();
            PrimaryImage.ForEachPixel(this.EncodeMonochromeAt, IsNotHeaderPixel);
            this.EmbedImage =
                await PrimaryImage.Pixels.ToWriteableBitmapAsync((int) PrimaryImage.Width,
                    (int) PrimaryImage.Height);

            OnEncodingComplete(new ImageEncodingEventArgs {
                EmbedImage = this.EmbedImage,
                EncryptedMessage = null,
                NonEncryptedMessage = null
            });
        }

        /// <summary>
        ///     If the given x, y pair lies within the bounds of the SecondaryImage, then the corresponding
        ///     pixel in the PrimaryImage has its least significant bit in the blue color channel to a value
        ///     that encodes the color of the SecondaryImage.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        protected void EncodeMonochromeAt(int x, int y)
        {
            var primaryPixel =
                ByteAnalysis.GetPixelBgra8(PrimaryImage.Pixels, x, y, PrimaryImage.Width);

            var sizeExceededFlag = x >= this.SecondaryImage.Height || y >= this.SecondaryImage.Width;

            var newColor = sizeExceededFlag
                ? primaryPixel
                : this.makeMonochrome(primaryPixel,
                    ByteAnalysis.GetPixelBgra8(this.SecondaryImage.Pixels, x, y, this.SecondaryImage.Width));

            ByteAnalysis.SetPixelBgra8(PrimaryImage.Pixels, x, y, newColor, PrimaryImage.Width);
        }

        private Color makeMonochrome(Color primaryPixel, Color secondaryPixel)
        {
            var black = Color.FromArgb(0, 0, 0, 0);
            var blueBit = primaryPixel.B;
            blueBit = secondaryPixel.Equals(black) ? blueBit.SetBit(0, true) : blueBit.SetBit(0, false);

            return Color.FromArgb(0, primaryPixel.R, primaryPixel.G, blueBit);
        }

        /// <summary>
        ///     Defines the color objects Header.HiddenMessageIdentifier and
        ///     Header.HiddenMessageDescriber,
        /// </summary>
        protected override void SetupHeader()
        {
            Header.HiddenMessageIdentifier = Color.FromArgb(0, 212, 212, 212);

            var secondPrimaryPixel =
                ByteAnalysis.GetPixelBgra8(PrimaryImage.Pixels, 0, 1, PrimaryImage.Width);
            var red = secondPrimaryPixel.R.SetBit(0, false);
            var blue = secondPrimaryPixel.B.SetBit(0, false);

            Header.HiddenMessageDescriber =
                Color.FromArgb(0, red, secondPrimaryPixel.G, blue);
        }

        /// <summary>
        ///     Ares the images incompatible.
        /// </summary>
        /// <returns>True if the images are incompatible; false otherwise.</returns>
        protected bool AreImagesIncompatible()
        {
            return PrimaryImage.Width < this.SecondaryImage.Width ||
                   PrimaryImage.Height < this.SecondaryImage.Height;
        }

        #endregion
    }
}