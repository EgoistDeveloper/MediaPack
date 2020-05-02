using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaPack.UI.Converters
{
    public class ByteFormatterConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object format, CultureInfo culture)
        {
            const double minKiloByte = 1024;
            const double minMegaByte = 1024 * 1024;
            const double minGigaByte = 1024 * 1024 * 1024;

            var byteCount = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);

            var suffix = "b";
            var output = 0d;

            if (byteCount >= minKiloByte)
            {
                suffix = "kB";
                output = Math.Round(byteCount / minKiloByte, 2);
            }

            if (byteCount >= minMegaByte)
            {
                suffix = "MB";
                output = Math.Round(byteCount / minMegaByte, 2);
            }

            if (byteCount >= minGigaByte)
            {
                suffix = "GB";
                output = Math.Round(byteCount / minGigaByte, 2);
            }

            return suffix == "b" ?
                $"{output:0} {suffix}" :
                $"{output:0.00} {suffix}";
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
