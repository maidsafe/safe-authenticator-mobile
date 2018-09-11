using System.Diagnostics;
using CommonUtils;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class HomePage : ContentPage, ICleanup {
    public HomePage() {
      InitializeComponent();
      MessagingCenter.Subscribe<HomeViewModel>(
        this,
        MessengerConstants.NavLoginPage,
        async _ => {
          MessageCenterUnsubscribe();
          if (!App.IsPageValid(this)) {
            return;
          }
          Debug.WriteLine("HomePage -> LoginPage");
          Navigation.InsertPageBefore(new LoginPage(), this);
          await Navigation.PopAsync();
        });
      MessagingCenter.Subscribe<HomeViewModel, RegisteredAppModel>(
        this,
        MessengerConstants.NavAppInfoPage,
        async (_, appInfo) => {
          if (!App.IsPageValid(this)) {
            MessageCenterUnsubscribe();
            return;
          }
          await Navigation.PushAsync(new AppInfoPage(appInfo));
          AccountsView.SelectedItem = null;
        });
    }

    public void MessageCenterUnsubscribe() {
      MessagingCenter.Unsubscribe<HomeViewModel>(this, MessengerConstants.NavLoginPage);
      MessagingCenter.Unsubscribe<HomeViewModel, RegisteredAppModel>(this, MessengerConstants.NavAppInfoPage);
    }
  }
}
