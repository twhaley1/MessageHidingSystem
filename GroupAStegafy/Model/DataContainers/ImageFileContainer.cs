using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupAStegafy.Model.DataContainers
{
    /// <summary>
    ///     Wraps information about an image file.
    /// </summary>
    public class ImageFileContainer
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the file.
        /// </summary>
        /// <value>
        ///     The file.
        /// </value>
        public StorageFile File { get; }

        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        /// <value>
        ///     The source.
        /// </value>
        public BitmapSource Source { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageFileContainer" /> class.
        ///     NOTE: It is preferred to use the static CreateAsync method to construct an
        ///     ImageFileContainer. Constructing with this constructor will result in
        ///     Source == null.
        ///     Precondition: file != null
        ///     Post-condition: File.Equals(file) and FileName.Equals(file.Name) and Source == null
        /// </summary>
        /// <param name="file">The file.</param>
        /// <exception cref="ArgumentNullException">file</exception>
        public ImageFileContainer(StorageFile file)
        {
            this.File = file ?? throw new ArgumentNullException(nameof(file));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImageFileContainer" /> class.
        ///     NOTE: Use this constructor when it is desirable to contain an image that is not
        ///     linked to a particular file. However, be aware that the ImageFileContainer resulting
        ///     from this constructor will have the following properties: File == null and FileName == null
        ///     Precondition: source != null
        ///     Post-condition: Source.Equals(source) and File == null and FileName == null
        /// </summary>
        /// <param name="source">The source.</param>
        /// <exception cref="ArgumentNullException">source</exception>
        public ImageFileContainer(BitmapSource source)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.File = null;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates an ImageFileContainer asynchronously. This approach to construction is preferred.
        ///     This method will call the ImageFileContainer::LoadSource() method so that Source is equal to
        ///     the BitmapSource contained within the file.
        ///     Precondition: file != null
        ///     Post-condition: File.Equals(file) and FileName.Equals(file.Name) and Source != null
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>The newly constructed ImageFileContainer</returns>
        public static async Task<ImageFileContainer> CreateAsync(StorageFile file)
        {
            var container = new ImageFileContainer(file);
            await container.LoadSource();
            return container;
        }

        /// <summary>
        ///     Loads the Source property from the File property.
        ///     Precondition: File != null
        ///     Post-condition: Source != null
        /// </summary>
        /// <exception cref="ActionNotSupportedException">If File == null</exception>
        public async Task LoadSource()
        {
            if (this.File == null)
            {
                throw new ActionNotSupportedException();
            }

            this.Source = new BitmapImage();
            using (var randomAccessStream = await this.File.OpenAsync(FileAccessMode.Read))
            {
                await this.Source.SetSourceAsync(randomAccessStream);
            }
        }

        #endregion
    }
}