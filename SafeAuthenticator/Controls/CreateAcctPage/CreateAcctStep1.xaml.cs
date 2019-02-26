using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamEffects;

namespace SafeAuthenticator.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateAcctStep1 : StackLayout
    {
        public CreateAcctStep1()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.Android)
            {
                TouchEffect.SetColor(PasteIconStackLayout, Color.Gray);
            }

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (sender, e) =>
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    await ((Image)sender).FadeTo(0, 50);
                    await ((Image)sender).FadeTo(1, 500);
                }

                ((CreateAcctViewModel)BindingContext).ClipboardPasteCommand.Execute(null);
            };
            PasteIcon.GestureRecognizers.Add(tapGestureRecognizer);
        }
    }
}
