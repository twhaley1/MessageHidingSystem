using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GroupAStegafy.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        #region Data members

        private double width = (double) Application.Current.Resources["StartPageWidth"];
        private double height = (double) Application.Current.Resources["StartPageHeight"];

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.initializeAppSize();
        }

        #endregion

        #region Methods

        private void initializeAppSize()
        {
            ApplicationView.PreferredLaunchViewSize = new Size(this.width, this.height);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            Window.Current.CoreWindow.SizeChanged += this.currentOnSizeChanged;
        }

        private void currentOnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.width = e.Size.Width;
            this.height = e.Size.Height;
            this.mainPage.Width = e.Size.Width;
            this.mainPage.Height = e.Size.Height;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StenographyPage));
        }

        #endregion
    }
}