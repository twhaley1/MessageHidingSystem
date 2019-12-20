namespace GroupAStegafy.Model.MessageHiding.Helpers
{
    /// <summary>
    ///     Provides a way to see if a string will fit in an image of
    ///     a specified width and height. If the string will not fit,
    ///     this class provides information about what bits per color
    ///     channel value might make the string fit.
    /// </summary>
    public class ImageTextQuery
    {
        #region Data members

        private const int ByteSize = 8;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the text.
        /// </summary>
        /// <value>
        ///     The text.
        /// </value>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public int Width { get; set; } = 1;

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height { get; set; } = 1;

        private int NumberOfPixels => this.Width * this.Height;

        private int NumberOfCharacters => this.Text.Length;

        private int AvailableColorChannels => 3 * (this.NumberOfPixels - 2);

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the suggested bits per color channel value that
        ///     might make the string fit into the image.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <returns>the suggested bits per color channel value.</returns>
        public int CalculateSuggestedBitsPerColorChannel()
        {
            var suggestedBitsPerColorChannel = int.MaxValue;
            for (var i = 1; i <= ByteSize; i++)
            {
                var channelsRequired = this.NumberOfCharacters * this.calculateColorChannelsRequiredPerChar(i);
                if (channelsRequired < this.AvailableColorChannels)
                {
                    suggestedBitsPerColorChannel = i;
                }
            }

            return suggestedBitsPerColorChannel;
        }

        /// <summary>
        ///     Determines whether this ImageTextQuery's Text property will fit in the image
        ///     with the specified bits per color channel value.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="bitsPerColorChannel">The bits per color channel.</param>
        /// <returns>true if the text will not fit in the image; false otherwise.</returns>
        public bool WillNotHoldText(int bitsPerColorChannel)
        {
            return this.NumberOfCharacters * this.calculateColorChannelsRequiredPerChar(bitsPerColorChannel) >
                   this.AvailableColorChannels;
        }

        private int calculateColorChannelsRequiredPerChar(int bitsPerColorChannel)
        {
            var colorChannelsRequired = ByteSize / bitsPerColorChannel;

            return ByteSize % bitsPerColorChannel != 0 ? colorChannelsRequired + 1 : colorChannelsRequired;
        }

        #endregion
    }
}