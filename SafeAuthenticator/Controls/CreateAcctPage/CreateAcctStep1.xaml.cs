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
                PasteIconStackLayout.SetBinding(Commands.TapProperty, "ClipboardPasteCommand");
            }

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (sender, e) =>
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    await ((Image)sender).FadeTo(0, 50);
                    ((CreateAcctViewModel)BindingContext).ClipboardPasteCommand.Execute(null);
                    await ((Image)sender).FadeTo(1, 500);
                }
            };
            PasteIcon.GestureRecognizers.Add(tapGestureRecognizer);
        }
    }
}
