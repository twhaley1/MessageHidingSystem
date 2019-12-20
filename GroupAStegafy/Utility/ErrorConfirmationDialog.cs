using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace GroupAStegafy.Utility
{
    /// <summary>
    ///     Provides the ability to generate a error confirmation dialog that
    ///     will display to the user.
    /// </summary>
    public static class ErrorConfirmationDialog
    {
        #region Methods

        /// <summary>
        ///     Shows the error confirmation dialog asynchronously.
        /// </summary>
        /// <param name="content">The content.</param>
        public static async Task ShowAsync(string content)
        {
            var dialog = new ContentDialog {
                Title = "ERROR",
                Content = content,
                CloseButtonText = "OK"
            };

            await dialog.ShowAsync();
        }

        #endregion
    }
}