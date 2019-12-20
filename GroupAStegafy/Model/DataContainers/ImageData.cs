using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using GroupAStegafy.Utility;

namespace GroupAStegafy.Model.DataContainers
{
    /// <summary>
    ///     Contains data for a passed in image file.
    /// </summary>
    public class ImageData
    {
        #region Types and Delegates

        /// <summary>
        ///     Determines whether a PixelOperation can be used at position x, y.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>True if the PixelOperation can be used; false otherwise.</returns>
        public delegate bool CanUsePixelOperation(int x, int y);

        /// <summary>
        ///     An operation on the pixel located at position x, y.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public delegate void PixelOperation(int x, int y);

        #endregion

        #region Properties

        /// <summary>  Contains a byte array of pixels of the file</summary>
        /// <value>The pixels.</value>
        public byte[] Pixels { get; set; }

        /// <summary>  Contains the width of the file </summary>
        /// <value>The width.</value>
        public uint Width { get; set; }

        /// <summary>Contains the height of the file</summary>
        /// <value>The height.</value>
        public uint Height { get; set; }

        /// <summary>
        ///     Gets or sets the image.
        /// </summary>
        /// <value>
        ///     The image.
        /// </value>
        public BitmapImage Image { get; set; }

        /// <summary>Contains the passed in file.</summary>
        /// <value>The file.</value>
        public StorageFile File { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageData" /> class.
        ///     Precondition: file != null
        ///     Post-condition: File.Equals(file)
        /// </summary>
        /// <param name="file">The file.</param>
        public ImageData(StorageFile file)
        {
            this.File = file ?? throw new ArgumentNullException(nameof(file));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes the file data.
        ///     Post-condition: Image != null and Width == width of the image and
        ///     Height == height of the image and Pixels == pixel data of
        ///     the image.
        ///     Precondition: none
        ///     Postcondition: Assigns the Image property, as well as the Height, Width, and Pixels
        /// </summary>
        public async Task InitializeImageData()
        {
            this.Image = await FileAccessor.LoadImageSource(this.File);

            using (var fileStream = await this.File.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                var unsignedWidth = Convert.ToUInt32(this.Image.PixelWidth);
                var unsignedHeight = Convert.ToUInt32(this.Image.PixelHeight);

                var transform = new BitmapTransform {
                    ScaledWidth = unsignedWidth,
                    ScaledHeight = unsignedHeight
                };

                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage
                );

                this.Width = unsignedWidth;
                this.Height = unsignedHeight;
                this.Pixels = pixelData.DetachPixelData();
            }
        }

        /// <summary>
        ///     Performs the specified PixelOperation on each pixel as long as the
        ///     CanUsePixelOperation returns true.
        ///     Precondition: none
        ///     Postcondition: operate action is preformed if the operation is valid.
        /// </summary>
        /// <param name="operate">The operate.</param>
        /// <param name="canOperate">The can operate.</param>
        public void ForEachPixel(PixelOperation operate, CanUsePixelOperation canOperate)
        {
            for (var i = 0; i < this.Height; i++)
            for (var j = 0; j < this.Width; j++)
            {
                if (canOperate(i, j))
                {
                    operate(i, j);
                }
            }
        }

        #endregion
    }
}