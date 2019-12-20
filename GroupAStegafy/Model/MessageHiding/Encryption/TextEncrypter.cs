using System.Text;
using System.Text.RegularExpressions;

namespace GroupAStegafy.Model.MessageHiding.Encryption
{
    /// <summary>
    ///     An Encrypter that handles encrypting/decrypting text.
    /// </summary>
    /// <seealso cref="string" />
    public class TextEncrypter : IEncryption<string>
    {
        #region Data members

        private string keyword;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the keyword.
        /// </summary>
        /// <value>
        ///     The keyword.
        /// </value>
        public string Keyword
        {
            get => this.keyword;
            set => this.keyword = Regex.Replace(value, @"\W", "");
        }

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
        public string Encrypt(string item)
        {
            var encryptionTable = new VigenereEncryptionTable(this.Keyword.ToUpper());
            var encryptedText = new StringBuilder();
            foreach (var character in item.ToUpper().ToCharArray())
            {
                encryptedText.Append(encryptionTable.GetEncryptedCharacter(character));
            }

            return encryptedText.ToString();
        }

        /// <summary>
        ///     Decrypts the specified item.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="item">The item that is decrypted.</param>
        /// <returns>
        ///     The decrypted item.
        /// </returns>
        public string Decrypt(string item)
        {
            var encryptionTable = new VigenereEncryptionTable(this.Keyword.ToUpper());

            var builder = new StringBuilder();
            foreach (var letter in item.ToUpper().ToCharArray())
            {
                builder.Append(encryptionTable.GetDecryptedCharacter(letter));
            }

            return builder.ToString();
        }

        #endregion
    }
}