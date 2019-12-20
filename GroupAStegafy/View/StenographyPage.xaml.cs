using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GroupAStegafy.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StenographyPage
    {
        #region Data members

        private readonly double width = (double) Application.Current.Resources["StenographyPageWidth"];
        private readonly double height = (double) Application.Current.Resources["StenographyPageHeight"];

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StenographyPage" /> class.
        /// </summary>
        public StenographyPage()
        {
            this.InitializeComponent();
            this.subscribeToEvents();
            this.initializeAppSize();
        }

        #endregion

        #region Methods

        private void initializeAppSize()
        {
            ApplicationView.PreferredLaunchViewSize = new Size(this.width, this.height);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private void currentOnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.stenographyPage.Width = e.Size.Width;
            this.stenographyPage.Height = e.Size.Height;
        }

        private void subscribeToEvents()
        {
            Loaded += this.OnLoaded;
            Window.Current.CoreWindow.SizeChanged += this.currentOnSizeChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TryResizeView(ApplicationView.PreferredLaunchViewSize);
        }

        private void HideImageOrTextButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.hideImageOrTextButton.Opacity = (bool) e.NewValue ? 1.0 : 0.25;
        }

        private void LoadHiddenImageOrTextButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.loadHiddenImageOrTextButton.Opacity = (bool) e.NewValue ? 1.0 : 0.25;
        }

        private void SaveEmbededButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.saveEmbededButton.Opacity = (bool) e.NewValue ? 1.0 : 0.25;
        }

        private void MinusButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.minusButton.Opacity = (bool) e.NewValue ? 1.0 : 0.25;
        }

        private void PlusButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.plusButton.Opacity = (bool) e.NewValue ? 1.0 : 0.25;
        }

        #endregion
    }
}