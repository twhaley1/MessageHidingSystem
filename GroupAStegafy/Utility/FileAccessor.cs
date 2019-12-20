using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupAStegafy.Utility
{
    /// <summary>
    ///     Contains utility methods for retrieving content from files. All methods in
    ///     this class should be used with await.
    /// </summary>
    public static class FileAccessor
    {
        #region Methods

        /// <summary>
        ///     Loads an BitmapImage object from the specified file.
        /// </summary>
        /// <param name="imageFile">The image file.</param>
        /// <returns>An BitmapImage object.</returns>
        public static async Task<BitmapImage> LoadImageSource(StorageFile imageFile)
        {
            var image = new BitmapImage();
            using (var randomAccessStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                await image.SetSourceAsync(randomAccessStream);
            }

            return image;
        }

        /// <summary>
        ///     Loads a string from the specified file.
        /// </summary>
        /// <param name="textFile">The text file.</param>
        /// <returns>The contents of the specified file as a string.</returns>
        public static async Task<string> LoadText(StorageFile textFile)
        {
            string contents;
            using (var stream = new StreamReader(await textFile.OpenStreamForReadAsync()))
            {
                contents = await stream.ReadToEndAsync();
            }

            return contents;
        }

        #endregion
    }
}