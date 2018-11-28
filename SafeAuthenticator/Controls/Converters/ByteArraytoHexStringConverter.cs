using System;
using System.Globalization;
using Xamarin.Forms;

namespace SafeAuthenticator.Controls.Converters
{
    public class ByteArraytoHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BitConverter.ToString((byte[])value).Replace("-", string.Empty).Substring(0, 6);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
