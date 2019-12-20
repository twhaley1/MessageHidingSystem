using System;
using Windows.UI.Xaml.Data;

namespace GroupAStegafy.Model
{
    /// <summary>
    ///     An implementation of the IValueConverter interface that handles the conversion
    ///     of integers to strings.
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Data.IValueConverter" />
    public class IntegerStringConverter : IValueConverter
    {
        #region Methods

        /// <summary>
        ///     Converts the specified value into its string representation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The string representation of value.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        /// <summary>
        ///     Converts the value back to its integer representation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The value's integer representation.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var numberString = (string) value;
            return int.Parse(numberString);
        }

        #endregion
    }
}