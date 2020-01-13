// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.IO;
using System.Linq;
using Foundation;
using NUnit.Runner.Services;
using UIKit;

namespace NUnit.Runner.Tests
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private readonly string _tcpListenHost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList
            .First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            // This will load all tests within the current project
            var nunit = new App();

            // If you want to add tests in another assembly
            // nunit.AddTestAssembly(typeof(MyTests).Assembly);

            // Available options for testing
            nunit.Options = new TestOptions
            {
                // If True, the tests will run automatically when the app starts
                // otherwise you must run them manually.
                AutoRun = true,

                // If True, the application will terminate automatically after running the tests.
                // TerminateAfterExecution = true,

                // Information about the tcp listener host and port.
                // For now, send result as XML to the listening server.
                // TcpWriterParameters = new TcpWriterInfo(System.Net.IPAddress.Any.ToString(), 10500),
                TcpWriterParameters = new TcpWriterInfo(_tcpListenHost, 10500),

                // Creates a NUnit Xml result file on the host file system using PCLStorage library.
                CreateXmlResultFile = true,

                // Choose a different path for the xml result file (ios file share / library directory)
                ResultFilePath =
                    Path.Combine(
                        NSFileManager.DefaultManager.GetUrls(
                            NSSearchPathDirectory.LibraryDirectory,
                            NSSearchPathDomain.User)[0].Path, "Results.xml")
            };

            LoadApplication(nunit);

            return base.FinishedLaunching(app, options);
        }
    }
}
