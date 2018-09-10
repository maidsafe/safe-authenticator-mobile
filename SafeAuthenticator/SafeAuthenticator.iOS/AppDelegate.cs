using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Services;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace SafeAuthenticator.iOS {
  [Register("AppDelegate")]
  public class AppDelegate : FormsApplicationDelegate {
    private AuthService Authenticator => DependencyService.Get<AuthService>();
    private static string LogFolderPath => DependencyService.Get<IFileOps>().ConfigFilesPath;

    public override bool FinishedLaunching(UIApplication app, NSDictionary options) {
      Forms.Init();
      LoadApplication(new App());

      AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
      TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

      DisplayCrashReport();

      return base.FinishedLaunching(app, options);
    }

    public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options) {
      Device.BeginInvokeOnMainThread(
        async () => {
          try {
            await Authenticator.HandleUrlActivationAsync(url.ToString());
            Debug.WriteLine("IPC Msg Handling Completed");
          } catch (Exception ex) {
            Debug.WriteLine($"Error: {ex.Message}");
          }
        });
      return true;
    }

    #region Error Handling

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

    private static void DisplayCrashReport() {
      const string errorFilename = "Fatal.log";
      var errorFilePath = Path.Combine(LogFolderPath, errorFilename);

      if (!File.Exists(errorFilePath)) {
        return;
      }

      var errorText = File.ReadAllText(errorFilePath);
      Device.BeginInvokeOnMainThread(
        () => {
          var vc = UIApplication.SharedApplication?.KeyWindow?.RootViewController;
          if (vc == null) {
            return;
          }
          var alert = UIAlertController.Create("Crash Report", errorText, UIAlertControllerStyle.Alert);
          alert.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Cancel, null));
          alert.AddAction(UIAlertAction.Create("Clear", UIAlertActionStyle.Default, action => { File.Delete(errorFilePath); }));
          vc.PresentViewController(alert, true, null);
        });
    }

    #endregion
  }
}
