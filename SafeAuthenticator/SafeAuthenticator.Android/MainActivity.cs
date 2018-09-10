using System;
using System.IO;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace SafeAuthenticator.Droid {
  [Activity(
     Label = "@string/app_name",
     Theme = "@style/MyTheme",
     MainLauncher = true,
     LaunchMode = LaunchMode.SingleTask,
     ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation),
   IntentFilter(new[] {Intent.ActionView}, Categories = new[] {Intent.CategoryDefault, Intent.CategoryBrowsable}, DataScheme = "safe-auth")]
  // ReSharper disable once UnusedMember.Global
  public class MainActivity : FormsAppCompatActivity {
    private AuthService Authenticator => DependencyService.Get<AuthService>();
    private static string LogFolderPath => DependencyService.Get<IFileOps>().ConfigFilesPath;

    private void HandleAppLaunch(string uri) {
      System.Diagnostics.Debug.WriteLine($"Launched via: {uri}");
      Device.BeginInvokeOnMainThread(
        async () => {
          try {
            await Authenticator.HandleUrlActivationAsync(uri);
            System.Diagnostics.Debug.WriteLine("IPC Msg Handling Completed");
          } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
          }
        });
    }

    public override void OnBackPressed() {
      if (Xamarin.Forms.Application.Current.MainPage is NavigationPage currentNav && currentNav.Navigation.NavigationStack.Count == 1) {
        Authenticator.FreeState();
      }

      base.OnBackPressed();
    }

    protected override void OnCreate(Bundle bundle) {
      TabLayoutResource = Resource.Layout.Tabbar;
      ToolbarResource = Resource.Layout.Toolbar;

      AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
      TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
      AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvOnUnhandledExceptionRaiser;

      base.OnCreate(bundle);
      Forms.Init(this, bundle);

      DisplayCrashReport();

      UserDialogs.Init(this);
      LoadApplication(new App());

      if (Intent?.Data != null) {
        HandleAppLaunch(Intent.Data.ToString());
      }
    }

    protected override void OnNewIntent(Intent intent) {
      base.OnNewIntent(intent);
      if (intent?.Data != null) {
        HandleAppLaunch(intent.Data.ToString());
      }
    }

    #region Error Handling

    private static void AndroidEnvOnUnhandledExceptionRaiser(object o, RaiseThrowableEventArgs exEventArgs) {
      var newExc = new Exception("AndroidEnvironmentOnUnhandledExceptionRaiser", exEventArgs.Exception);
      LogUnhandledException(newExc);
    }

    private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs exEventArgs) {
      var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", exEventArgs.Exception);
      LogUnhandledException(newExc);
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs exEventArgs) {
      var newExc = new Exception("CurrentDomainOnUnhandledException", exEventArgs.ExceptionObject as Exception);
      LogUnhandledException(newExc);
    }

    private static void LogUnhandledException(Exception exception) {
      try {
        const string errorFileName = "Fatal.log";
        var errorFilePath = Path.Combine(LogFolderPath, errorFileName);
        var errorMessage = $"Time: {DateTime.Now}\nError: Unhandled Exception\n{exception}\n\n";
        File.AppendAllText(errorFilePath, errorMessage);
      } catch {
        // just suppress any error logging exceptions
      }
    }

    private void DisplayCrashReport() {
      const string errorFilename = "Fatal.log";
      var errorFilePath = Path.Combine(LogFolderPath, errorFilename);

      if (!File.Exists(errorFilePath)) {
        return;
      }

      var errorText = File.ReadAllText(errorFilePath);
      new AlertDialog.Builder(this).SetPositiveButton("Clear", (sender, args) => { File.Delete(errorFilePath); }).
        SetNegativeButton("Close", (sender, args) => { }).SetMessage(errorText).SetTitle("Crash Report").Show();
    }

    #endregion
  }
}
