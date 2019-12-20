using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using GroupAStegafy.Model.FileDescriptions;

namespace GroupAStegafy.Utility
{
    /// <summary>
    ///     Holds IO Utility methods that involve the user's control.
    /// </summary>
    public static class UserInput
    {
        #region Methods

        /// <summary>
        ///     Prompts the user to select a file with the operating system's file selection dialog.
        ///     Only files with the specified FileExtensions will be shown in the dialog.
        /// </summary>
        /// <param name="extensions">The extensions.</param>
        /// <returns>The file that the user selects.</returns>
        public static async Task<StorageFile> SelectFile(IEnumerable<FileExtension> extensions)
        {
            var openPicker = new FileOpenPicker {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            foreach (var extension in extensions ?? Enumerable.Empty<FileExtension>())
            {
                var extensionString = extension.Extension;
                if (Regex.IsMatch(extensionString, Settings.ExtensionRegex))
                {
                    openPicker.FileTypeFilter.Add(extensionString);
                }
            }

            return await openPicker.PickSingleFileAsync();
        }

        /// <summary>
        ///     Saves the specified bitmap in a file selected by the user.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="dpiX">The dpi x.</param>
        /// <param name="dpiY">The dpi y.</param>
        public static async Task SaveBitmap(WriteableBitmap bitmap, double dpiX, double dpiY)
        {
            var fileSavePicker = new FileSavePicker {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };
            var settings = new Settings();
            fileSavePicker.FileTypeChoices.Add("File Export Files",
                settings.ValidExportImageExtensions.Select(fileExtension => fileExtension.Extension)
                        .ToList());
            var saveFile = await fileSavePicker.PickSaveFileAsync();

            if (saveFile != null)
            {
                var stream = await saveFile.OpenAsync(FileAccessMode.ReadWrite);
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                var pixelStream = bitmap.PixelBuffer.AsStream();
                var pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                    (uint) bitmap.PixelWidth,
                    (uint) bitmap.PixelHeight, dpiX, dpiY, pixels);
                await encoder.FlushAsync();

                stream.Dispose();
            }
        }

        /// <summary>
        ///     Saves the specified text in a file selected by the user.
        /// </summary>
        /// <param name="text">The text.</param>
        public static async Task SaveText(string text)
        {
            var fileSavePicker = new FileSavePicker {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "text"
            };

            var settings = new Settings();
            fileSavePicker.FileTypeChoices.Add("TXT Files",
                settings.ValidTextExtensions.Select(fileExtension => fileExtension.Extension).ToList());
            var saveFile = await fileSavePicker.PickSaveFileAsync();

            if (saveFile != null)
            {
                var stream = await saveFile.OpenStreamForWriteAsync();
                var linesOfText = text.Split(Environment.NewLine);
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var line in linesOfText)
                    {
                        await writer.WriteLineAsync(line);
                    }
                }
            }
        }

        #endregion
    }
}