using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupAStegafy.Model.MessageHiding.Encryption
{
    /// <summary>Class responsible for encrypting a passed in character according to the Vigenere cipher.</summary>
    public class VigenereEncryptionTable
    {
        #region Data members

        private readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private readonly Dictionary<char, char[]> encryptionTable;
        private readonly char[] keyword;

        private int currentKeywordIndex;

        #endregion

        #region Properties

        private char NextKeywordCharacter
        {
            get
            {
                var nextKeywordCharacter = this.keyword[this.currentKeywordIndex];
                this.currentKeywordIndex++;
                if (this.currentKeywordIndex >= this.keyword.Length)
                {
                    this.currentKeywordIndex = 0;
                }

                return nextKeywordCharacter;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="VigenereEncryptionTable" /> class.
        ///     Precondition: keyword != null
        ///     Postcondition: keyword, encryption table, and currentKeywordIndex are set to default values.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        public VigenereEncryptionTable(string keyword)
        {
            this.keyword = keyword?.ToCharArray() ?? throw new ArgumentNullException(nameof(keyword));
            this.encryptionTable = new Dictionary<char, char[]>();
            this.currentKeywordIndex = 0;
            this.populateEncryptionTable();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns the encrypted version of the passed in character according to the keyword.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="character">The character to be encrypted.</param>
        /// <returns>The encrypted version of the passed in character according to the keyword.</returns>
        public char GetEncryptedCharacter(char character)
        {
            var charIndex = this.convertCharToIndex(this.NextKeywordCharacter);
            return this.encryptionTable[character][charIndex];
        }

        /// <summary>
        ///     Gets the decrypted character.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="character">The character to decrypt.</param>
        /// <returns>The decrypted character.</returns>
        public char GetDecryptedCharacter(char character)
        {
            var indexOfDecryptedCharacter = this.encryptionTable[this.NextKeywordCharacter].ToList().IndexOf(character);

            return this.alphabet[indexOfDecryptedCharacter];
        }

        private int convertCharToIndex(char character)
        {
            return this.alphabet.ToList().IndexOf(character);
        }

        private void populateEncryptionTable()
        {
            for (var i = 0; i < this.alphabet.Length; i++)
            {
                var row = new char[this.alphabet.Length];
                var count = i;
                for (var j = 0; j < this.alphabet.Length; j++)
                {
                    if (count >= row.Length)
                    {
                        count = 0;
                    }

                    row[j] = this.alphabet[count];
                    count++;
                }

                this.encryptionTable.Add(this.alphabet[i], row);
            }
        }

        #endregion
    }
}