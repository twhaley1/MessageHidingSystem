using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using GroupAStegafy.Extensions;
using GroupAStegafy.Model.DataContainers;
using GroupAStegafy.Model.MessageHiding;
using GroupAStegafy.Model.MessageHiding.Encryption;
using GroupAStegafy.Model.MessageHiding.Helpers;
using GroupAStegafy.Model.MessageHiding.Helpers.EventHelpers;
using GroupAStegafy.Properties;
using GroupAStegafy.Utility;

namespace GroupAStegafy.ViewModel
{
    /// <summary>
    ///     Binds the view to the model.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        #region Data members

        private readonly Settings settings;

        private StorageFile hiddenTextFile;
        private ImageFileContainer primaryContainer;
        private ImageFileContainer secondaryContainer;

        private ImageSource encryptedImage;
        private ImageSource nonEncryptedImage;

        private bool canSave;
        private bool isMessageHidingDisabled;
        private string secondaryContainerDescription;
        private string uploadedText;
        private int bitsPerColorChannel;

        private bool? usesEncryption;
        private string encryptionKey;
        private string encryptedText;
        private string nonEncryptedText;
        private string encryptedHeaderText;
        private string unencryptedHeaderText;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the load command.
        /// </summary>
        /// <value>
        ///     The load command.
        /// </value>
        public RelayCommand LoadPrimaryImageCommand { get; private set; }

        /// <summary>
        ///     Gets or sets the load hidden image or text command.
        /// </summary>
        /// <value>
        ///     The load hidden image or text command.
        /// </value>
        public RelayCommand LoadMessageCommand { get; private set; }

        /// <summary>
        ///     Gets or sets the hide message command.
        /// </summary>
        /// <value>
        ///     The hide message command.
        /// </value>
        public RelayCommand HideMessageCommand { get; private set; }

        /// <summary>
        ///     Gets or sets the save embed command.
        /// </summary>
        /// <value>
        ///     The save embed command.
        /// </value>
        public RelayCommand SaveCommand { get; private set; }

        /// <summary>
        ///     Gets or sets the minus bits per color channel command.
        /// </summary>
        /// <value>
        ///     The minus bits per color channel command.
        /// </value>
        public RelayCommand DecrementBitsPerColorChannelCommand { get; private set; }

        /// <summary>
        ///     Gets or sets the plus bits per color channel command.
        /// </summary>
        /// <value>
        ///     The plus bits per color channel command.
        /// </value>
        public RelayCommand IncrementBitsPerColorChannelCommand { get; private set; }

        /// <summary>
        ///     Gets or sets the primary container.
        ///     Post-condition: May be able to load or hide a message. This will
        ///     also result in a change in the value of PrimaryImageSource.
        /// </summary>
        /// <value>
        ///     The primary container.
        /// </value>
        public ImageFileContainer PrimaryContainer
        {
            get => this.primaryContainer;
            set
            {
                this.primaryContainer = value;
                this.OnPropertyChanged(nameof(this.PrimaryImageSource));
                this.LoadMessageCommand.OnCanExecuteChanged();
                this.HideMessageCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the secondary container.
        ///     Post-condition: May be able to hide a message. A change in this
        ///     value may also result in changes in SecondaryImageSource, EncryptionCheckShown,
        ///     and IsImageMessageShown
        /// </summary>
        /// <value>
        ///     The secondary container.
        /// </value>
        public ImageFileContainer SecondaryContainer
        {
            get => this.secondaryContainer;
            set
            {
                this.secondaryContainer = value;
                this.OnPropertyChanged(nameof(this.SecondaryImageSource));
                this.OnPropertyChanged(nameof(this.EncryptionCheckShown));
                this.OnPropertyChanged(nameof(this.IsImageMessageShown));
                this.HideMessageCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the encrypted image source.
        ///     Post-condition: A change in this value may result in a change in
        ///     IsEncryptionBlockShown and IsEncryptedImageShown.
        /// </summary>
        /// <value>
        ///     The encrypted image source.
        /// </value>
        public ImageSource EncryptedImageSource
        {
            get => this.encryptedImage;
            set
            {
                this.encryptedImage = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.IsEncryptionBlockShown));
                this.OnPropertyChanged(nameof(this.IsEncryptedImageShown));
            }
        }

        /// <summary>
        ///     Gets or sets the non encrypted image source.
        ///     Post-condition: A change in this value may result in a change in
        ///     IsEncryptionBlockShown and IsUnencryptedImageShown.
        /// </summary>
        /// <value>
        ///     The non encrypted image source.
        /// </value>
        public ImageSource NonEncryptedImageSource
        {
            get => this.nonEncryptedImage;
            set
            {
                this.nonEncryptedImage = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.IsEncryptionBlockShown));
                this.OnPropertyChanged(nameof(this.IsUnencryptedImageShown));
            }
        }

