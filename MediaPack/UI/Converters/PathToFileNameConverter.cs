using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace MediaPack.UI.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class PathToFileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is string val)) return null;

            return Path.GetFileName(val);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}