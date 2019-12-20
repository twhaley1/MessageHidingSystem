using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroupAStegafy.Extensions;

namespace GroupAStegafy.Model.MessageHiding.Helpers
{
    /// <summary>
    ///     Manages a collection of bits for turning bytes into strings.
    /// </summary>
    public class BitBuilder
    {
        #region Data members

        private const int ByteSize = 8;

        private readonly List<bool> bits;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BitBuilder" /> class.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        public BitBuilder()
        {
            this.bits = new List<bool>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds the range of bits to this BitBuilder. When the size of this BitBuilder
        ///     equal to 1 byte, then this method removes that byte and returns that byte's
        ///     character representation as a string. If this BitBuilder has not yet reached
        ///     the size of one byte, then this method returns an empty string.
        ///     Precondition: none
        ///     Postcondition: the newBits are appended to bits
        /// </summary>
        /// <param name="newBits">The new bits.</param>
        /// <returns>the character string pulled from the front of this builder.</returns>
        public string AddRange(IList<bool> newBits)
        {
            var newBitsToAdd = this.bits.Count + newBits.Count > ByteSize
                ? newBits.TakeLast(8 - this.bits.Count)
                : newBits;
            this.bits.AddRange(newBitsToAdd);

            if (this.bits.Count == ByteSize)
            {
                this.bits.Reverse();
                var characterArray = this.bits.ToArray();
                this.bits.Clear();

                return Encoding.ASCII.GetString(new[] {characterArray.ToByte()});
            }

            return string.Empty;
        }

        #endregion
    }
}