using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace SafeAuthenticator.Controls.Converters
{
    class FormatTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new InvalidOperationException("The target must be a string");
            }
            var text = value.ToString();
            var result = string.Empty;
            text.Split(' ').ToList().ForEach(i => result += i[0]);
            return result.Length < 2 ? result.Substring(0, 1).ToUpper() : result.Substring(0, 2).ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
