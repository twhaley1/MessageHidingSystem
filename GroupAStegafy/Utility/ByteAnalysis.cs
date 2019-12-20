using System.Drawing;

namespace GroupAStegafy.Utility
{
    /// <summary>
    ///     Contains functionality to get or set different pixel values.
    /// </summary>
    public static class ByteAnalysis
    {
        #region Methods

        /// <summary>
        ///     Gets the pixel bgra8.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <returns>A Color object.</returns>
        public static Color GetPixelBgra8(byte[] pixels, int x, int y, uint width)
        {
            var offset = (x * (int) width + y) * 4;
            var red = pixels[offset + 2];
            var green = pixels[offset + 1];
            var blue = pixels[offset];

            return Color.FromArgb(0, red, green, blue);
        }

        /// <summary>
        ///     Sets the pixel bgra8.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        public static void SetPixelBgra8(byte[] pixels, int x, int y, Color color, uint width)
        {
            var offset = (x * (int) width + y) * 4;
            pixels[offset + 2] = color.R;
            pixels[offset + 1] = color.G;
            pixels[offset + 0] = color.B;
        }

        #endregion
    }
}