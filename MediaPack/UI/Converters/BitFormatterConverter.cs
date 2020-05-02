using System;
using System.Globalization;
using System.Windows.Data;

namespace MediaPack.UI.Converters
{
    public class BitFormatterConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object format, CultureInfo culture)
        {
            const double minKiloBit = 1000;
            const double minMegaBit = 1000 * 1000;
            const double minGigaBit = 1000 * 1000 * 1000;

            var byteCount = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);

            var suffix = "bits/s";
            var output = 0d;

            if (byteCount >= minKiloBit)
            {
                suffix = "kbits/s";
                output = Math.Round(byteCount / minKiloBit, 2);
            }

            if (byteCount >= minMegaBit)
            {
                suffix = "Mbits/s";
                output = Math.Round(byteCount / minMegaBit, 2);
            }

            if (byteCount >= minGigaBit)
            {
                suffix = "Gbits/s";
                output = Math.Round(byteCount / minGigaBit, 2);
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
