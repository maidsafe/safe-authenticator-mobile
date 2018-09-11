using System.Windows.Input;
using JetBrains.Annotations;
using SafeAuthenticator.Models;
using Xamarin.Forms;

namespace SafeAuthenticator.ViewModels {
  internal class AppInfoViewModel : BaseViewModel {
    private RegisteredAppModel _appModelInfo;
    [PublicAPI]
    public RegisteredAppModel AppModelInfo { get => _appModelInfo; set => SetProperty(ref _appModelInfo, value); }
    [PublicAPI]
    public ICommand RevokeAppCommand { get; }

    public AppInfoViewModel(RegisteredAppModel appModelInfo) {
      AppModelInfo = appModelInfo;
      RevokeAppCommand = new Command(OnRevokeAppCommand);
    }

    private async void OnRevokeAppCommand() {
      await Application.Current.MainPage.DisplayAlert("Not Supported", "Not yet implemented.", "OK");
    }
  }
}
