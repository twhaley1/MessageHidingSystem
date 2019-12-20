using System;
using System.Threading.Tasks;
using GroupAStegafy.Model.DataContainers;

namespace GroupAStegafy.Model.MessageHiding
{
    /// <summary>
    ///     An abstract representation of a base Decoder.
    /// </summary>
    public abstract class Decoder
    {
        #region Properties

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
        ///     Initializes a new instance of the <see cref="Decoder" /> class.
        ///     Precondition: primaryImage != null
        ///     Post-condition: PrimaryImage.Equals(primaryImage)
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <exception cref="ArgumentNullException">primaryImage</exception>
        protected Decoder(ImageData primaryImage)
        {
            this.PrimaryImage = primaryImage ?? throw new ArgumentNullException(nameof(primaryImage));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Decodes a message from the PrimaryImage.
        /// </summary>
        /// <returns>Task.Completed.</returns>
        public abstract Task DecodeMessage();

        /// <summary>
        ///     Occurs when the decoding has finished.
        /// </summary>
        public event EventHandler<EventArgs> DecodingComplete;

        /// <summary>
        ///     Raises the <see cref="E:DecodingComplete" /> event.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected virtual void OnDecodingComplete(EventArgs args)
        {
            this.DecodingComplete?.Invoke(this, args);
        }

        #endregion
    }
}