        /// <summary>
        ///     Gets or sets the encrypted text.
        ///     Post-condition: A change in this value may result in a change
        ///     in IsEncryptionBlockShown and IsEncryptedTextShown.
        /// </summary>
        /// <value>
        ///     The encrypted text.
        /// </value>
        public string EncryptedText
        {
            get => this.encryptedText;
            set
            {
                this.encryptedText = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.IsEncryptionBlockShown));
                this.OnPropertyChanged(nameof(this.IsEncryptedTextShown));
            }
        }

        /// <summary>
        ///     Gets or sets the non encrypted text.
        ///     Post-condition: A change in this value may result in a change
        ///     in IsEncryptionBlockShown and IsUnencryptedTextShown.
        /// </summary>
        /// <value>
        ///     The non encrypted text.
        /// </value>
        public string NonEncryptedText
        {
            get => this.nonEncryptedText;
            set
            {
                this.nonEncryptedText = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.IsEncryptionBlockShown));
                this.OnPropertyChanged(nameof(this.IsUnencryptedTextShown));
            }
        }

        /// <summary>
        ///     Gets the primary image source.
        ///     Post-condition: This value will be null if PrimaryContainer == null.
        /// </summary>
        /// <value>
        ///     The primary image source.
        /// </value>
        public ImageSource PrimaryImageSource => this.PrimaryContainer?.Source;

        /// <summary>
        ///     Gets the secondary image source.
        ///     Post-condition: This value will be null if SecondaryContainer == null.
        /// </summary>
        /// <value>
        ///     The secondary image source.
        /// </value>
        public ImageSource SecondaryImageSource => this.SecondaryContainer?.Source;

        /// <summary>
        ///     Gets or sets the hidden text.
        ///     Post-condition: May be able to hide a message. A change in this value
        ///     may result in a change in KeyShown, EncryptionCheckShown, IsTextMessageShown,
        ///     and IsBitsPerColorChannelBlockShown.
        /// </summary>
        /// <value>
        ///     The user uploaded text.
        /// </value>
        public string UploadedText
        {
            get => this.uploadedText;
            set
            {
                this.uploadedText = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.KeyShown));
                this.OnPropertyChanged(nameof(this.EncryptionCheckShown));
                this.OnPropertyChanged(nameof(this.IsTextMessageShown));
                this.OnPropertyChanged(nameof(this.IsBitsPerColorChannelBlockShown));
                this.HideMessageCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the bits per color channel.
        ///     Post-condition: May be able to increment or decrement the
        ///     bits per color channel value.
        /// </summary>
        /// <value>
        ///     The bits per color channel text.
        /// </value>
        public int BitsPerColorChannel
        {
            get => this.bitsPerColorChannel;
            set
            {
                this.bitsPerColorChannel = value;
                this.OnPropertyChanged();
                this.IncrementBitsPerColorChannelCommand.OnCanExecuteChanged();
                this.DecrementBitsPerColorChannelCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the description for what resides in the SecondaryContainer.
        /// </summary>
        /// <value>
        ///     The description for the SecondaryContainer.
        /// </value>
        public string SecondaryContainerDescription
        {
            get => this.secondaryContainerDescription;
            set
            {
                this.secondaryContainerDescription = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the user wants encryption to be used.
        ///     Post-condition: if value == null or value == false, then EncryptionKey == null. Also,
        ///     a message may be able to be hidden. A change in this value may result in a change in
        ///     KeyShown.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the user wants encryption to be used; otherwise, <c>false</c>.
        /// </value>
        public bool? UsesEncryption
        {
            get => this.usesEncryption;
            set
            {
                if (!(value ?? false))
                {
                    this.EncryptionKey = null;
                }

                this.usesEncryption = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.KeyShown));
                this.HideMessageCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets the encryption key.
        ///     Post-condition: A message may be able to be hidden.
        /// </summary>
        /// <value>
        ///     The encryption key.
        /// </value>
        public string EncryptionKey
        {
            get => this.encryptionKey;
            set
            {
                this.encryptionKey = value;
                this.HideMessageCommand.OnCanExecuteChanged();
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the encrypted display's header text.
        /// </summary>
        /// <value>
        ///     The encrypted header text.
        /// </value>
        public string EncryptedHeaderText
        {
            get => this.encryptedHeaderText;
            set
            {
                this.encryptedHeaderText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the nonencrypted display's header text.
        /// </summary>
        /// <value>
        ///     The nonencrypted header text.
        /// </value>
        public string UnencryptedHeaderText
        {
            get => this.unencryptedHeaderText;
            set
            {
                this.unencryptedHeaderText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the textbox to enter a key is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if text field to enter key is visible; otherwise, <c>false</c>.
        /// </value>
        public bool KeyShown => !string.IsNullOrEmpty(this.UploadedText) && (this.UsesEncryption ?? false);

        /// <summary>
        ///     Gets a value indicating whether encryption checkbox is shown.
        /// </summary>
        /// <value>
        ///     <c>true</c> if encryption checkbox is shown; otherwise, <c>false</c>.
        /// </value>
        public bool EncryptionCheckShown =>
            (this.SecondaryImageSource != null || !string.IsNullOrEmpty(this.UploadedText)) &&
            !this.IsMessageHidingDisabled;

        /// <summary>
        ///     Gets a value indicating whether the user has uploaded an image that has been
        ///     placed in SecondaryContainer.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the user has uploaded an image that is in SecondaryContainer; otherwise, <c>false</c>.
        /// </value>
        public bool IsImageMessageShown => this.SecondaryContainer != null;

        /// <summary>
        ///     Gets a value indicating whether the user has uploaded a text file in which the
        ///     text resides in UploadedText.
        /// </summary>
        /// <value>
        ///     <c>true</c> if UploadedText contains text contents; otherwise, <c>false</c>.
        /// </value>
        public bool IsTextMessageShown => this.UploadedText != null;

        /// <summary>
        ///     Gets a value indicating whether the encryption block is shown. The encryption block
        ///     is the parent container that contains areas for the results of an encryption/decryption.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the encryption block is shown; otherwise, <c>false</c>.
        /// </value>
        public bool IsEncryptionBlockShown => this.EncryptedText != null || this.NonEncryptedText != null ||
                                              this.EncryptedImageSource != null || this.NonEncryptedImageSource != null;

        /// <summary>
        ///     Gets a value indicating if an encrypted image is currently being displayed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if an encrypted image is currently being shown; otherwise, <c>false</c>.
        /// </value>
        public bool IsEncryptedImageShown => this.EncryptedImageSource != null;

        /// <summary>
        ///     Gets a value indicating if an unencrypted image is currently being displayed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if an unencrypted image is currently being shown; otherwise, <c>false</c>.
        /// </value>
        public bool IsUnencryptedImageShown => this.NonEncryptedImageSource != null;

        /// <summary>
        ///     Gets a value indicating if encrypted text is currently being displayed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if encrypted text is currently being displayed; otherwise, <c>false</c>.
        /// </value>
        public bool IsEncryptedTextShown => this.EncryptedText != null;

        /// <summary>
        ///     Gets a value indicating if unencrypted text is currently being displayed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if unencrypted text is currently being displayed; otherwise, <c>false</c>.
        /// </value>
        public bool IsUnencryptedTextShown => this.NonEncryptedText != null;

        /// <summary>
        ///     Gets a value indicating if the bits per color channel block is being shown. The bits per
        ///     color channel block is the parent container of the UI components that allow the user to change and
        ///     see the bits per color channel.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the bits per color channel block is shown; otherwise, <c>false</c>.
        /// </value>
        public bool IsBitsPerColorChannelBlockShown => this.UploadedText != null && !this.IsMessageHidingDisabled;

        private bool IsMessageHidingDisabled
        {
            get => this.isMessageHidingDisabled;
            set
            {
                this.isMessageHidingDisabled = value;
                this.HideMessageCommand.OnCanExecuteChanged();
                this.OnPropertyChanged(nameof(this.EncryptionCheckShown));
                this.OnPropertyChanged(nameof(this.IsBitsPerColorChannelBlockShown));
            }
        }

        private bool CanSave
        {
            get => this.canSave;
            set
            {
                this.canSave = value;
                this.SaveCommand.OnCanExecuteChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewModel" /> class.
        ///     Post-condition: BitsPerColorChannel == Settings.MinimumBitsPerColorChannel and
        ///     SecondaryContainerDescription = ViewModelTextConstants.UPLOAD_DATA_TO_ENCODE and
        ///     EncryptionKey.Equals(string.Empty) and UsesEncryption == false and
        ///     IsMessageHidingDisabled == true.
        /// </summary>
        public ViewModel()
        {
            this.initializeCommands();
            this.BitsPerColorChannel = Settings.MinimumBitsPerColorChannel;
            this.SecondaryContainerDescription = ViewModelTextConstants.UPLOAD_DATA_TO_ENCODE;
            this.EncryptionKey = string.Empty;

            this.settings = new Settings();
            this.UsesEncryption = false;
            this.IsMessageHidingDisabled = true;
        }

        #endregion

        #region Methods

        private void onImageDecodingComplete(object sender, EventArgs e)
        {
            var args = (ImageDecodingEventArgs) e;
            this.SecondaryContainer = new ImageFileContainer(args.DecodedImage);
            this.UploadedText = null;

            this.EncryptedHeaderText = ViewModelTextConstants.RAW_TEXT;
            this.EncryptedImageSource = args.RawImage;
        }

        private void onTextDecodingComplete(object sender, EventArgs e)
        {
            var args = (TextDecodingEventArgs) e;
            this.UploadedText = args.DecodedText;
            this.SecondaryContainer = null;

            this.EncryptedHeaderText = ViewModelTextConstants.RAW_TEXT;
            this.UnencryptedHeaderText = ViewModelTextConstants.KEYWORD;

            this.EncryptedText = args.RawText;
            this.NonEncryptedText = args.Keyword;
        }

        private async Task decodeHiddenMessage(ImageData primaryData, Header header)
        {
            var descriptor = header.HiddenMessageDescriber;
            var usingEncryption = descriptor.R.GetBit(0);
            if (descriptor.B.GetBit(0))
            {
                var bitsPerColorChannelValue = (int) descriptor.G;
                var decoder = usingEncryption
                    ? new EncryptedTextDecoder(primaryData, bitsPerColorChannelValue, new TextEncrypter())
                    : new TextDecoder(primaryData, bitsPerColorChannelValue);
                decoder.DecodingComplete += this.onTextDecodingComplete;

                await decoder.DecodeMessage();
            }
            else
            {
                var decoder = usingEncryption
                    ? new EncryptedImageDecoder(primaryData, new ImageEncrypter {Height = (int) primaryData.Height})
                    : new ImageDecoder(primaryData);
                decoder.DecodingComplete += this.onImageDecodingComplete;

                await decoder.DecodeMessage();
            }

            this.SecondaryContainerDescription = ViewModelTextConstants.DECODED_RESULTS;
        }

        private async Task hideImage()
        {
            var primaryData = new ImageData(this.PrimaryContainer.File);
            var secondaryData = new ImageData(this.SecondaryContainer.File);

            try
            {
                var encoder = this.UsesEncryption ?? false
                    ? await EncryptedImageEncoder.CreateAsync(primaryData, secondaryData,
                        new ImageEncrypter())
                    : await ImageEncoder.CreateAsync(primaryData, secondaryData);

                encoder.EncodingComplete += this.onEncodingImageComplete;
                await encoder.EncodeMessage();
            }
            catch (InvalidOperationException)
            {
                await ErrorConfirmationDialog.ShowAsync(
                    ViewModelTextConstants.FILE_TOO_WIDE_MESSAGE);
            }
        }

        private void onEncodingImageComplete(object sender, EventArgs e)
        {
            var args = (ImageEncodingEventArgs) e;

            this.EncryptedImageSource = args.EncryptedMessage;
            this.NonEncryptedImageSource = args.NonEncryptedMessage;

            this.SecondaryContainer = new ImageFileContainer(args.EmbedImage);
        }

        private async Task hideText()
        {
            var primaryData = new ImageData(this.PrimaryContainer.File);

            TextEncoder encoder = null;
            try
            {
                encoder = this.UsesEncryption ?? false
                    ? await EncryptedTextEncoder.CreateAsync(primaryData, this.UploadedText, this.BitsPerColorChannel,
                        new TextEncrypter {Keyword = this.EncryptionKey})
                    : await TextEncoder.CreateAsync(primaryData, this.UploadedText, this.BitsPerColorChannel);

                encoder.EncodingComplete += this.onEncodingTextComplete;
                await encoder.EncodeMessage();
            }
            catch (InvalidOperationException)
            {
                var bitsPerColorChannelMessage =
                    encoder?.SuggestedBitsPerColorChannel > Settings.MaximumBitsPerColorChannel
                        ? ViewModelTextConstants.NO_POSSIBLE_BPCC
                        : ViewModelTextConstants.BPCC_TOO_LOW_SUGGESTED + encoder?.SuggestedBitsPerColorChannel;

                await ErrorConfirmationDialog.ShowAsync(bitsPerColorChannelMessage);
            }
        }

        private void onEncodingTextComplete(object sender, EventArgs e)
        {
            var args = (TextEncodingEventArgs) e;

            this.EncryptedText = args.EncryptedMessage;
            this.NonEncryptedText = args.NonEncryptedMessage;

            this.SecondaryContainer = new ImageFileContainer(args.EmbedImage);
        }

        private void initializeCommands()
        {
            this.LoadPrimaryImageCommand = new RelayCommand(this.loadPrimaryImage, obj => true);
            this.LoadMessageCommand =
                new RelayCommand(this.loadHiddenImageOrText, this.canLoadHiddenImageOrText);
            this.HideMessageCommand = new RelayCommand(this.hideMessage, this.canHideMessage);

            this.SaveCommand = new RelayCommand(this.saveEmbedImage, obj => this.CanSave);
            this.DecrementBitsPerColorChannelCommand = new RelayCommand(obj => this.BitsPerColorChannel--,
                obj => this.BitsPerColorChannel > Settings.MinimumBitsPerColorChannel);
            this.IncrementBitsPerColorChannelCommand = new RelayCommand(obj => this.BitsPerColorChannel++,
                obj => this.BitsPerColorChannel < Settings.MaximumBitsPerColorChannel);
        }

        private void putUiStartState()
        {
            this.BitsPerColorChannel = Settings.MinimumBitsPerColorChannel;
            this.SecondaryContainerDescription = ViewModelTextConstants.UPLOAD_DATA_TO_ENCODE;
            this.UsesEncryption = null;
            this.EncryptionKey = null;
            this.hiddenTextFile = null;
            this.PrimaryContainer = null;
            this.SecondaryContainer = null;
            this.UploadedText = null;
            this.EncryptedImageSource = null;
            this.NonEncryptedImageSource = null;
            this.EncryptedText = null;
            this.NonEncryptedText = null;
            this.CanSave = false;
            this.IsMessageHidingDisabled = false;
            this.EncryptedHeaderText = null;
            this.UnencryptedHeaderText = null;
        }

        #endregion

        #region Command Handlers

        private async void saveEmbedImage(object obj)
        {
            if (this.UploadedText != null)
            {
                await UserInput.SaveText(this.UploadedText);
            }
            else
            {
                using (var fileStream = await this.PrimaryContainer.File.OpenAsync(FileAccessMode.Read))
                {
                    var bitmapDecoder = await BitmapDecoder.CreateAsync(fileStream);
                    await UserInput.SaveBitmap(this.SecondaryImageSource as WriteableBitmap, bitmapDecoder.DpiX,
                        bitmapDecoder.DpiY);
                }
            }

            this.putUiStartState();
        }

        private async void loadPrimaryImage(object obj)
        {
            this.putUiStartState();

            var selectedFile = await UserInput.SelectFile(this.settings.ValidImageExtensions);
            if (selectedFile != null)
            {
                var primaryData = new ImageData(selectedFile);
                var header = await Header.CreateAsync(primaryData);

                if (header.HiddenMessageIdentifier.Equals(this.settings.Identifier))
                {
                    await this.decodeHiddenMessage(primaryData, header);
                    this.CanSave = true;
                    this.IsMessageHidingDisabled = true;
                }

                this.PrimaryContainer = await ImageFileContainer.CreateAsync(selectedFile);
            }
            else
            {
                await ErrorConfirmationDialog.ShowAsync(ViewModelTextConstants.FILE_WAS_NOT_LOADED);
            }
        }

        private async void loadHiddenImageOrText(object obj)
        {
            var file = await UserInput.SelectFile(this.settings.AllValidExtensions);
            if (file != null)
            {
                var currentPrimaryContainer = this.PrimaryContainer;
                this.putUiStartState();
                this.PrimaryContainer = currentPrimaryContainer;

                if (file.HasTargetExtension(this.settings.ValidImageExtensions))
                {
                    this.hiddenTextFile = null;
                    this.SecondaryContainer = await ImageFileContainer.CreateAsync(file);
                }
                else if (file.HasTargetExtension(this.settings.ValidTextExtensions))
                {
                    this.UploadedText = await FileAccessor.LoadText(file);
                    this.hiddenTextFile = file;
                    this.SecondaryContainer = null;
                }
                else
                {
                    await ErrorConfirmationDialog.ShowAsync(ViewModelTextConstants.FILE_WAS_NOT_LOADED);
                    return;
                }

                this.IsMessageHidingDisabled = false;
                this.SecondaryContainerDescription = ViewModelTextConstants.HIDE_IN_PRIMARY_FILE;
            }
            else
            {
                await ErrorConfirmationDialog.ShowAsync(ViewModelTextConstants.FILE_WAS_NOT_LOADED);
            }
        }

        private async void hideMessage(object obj)
        {
            if (this.hiddenTextFile != null)
            {
                await this.hideText();
            }
            else
            {
                await this.hideImage();
            }

            if (this.UsesEncryption ?? false)
            {
                this.EncryptedHeaderText = ViewModelTextConstants.ENCRYPTED_INFORMATION;
                this.UnencryptedHeaderText = ViewModelTextConstants.RAW_INFORMATION;
            }

            this.IsMessageHidingDisabled = true;
            this.CanSave = true;
            this.UsesEncryption = null;
            this.UploadedText = null;
            this.SecondaryContainerDescription = ViewModelTextConstants.MESSAGE_HIDDEN_IN_IMAGE_BELOW;
        }

        private bool canHideMessage(object obj)
        {
            var hasProperEncryptionFieldsFilled = true;
            if (!string.IsNullOrEmpty(this.UploadedText) && (this.UsesEncryption ?? false))
            {
                hasProperEncryptionFieldsFilled = !string.IsNullOrEmpty(this.EncryptionKey);
            }

            return this.PrimaryImageSource != null &&
                   (this.SecondaryImageSource != null || this.UploadedText != null) &&
                   hasProperEncryptionFieldsFilled && !this.IsMessageHidingDisabled;
        }

        private bool canLoadHiddenImageOrText(object obj)
        {
            return this.PrimaryImageSource != null;
        }

        #endregion

        #region Events and Invocations

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        /// <returns>A PropertyChangedEventHandler</returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}