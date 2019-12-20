using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GroupAStegafy.Extensions;
using GroupAStegafy.Model.DataContainers;
using GroupAStegafy.Model.MessageHiding.Helpers;
using GroupAStegafy.Model.MessageHiding.Helpers.EventHelpers;
using GroupAStegafy.Utility;

namespace GroupAStegafy.Model.MessageHiding
{
    /// <summary>
    ///     A Decoder that decodes text from within a PrimaryImage.
    /// </summary>
    /// <seealso cref="GroupAStegafy.Model.MessageHiding.Decoder" />
    public class TextDecoder : Decoder
    {
        #region Data members

        private readonly int bitsPerColorChannel;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the decoded text.
        /// </summary>
        /// <value>
        ///     The decoded text.
        /// </value>
        protected string DecodedText { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextDecoder" /> class.
        ///     Precondition: primaryImage != null
        ///     Post-condition: PrimaryImage.Equals(primaryImage)
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <param name="bitsPerColorChannel">The bits per color channel.</param>
        public TextDecoder(ImageData primaryImage, int bitsPerColorChannel) : base(primaryImage)
        {
            this.bitsPerColorChannel = bitsPerColorChannel;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Decodes a message from the PrimaryImage.
        /// </summary>
        /// <returns>
        ///     Task.Completed.
        /// </returns>
        public override Task DecodeMessage()
        {
            this.DecodeText();
            OnDecodingComplete(new TextDecodingEventArgs {DecodedText = this.DecodedText});
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Decodes the text by searching through pixels until Settings.EndOfMessageCode
        ///     has been found.
        /// </summary>
        protected void DecodeText()
        {
            var message = new MarkedStringBuilder(Settings.EndOfMessageCode);
            var currentX = 2;
            var currentY = 0;

            var bits = new BitBuilder();
            while (!message.HasEndMarkerBeenFound)
            {
                var currentPixelColor =
                    ByteAnalysis.GetPixelBgra8(PrimaryImage.Pixels, currentY, currentX, PrimaryImage.Width);

                var blueByte = currentPixelColor.B.ToBooleanArray();
                var greenByte = currentPixelColor.G.ToBooleanArray();
                var redByte = currentPixelColor.R.ToBooleanArray();

                var lastBlueBits = blueByte.Take(this.bitsPerColorChannel).Reverse().ToList();
                var lastGreenBits = greenByte.Take(this.bitsPerColorChannel).Reverse().ToList();
                var lastRedBits = redByte.Take(this.bitsPerColorChannel).Reverse().ToList();

                message.Append(bits.AddRange(lastRedBits));
                message.Append(bits.AddRange(lastGreenBits));
                message.Append(bits.AddRange(lastBlueBits));

                currentY = currentX + 1 >= PrimaryImage.Width ? currentY + 1 : currentY;
                currentX = currentX + 1 >= PrimaryImage.Width ? 0 : currentX + 1;
            }

            this.DecodedText = Regex.Replace(message.ToString(), Settings.EndOfMessageCode, "");
        }

        #endregion
    }
}