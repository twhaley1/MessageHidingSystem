using System;
using System.Text;

namespace GroupAStegafy.Model.MessageHiding.Helpers
{
    /// <summary>
    ///     A StringBuilder wrapper class that takes an end of string marker into account.
    /// </summary>
    public class MarkedStringBuilder
    {
        #region Data members

        private readonly StringBuilder builder;
        private readonly string endOfStringMarker;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether the end of this builder contains the end of string marker.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the end of this builder ends with the end of string marker; otherwise, <c>false</c>.
        /// </value>
        public bool HasEndMarkerBeenFound => this.builder.ToString().EndsWith(this.endOfStringMarker);

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MarkedStringBuilder" /> class.
        ///     Precondition: endOfStringMarker != null
        ///     Postcondition: endOfStringMarker is set to the parameter, and the builder is instantiated.
        /// </summary>
        /// <param name="endOfStringMarker">The end of string marker.</param>
        /// <exception cref="ArgumentNullException">endOfStringMarker</exception>
        public MarkedStringBuilder(string endOfStringMarker)
        {
            this.endOfStringMarker = endOfStringMarker ?? throw new ArgumentNullException(nameof(endOfStringMarker));
            this.builder = new StringBuilder();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Converts to string.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.builder.ToString();
        }

        /// <summary>
        ///     Appends the specified item to this builder as long as the end of string
        ///     marker has not been found.
        ///     Precondition: !HasEndMarkerBeenFound
        ///     Postcondition: if the builder end marker has not been found, append the item to the builder.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>This builder.</returns>
        /// <exception cref="ArgumentNullException">item</exception>
        public StringBuilder Append(string item)
        {
            if (!this.HasEndMarkerBeenFound)
            {
                this.builder.Append(item ?? throw new ArgumentNullException(nameof(item)));
            }

            return this.builder;
        }

        #endregion
    }
}