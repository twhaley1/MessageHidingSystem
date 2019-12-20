using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using GroupAStegafy.Extensions;
using GroupAStegafy.Model.DataContainers;
using GroupAStegafy.Model.MessageHiding.Helpers;
using GroupAStegafy.Model.MessageHiding.Helpers.EventHelpers;
using GroupAStegafy.Utility;

namespace GroupAStegafy.Model.MessageHiding
{
    /// <summary>
    ///     An Encoder that encodes text into a PrimaryImage.
    /// </summary>
    /// <seealso cref="GroupAStegafy.Model.MessageHiding.Encoder" />
    public class TextEncoder : Encoder
    {
        #region Data members

        private readonly int bitsPerColorChannel;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the suggested bits per color channel.
        /// </summary>
        /// <value>
        ///     The suggested bits per color channel.
        /// </value>
        public int SuggestedBitsPerColorChannel { get; private set; }

        /// <summary>
        ///     Gets or sets the original message.
        /// </summary>
        /// <value>
        ///     The original message.
        /// </value>
        protected string OriginalMessage { get; set; }

        /// <summary>
        ///     Gets or sets the embed image.
        /// </summary>
        /// <value>
        ///     The embed image.
        /// </value>
        protected WriteableBitmap EmbedImage { get; set; }

        /// <summary>
        ///     Gets or sets the message queue.
        /// </summary>
        /// <value>
        ///     The message queue.
        /// </value>
        protected Queue<bool[]> MessageQueue { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextEncoder" /> class.
        ///     Precondition: primaryImage != null and message != null
        ///     Post-condition: PrimaryImage.Equals(primaryImage) and SuggestedBitsPerColorChannel == bitsPerColorChannel
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <param name="message">The message.</param>
        /// <param name="bitsPerColorChannel">The bits per color channel.</param>
        /// <exception cref="ArgumentNullException">message</exception>
        protected TextEncoder(ImageData primaryImage, string message, int bitsPerColorChannel) : base(primaryImage)
        {
            this.OriginalMessage = message ?? throw new ArgumentNullException(nameof(message));
            this.bitsPerColorChannel = bitsPerColorChannel;
            this.SuggestedBitsPerColorChannel = bitsPerColorChannel;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates a TextEncoder asynchronously to ensure that the PrimaryImage has its data initialized properly.
        ///     Precondition: primaryImage != null and message != null
        ///     Post-condition: PrimaryImage has its data initialized in the returned TextEncoder.
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <param name="message">The message.</param>
        /// <param name="bitsPerColorChannel">The bits per color channel.</param>
        /// <returns>A TextEncoder with an initialized PrimaryContainer.</returns>
        public static async Task<TextEncoder> CreateAsync(ImageData primaryImage, string message,
            int bitsPerColorChannel)
        {
            var encoder = new TextEncoder(primaryImage, message, bitsPerColorChannel);
            await encoder.PrimaryImage.InitializeImageData();

            return encoder;
        }

        /// <summary>
        ///     Encodes a text message into the PrimaryImage.
        /// </summary>
        public override async Task EncodeMessage()
        {
            this.OriginalMessage = Regex.Replace(this.OriginalMessage, @"\W", "") + Settings.EndOfMessageCode;
            this.MessageQueue = this.BuildMessageEncodingQueueFrom(Encoding.ASCII.GetBytes(this.OriginalMessage));
            await this.EncodeText();

            OnEncodingComplete(new TextEncodingEventArgs {
                EmbedImage = this.EmbedImage,
                NonEncryptedMessage = null,
                EncryptedMessage = null
            });
        }

        /// <summary>
        ///     Encodes the text. If the text will not fit in the image with the
        ///     current bits per color channel, then BitsPerColorChannel will be set to
        ///     a value that may make the message fit.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the text will not fit within the PrimaryImage.</exception>
        protected async Task EncodeText()
        {
            var imageQuery = new ImageTextQuery {
                Text = this.OriginalMessage,
                Width = (int) PrimaryImage.Width,
                Height = (int) PrimaryImage.Height
            };
            if (imageQuery.WillNotHoldText(this.bitsPerColorChannel))
            {
                this.SuggestedBitsPerColorChannel = imageQuery.CalculateSuggestedBitsPerColorChannel();
                throw new InvalidOperationException();
            }

            WriteHeader();
            PrimaryImage.ForEachPixel(this.embedText, IsNotHeaderPixel);
            this.EmbedImage =
                await PrimaryImage.Pixels.ToWriteableBitmapAsync((int) PrimaryImage.Width,
                    (int) PrimaryImage.Height);
        }

        private void embedText(int x, int y)
        {
            var primaryPixel =
                ByteAnalysis.GetPixelBgra8(PrimaryImage.Pixels, x, y, PrimaryImage.Width);

            var newColor = this.insertTextBits(primaryPixel);

            ByteAnalysis.SetPixelBgra8(PrimaryImage.Pixels, x, y, newColor, PrimaryImage.Width);
        }

        private Color insertTextBits(Color primaryPixel)
        {
            var newRed = primaryPixel.R;
            var newGreen = primaryPixel.G;
            var newBlue = primaryPixel.B;

            newRed = this.embedTextInColorChannel(newRed);
            newGreen = this.embedTextInColorChannel(newGreen);
            newBlue = this.embedTextInColorChannel(newBlue);

            return Color.FromArgb(0, newRed, newGreen, newBlue);
        }

        private byte embedTextInColorChannel(byte colorChannel)
        {
            if (this.MessageQueue.Count != 0)
            {
                var colorChannelArray = colorChannel.ToBooleanArray();
                var bits = this.MessageQueue.Dequeue().Reverse().ToArray();
                for (var i = 0; i < bits.Length; i++)
                {
                    colorChannelArray[i] = bits[i];
                }

                return colorChannelArray.ToByte();
            }

            return colorChannel;
        }

        /// <summary>
        ///     Setups the header.
        /// </summary>
        protected override void SetupHeader()
        {
            Header.HiddenMessageIdentifier = Color.FromArgb(0, 212, 212, 212);

            var secondPrimaryPixel =
                ByteAnalysis.GetPixelBgra8(PrimaryImage.Pixels, 0, 1, PrimaryImage.Width);
            var red = secondPrimaryPixel.R.SetBit(0, false);
            var green = this.bitsPerColorChannel;
            var blue = secondPrimaryPixel.B.SetBit(0, true);

            Header.HiddenMessageDescriber =
                Color.FromArgb(0, red, green, blue);
        }

        /// <summary>
        ///     Returns the message encoding queue derived from the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The message encoding queue.</returns>
        protected Queue<bool[]> BuildMessageEncodingQueueFrom(IEnumerable<byte> bytes)
        {
            var boolQueue = new Queue<bool[]>();
            foreach (var currentByte in bytes ?? Enumerable.Empty<byte>())
            {
                var currentBooleanArray = currentByte.ToBooleanArray();
                var splitArray = currentBooleanArray.ReverseSplitElementsBy(this.bitsPerColorChannel);
                foreach (var splitBooleanArray in splitArray)
                {
                    boolQueue.Enqueue(splitBooleanArray);
                }
            }

            return boolQueue;
        }

        #endregion
    }
}