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
    ///     A Decoder that decodes an image from within an image.
    /// </summary>
    /// <seealso cref="GroupAStegafy.Model.MessageHiding.Decoder" />
    public class ImageDecoder : Decoder
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the decoded image.
        /// </summary>
        /// <value>
        ///     The decoded image.
        /// </value>
        protected WriteableBitmap DecodedImage { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageDecoder" /> class.
        ///     Precondition: primaryImage != null
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        public ImageDecoder(ImageData primaryImage) : base(primaryImage)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Decodes an image message from the PrimaryImage.
        /// </summary>
        public override async Task DecodeMessage()
        {
            var decodedPixels = this.DecodePixels();

            this.DecodedImage =
                await decodedPixels.ToWriteableBitmapAsync((int) PrimaryImage.Width, (int) PrimaryImage.Height);
            OnDecodingComplete(new ImageDecodingEventArgs {DecodedImage = this.DecodedImage});
        }

        /// <summary>
        ///     Decodes each pixel by analyzing the least significant bit of the blue color channel.
        /// </summary>
        /// <returns>The decoded pixels.</returns>
        protected byte[] DecodePixels()
        {
            var decodedPixels = new byte[PrimaryImage.Pixels.Length];
            PrimaryImage.ForEachPixel((x, y) =>
            {
                var currentPixel = ByteAnalysis.GetPixelBgra8(PrimaryImage.Pixels, x, y, PrimaryImage.Width);
                var decodedPixel = this.decodePixel(currentPixel);
                ByteAnalysis.SetPixelBgra8(decodedPixels, x, y, decodedPixel, PrimaryImage.Width);
            }, (x, y) => true);

            return decodedPixels;
        }

        private Color decodePixel(Color pixel)
        {
            var leastSignificantBitOfBlue = pixel.B.GetBit(0);

            return leastSignificantBitOfBlue ? Color.FromArgb(0, 0, 0, 0) : Color.FromArgb(0, 255, 255, 255);
        }

        #endregion
    }
}