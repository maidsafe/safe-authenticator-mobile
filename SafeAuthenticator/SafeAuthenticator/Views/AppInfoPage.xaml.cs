using CommonUtils;
using SafeAuthenticator.Models;
using SafeAuthenticator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SafeAuthenticator.Views {
  [XamlCompilation(XamlCompilationOptions.Compile)]
  // ReSharper disable once MemberCanBeInternal
  public partial class AppInfoPage : ContentPage, ICleanup {
    // ReSharper disable once MemberCanBeInternal
    public AppInfoPage() : this(null) { }

    // ReSharper disable once MemberCanBeInternal
    public AppInfoPage(RegisteredAppModel appModelInfo) {
      InitializeComponent();
      BindingContext = new AppInfoViewModel(appModelInfo);
    }

    public void MessageCenterUnsubscribe() { }
  }
}
