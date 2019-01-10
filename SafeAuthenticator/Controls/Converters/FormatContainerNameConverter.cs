using System;
using System.Globalization;
using Xamarin.Forms;

namespace SafeAuthenticator.Controls.Converters
{
    internal class FormatContainerNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value.ToString();
            switch (text)
            {
                case "App Container":
                    return "App's own container";
                case "_publicNames":
                    return "Public Names";
                default:
                    return $"{text.Substring(1, 1).ToUpper()}{text.Substring(2)}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
