using System.Drawing;
using System.Threading.Tasks;
using GroupAStegafy.Model.DataContainers;
using GroupAStegafy.Utility;

namespace GroupAStegafy.Model.MessageHiding.Helpers
{
    /// <summary>
    ///     Represents the header pixels in an embed image.
    /// </summary>
    public class Header
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the hidden message identifier.
        /// </summary>
        /// <value>
        ///     The hidden message identifier.
        /// </value>
        public Color HiddenMessageIdentifier { get; set; }

        /// <summary>
        ///     Gets or sets the hidden message describer.
        /// </summary>
        /// <value>
        ///     The hidden message describer.
        /// </value>
        public Color HiddenMessageDescriber { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Fetches the header data from the specified ImageData.
        ///     Precondition: none
        ///     Postcondition: the HiddenMessageIdentifier and HiddenMessageDescriber are instantiated
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A Header with the first two header pixels set to the Identifier and Describer, respectively</returns>
        public static async Task<Header> CreateAsync(ImageData data)
        {
            await data.InitializeImageData();

            var firstPixel = ByteAnalysis.GetPixelBgra8(data.Pixels, 0, 0, data.Width);
            var secondPixel = ByteAnalysis.GetPixelBgra8(data.Pixels, 0, 1, data.Width);

            return new Header {HiddenMessageIdentifier = firstPixel, HiddenMessageDescriber = secondPixel};
        }

        #endregion
    }
}