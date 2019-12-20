using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using GroupAStegafy.Model.FileDescriptions;

namespace GroupAStegafy
{
    /// <summary>
    ///     Contains settings used by the application.
    /// </summary>
    public class Settings
    {
        #region Data members

        /// <summary>
        ///     The extension regex
        /// </summary>
        public const string ExtensionRegex = "\\.\\w+";

        /// <summary>
        ///     The end of message code
        /// </summary>
        public const string EndOfMessageCode = "#.-.-.-#";

        /// <summary>
        ///     The minimum bits per color channel
        /// </summary>
        public const int MinimumBitsPerColorChannel = 1;

        /// <summary>
        ///     The maximum bits per color channel
        /// </summary>
        public const int MaximumBitsPerColorChannel = 8;

        /// <summary>
        ///     The encryption tag
        /// </summary>
        public const string EncryptionTag = "#KEY#";

        #endregion

        #region Properties

        /// <summary>
        ///     Gets all valid extensions.
        /// </summary>
        /// <value>
        ///     All valid extensions.
        /// </value>
        public IEnumerable<FileExtension> AllValidExtensions => this
                                                                .ValidImageExtensions.Union(this.ValidTextExtensions)
                                                                .Union(this.ValidExportImageExtensions);

        /// <summary>
        ///     Gets the valid image extensions.
        /// </summary>
        /// <value>
        ///     The valid image extensions.
        /// </value>
        public IEnumerable<FileExtension> ValidImageExtensions { get; }

        /// <summary>
        ///     Gets the valid export image extensions.
        /// </summary>
        /// <value>
        ///     The valid export image extensions.
        /// </value>
        public IEnumerable<FileExtension> ValidExportImageExtensions { get; }

        /// <summary>
        ///     Gets the valid text extensions.
        /// </summary>
        /// <value>
        ///     The valid text extensions.
        /// </value>
        public IEnumerable<FileExtension> ValidTextExtensions { get; }

        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public Color Identifier { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Settings" /> class.
        /// </summary>
        public Settings()
        {
            this.ValidImageExtensions = new Collection<FileExtension> {
                new FileExtension {Extension = ".png"},
                new FileExtension {Extension = ".bmp"}
            };
            this.ValidExportImageExtensions = new Collection<FileExtension> {
                new FileExtension {Extension = ".bmp"}
            };
            this.ValidTextExtensions = new Collection<FileExtension> {
                new FileExtension {Extension = ".txt"}
            };
            this.Identifier = Color.FromArgb(0, 212, 212, 212);
        }

        #endregion
    }
}