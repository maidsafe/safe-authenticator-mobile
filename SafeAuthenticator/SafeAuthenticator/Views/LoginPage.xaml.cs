using System.Diagnostics;
using CommonUtils;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class LoginPage : ContentPage, ICleanup {
    public LoginPage() {
      InitializeComponent();

      MessagingCenter.Subscribe<LoginViewModel>(
        this,
        MessengerConstants.NavHomePage,
        async sender => {
          MessageCenterUnsubscribe();
          if (!App.IsPageValid(this)) {
            return;
          }
          Debug.WriteLine("LoginPage -> HomePage");

          Navigation.InsertPageBefore(new HomePage(), this);
          await Navigation.PopAsync();
        });

      MessagingCenter.Subscribe<LoginViewModel>(
        this,
        MessengerConstants.NavCreateAcctPage,
        async _ => {
          if (!App.IsPageValid(this)) {
            MessageCenterUnsubscribe();
            return;
          }
          Debug.WriteLine("LoginPage -> CreateAcctPage");
          await Navigation.PushAsync(new CreateAcctPage());
        });
    }

    public void MessageCenterUnsubscribe() {
      MessagingCenter.Unsubscribe<LoginViewModel>(this, MessengerConstants.NavHomePage);
      MessagingCenter.Unsubscribe<LoginViewModel>(this, MessengerConstants.NavCreateAcctPage);
    }
  }
}
