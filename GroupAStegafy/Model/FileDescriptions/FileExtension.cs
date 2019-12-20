using System;

namespace GroupAStegafy.Model.FileDescriptions
{
    /// <summary>
    ///     Wraps a string that is intended to represent a file extension such as '.jpg', '.docx', etc..
    /// </summary>
    public class FileExtension
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the extension.
        /// </summary>
        /// <value>
        ///     The extension.
        /// </value>
        public string Extension { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Determines if the specified string matches this file extension.
        ///     Precondition: none
        ///     Postcondition: None
        /// </summary>
        /// <param name="other">The string to be compared to.</param>
        /// <returns>true if the specified string matches this file extension, false otherwise.</returns>
        public bool ExtensionEquals(string other)
        {
            return this.Extension.Equals(other, StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion
    }
}