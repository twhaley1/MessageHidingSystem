using System.Collections.Generic;
using System.Linq;
using GroupAStegafy.Extensions;

namespace GroupAStegafy.Model.MessageHiding.Encryption
{
    /// <summary>
    ///     An Encrypter that handles encrypting and decrypting the bytes associated
    ///     with an image.
    /// </summary>
    /// <seealso cref="byte" />
    public class ImageEncrypter : IEncryption<byte[]>
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Encrypts the specified item.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="item">The item to be encrypted.</param>
        /// <returns>
        ///     The encrypted item.
        /// </returns>
        public byte[] Encrypt(byte[] item)
        {
            var splitArray = item.SplitElementsBy(item.Length / this.Height).ToList();

            var half = splitArray.Count / 2;
            var topHalf = splitArray.Take(half).ToArray().CombineContents();
            half = splitArray.Count - half;
            var bottomHalf = splitArray.TakeLast(half).ToArray().CombineContents();

            var reversedQuadrants = new List<byte[]> {bottomHalf, topHalf};

            return reversedQuadrants.ToArray().CombineContents();
        }

        /// <summary>
        ///     Decrypts the specified item.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="item">The item to be decrypted.</param>
        /// <returns>
        ///     The decrypted item.
        /// </returns>
        public byte[] Decrypt(byte[] item)
        {
            var splitArray = item.SplitElementsBy(item.Length / this.Height).ToList();

            var secondHalf = splitArray.Count / 2;
            var firstHalf = splitArray.Count - secondHalf;
            var topHalf = splitArray.Take(firstHalf).ToArray().CombineContents();
            var bottomHalf = splitArray.TakeLast(secondHalf).ToArray().CombineContents();

            var reversedQuadrants = new List<byte[]> {bottomHalf, topHalf};

            return reversedQuadrants.ToArray().CombineContents();
        }

        #endregion
    }
}