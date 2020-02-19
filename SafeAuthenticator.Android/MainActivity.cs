// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.IO;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Plugin.CurrentActivity;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using CarouselViewRenderer = CarouselView.FormsPlugin.Android.CarouselViewRenderer;

namespace SafeAuthenticator.Droid
{
    [Activity(
        Label = "@string/app_name",
        Theme = "@style/MyTheme",
        MainLauncher = false,
        LaunchMode = LaunchMode.SingleTask,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "safe-auth")]

    // ReSharper disable once UnusedMember.Global
    public class MainActivity : FormsAppCompatActivity
    {
        private static string LogFolderPath => DependencyService.Get<IFileOps>().ConfigFilesPath;

        private AuthService Authenticator => DependencyService.Get<AuthService>();

        private bool _killApp;

        private void HandleAppLaunch(string uri)
        {
            System.Diagnostics.Debug.WriteLine($"Launched via: {uri}");
            Device.BeginInvokeOnMainThread(
                async () =>
                {
                    try
                    {
                        await Authenticator.HandleUrlActivationAsync(uri);
                        System.Diagnostics.Debug.WriteLine("IPC Msg Handling Completed");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                    }
                });
        }

        public override void OnBackPressed()
        {
            if (Xamarin.Forms.Application.Current.MainPage is NavigationPage currentNav &&
                currentNav.Navigation.NavigationStack.Count == 1)
            {
                if (_killApp)
                {
                    Authenticator.FreeState();
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, "Press Back again to Exit.", ToastLength.Short).Show();
                    _killApp = true;

                    Action myAction = () => { _killApp = false; };
                    new Handler().PostDelayed(myAction, 3000);
                }
            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            Xamarin.Essentials.Platform.Init(this, bundle);
            CrossCurrentActivity.Current.Init(this, bundle);
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvOnUnhandledExceptionRaiser;

            base.OnCreate(bundle);
            Plugin.InputKit.Platforms.Droid.Config.Init(this, bundle);
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            XamEffects.Droid.Effects.Init();
            Forms.Init(this, bundle);
            FormsMaterial.Init(this, bundle);

            DisplayCrashReport();
            CarouselViewRenderer.Init();
            CheckAppPermissions();
            LoadApplication(new App());
            Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);

            if (Intent?.Data != null)
            {
                HandleAppLaunch(Intent.Data.ToString());
            }
        }

        private void CheckAppPermissions()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                return;
            }
            else
            {
                if (PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, PackageName) != Permission.Granted
                    && PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, PackageName) != Permission.Granted)
                {
                    var permissions = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
                    RequestPermissions(permissions, 1);
                }
            }
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            if (intent?.Data != null)
            {
                HandleAppLaunch(intent.Data.ToString());
            }
        }

        #region Error Handling

        private static void AndroidEnvOnUnhandledExceptionRaiser(object o, RaiseThrowableEventArgs exEventArgs)
        {
            var newExc = new Exception("AndroidEnvironmentOnUnhandledExceptionRaiser", exEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        private static void TaskSchedulerOnUnobservedTaskException(
            object sender,
            UnobservedTaskExceptionEventArgs exEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", exEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs exEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", exEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }

        private static void LogUnhandledException(Exception exception)
        {
            try
            {
                const string errorFileName = "Fatal.log";
                var errorFilePath = Path.Combine(LogFolderPath, errorFileName);
                var errorMessage = $"Time: {DateTime.Now}\nError: Unhandled Exception\n{exception}\n\n";
                File.AppendAllText(errorFilePath, errorMessage);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }

        private void DisplayCrashReport()
        {
            const string errorFilename = "Fatal.log";
            var errorFilePath = Path.Combine(LogFolderPath, errorFilename);

            if (!File.Exists(errorFilePath))
            {
                return;
            }

            var errorText = File.ReadAllText(errorFilePath);
            new AlertDialog.Builder(this).SetPositiveButton("Clear", (sender, args) => { File.Delete(errorFilePath); })
                .SetNegativeButton("Close", (sender, args) => { }).SetMessage(errorText).SetTitle("Crash Report")
                .Show();
        }

        #endregion
    }
}
