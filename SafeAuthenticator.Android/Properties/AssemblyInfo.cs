// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System.Reflection;
using Android.App;
using Xamarin.Forms;

// Information about this assembly is defined by the following attributes.
// Change them to the values specific to your project.

[assembly: AssemblyTitle("SafeAuthenticator.Android")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("${AuthorCopyright}")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ResolutionGroupName("Xamarin")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

[assembly: AssemblyVersion("1.0.0")]

// The following attributes are used to specify the signing key for the assembly,
// if desired. See the Mono documentation for more information about signing.

// [assembly: AssemblyDelaySign(false)]
// [assembly: AssemblyKeyFile("")]

#if DEBUG
[assembly: Application(Debuggable = true)]
#else
[assembly: Application(Debuggable = false)]
#endif
