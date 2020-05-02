using System;
using System.Globalization;
using System.Windows.Data;
namespace MediaPack.UI.Converters
{
    public class PercentageFormatterConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object format, CultureInfo culture)
        {
            var percentage = 0d;
            if (value is double d) percentage = d;

            percentage = Math.Round(percentage * 100d, 0);

            if (format == null || Math.Abs(percentage) <= double.Epsilon)
                return $"{percentage,3:0} %".Trim();

            return $"{(percentage > 0d ? "R " : "L ")} {Math.Abs(percentage),3:0} %".Trim();
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
