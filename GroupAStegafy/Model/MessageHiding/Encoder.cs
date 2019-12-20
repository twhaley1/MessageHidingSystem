using System;
using System.Threading.Tasks;
using GroupAStegafy.Model.DataContainers;
using GroupAStegafy.Model.MessageHiding.Helpers;
using GroupAStegafy.Utility;

namespace GroupAStegafy.Model.MessageHiding
{
    /// <summary>
    ///     An abstract representation of a base Encoder.
    /// </summary>
    public abstract class Encoder
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        /// <value>
        ///     The header.
        /// </value>
        public Header Header { get; protected set; }

        /// <summary>
        ///     Gets the primary image.
        /// </summary>
        /// <value>
        ///     The primary image.
        /// </value>
        public ImageData PrimaryImage { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Encoder" /> class.
        ///     Precondition: primaryImage != null
        ///     Post-condition: PrimaryImage.Equals(primaryImage) and Header != null
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <exception cref="ArgumentNullException">primaryImage</exception>
        protected Encoder(ImageData primaryImage)
        {
            this.PrimaryImage = primaryImage ?? throw new ArgumentNullException(nameof(primaryImage));
            this.Header = new Header();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Encodes a message message into the PrimaryImage.
        /// </summary>
        /// <returns>Task.Completed.</returns>
        public abstract Task EncodeMessage();

        /// <summary>
        ///     Sets the properties inside Header. WriteHeader will write
        ///     these values to the PrimaryImage's pixels.
        /// </summary>
        protected abstract void SetupHeader();

        /// <summary>
        ///     Writes the Header's property colors into the PrimaryImage's pixels.
        /// </summary>
        protected void WriteHeader()
        {
            this.SetupHeader();

            ByteAnalysis.SetPixelBgra8(this.PrimaryImage.Pixels, 0, 0, this.Header.HiddenMessageIdentifier,
                this.PrimaryImage.Width);
            ByteAnalysis.SetPixelBgra8(this.PrimaryImage.Pixels, 0, 1, this.Header.HiddenMessageDescriber,
                this.PrimaryImage.Width);
        }

        /// <summary>
        ///     Determines whether the given x and y pair is part of a header pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        ///     <c>true</c> if the x, y pair is not a header pixel; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsNotHeaderPixel(int x, int y)
        {
            return !(x == 0 && y < 2);
        }

        /// <summary>
        ///     Occurs when the encoding completes.
        /// </summary>
        public event EventHandler EncodingComplete;

        /// <summary>
        ///     Raises the <see cref="E:EncodingComplete" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnEncodingComplete(EventArgs args)
        {
            this.EncodingComplete?.Invoke(this, args);
        }

        #endregion
    }
}