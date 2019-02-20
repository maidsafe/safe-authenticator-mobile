using Xamarin.Forms;

namespace SafeAuthenticator.Controls
{
    public class CustomScrollView : ScrollView
    {
        public static readonly BindableProperty IsScrollEnabledProperty = BindableProperty.Create(
            nameof(IsScrollEnabled),
            typeof(bool),
            typeof(CustomScrollView),
            default(bool));

        /// <summary>
        /// This property can be used to enabled/disable the scroll
        /// </summary>
        public bool IsScrollEnabled
        {
            get { return (bool)GetValue(IsScrollEnabledProperty); }
            set { SetValue(IsScrollEnabledProperty, value); }
        }
    }
}
