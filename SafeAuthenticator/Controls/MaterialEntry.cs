using System;
using SafeAuthenticator.Controls.Effects;
using Xamarin.Forms;

namespace SafeAuthenticator.Controls
{
    public class MaterialEntry : Entry
    {
        public MaterialEntry()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                FloatingHintEnabled = false;
            }
        }

        protected override void OnPropertyChanging(string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);

            if (propertyName == IsPasswordProperty.PropertyName && Device.RuntimePlatform == Device.iOS)
            {
                Effects.Add(new ShowHidePasswordEffect());
            }
        }

        public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(
            nameof(ErrorText),
            typeof(string),
            typeof(MaterialEntry),
            default(string),
            propertyChanged: OnErrorTextChangedInternal);

        public static readonly BindableProperty FloatingHintEnabledProperty = BindableProperty.Create(
            nameof(FloatingHintEnabled),
            typeof(bool),
            typeof(MaterialEntry),
            true);

        public static readonly BindableProperty ActivePlaceholderColorProperty = BindableProperty.Create(
            nameof(ActivePlaceholderColor),
            typeof(Color),
            typeof(MaterialEntry),
            Color.Accent);

        /// <summary>
        /// ActivePlaceholderColor summary.
        /// </summary>
        public Color ActivePlaceholderColor
        {
            get => (Color)GetValue(ActivePlaceholderColorProperty);
            set => SetValue(ActivePlaceholderColorProperty, value);
        }

        /// <summary>
        /// <c>true</c> to float the hint into a label, otherwise <c>false</c>.
        /// </summary>
        public bool FloatingHintEnabled
        {
            get => (bool)GetValue(FloatingHintEnabledProperty);
            set => SetValue(FloatingHintEnabledProperty, value);
        }

        /// <summary>
        /// Gets or Sets whether or not the Error Style is 'Underline' or 'None'
        /// </summary>
        public ErrorDisplay ErrorDisplay { get; set; } = ErrorDisplay.Underline;

        /// <summary>
        /// Error text for the entry. An empty string removes the error.
        /// </summary>
        public string ErrorText
        {
            get => (string)GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }

        /// <summary>
        /// Raised when the value of the error text changes
        /// </summary>
        public event EventHandler<TextChangedEventArgs> ErrorTextChanged;

        private static void OnErrorTextChangedInternal(BindableObject bindableObject, object oldValue, object newValue)
        {
            var materialEntry = (MaterialEntry)bindableObject;
            materialEntry.OnErrorTextChanged(bindableObject, oldValue, newValue);
            materialEntry.ErrorTextChanged?.Invoke(materialEntry, new TextChangedEventArgs((string)oldValue, (string)newValue));
        }

        protected virtual void OnErrorTextChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
        }
    }

    public enum ErrorDisplay
    {
        Underline,
        None
    }
}
