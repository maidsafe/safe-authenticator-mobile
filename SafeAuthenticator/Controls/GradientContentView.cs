using Xamarin.Forms;

namespace SafeAuthenticator.Controls
{
    public enum GradientOrientation
    {
        Vertical,
        Horizontal
    }

    public class GradientContentView : ContentView
    {
        public GradientOrientation Orientation
        {
            get => (GradientOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public static readonly BindableProperty OrientationProperty = BindableProperty.Create(
                nameof(Orientation),
                typeof(GradientOrientation),
                typeof(GradientContentView),
                GradientOrientation.Vertical);

        public Color StartColor
        {
            get => (Color)GetValue(StartColorProperty);
            set => SetValue(StartColorProperty, value);
        }

        public static readonly BindableProperty StartColorProperty = BindableProperty.Create(
            nameof(EndColor),
            typeof(Color),
            typeof(GradientContentView),
            Color.Default);

        public Color EndColor
        {
            get => (Color)GetValue(EndColorProperty);
            set => SetValue(EndColorProperty, value);
        }

        public static readonly BindableProperty EndColorProperty = BindableProperty.Create(
            nameof(EndColor),
            typeof(Color),
            typeof(GradientContentView),
            Color.Default);
    }
}
