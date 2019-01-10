using System;
using System.Globalization;
using Xamarin.Forms;

namespace SafeAuthenticator.Controls.Converters
{
    internal class ContainerNameToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value.ToString();
            switch (text)
            {
                case "Documents":
                    return "documents";
                case "Downloads":
                    return "downloads";
                case "Music":
                    return "music";
                case "Pictures":
                    return "pictures";
                case "Videos":
                    return "videos";
                case "Public":
                    return "publicContainer";
                case "Public Names":
                    return "publicNames";
                case "App's own Container":
                    return "appcontainer";
                default:
                    throw new Exception();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
