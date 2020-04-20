# Safe Authenticator Mobile

The SAFE Authenticator acts as a gateway to the [SAFE Network](https://safenetwork.tech/) by enabling users to create an account & authenticate themselves onto the SAFE Network. It helps users ensure they have full control over the permissions they grant to SAFE apps.

**Maintainer:** Ravinder Jangra (ravinder.jangra@maidsafe.net)

## Build Status

|CI service|Platform|Status|
|---|---|---|
|Azure DevOps|MacOS| [![Build Status](https://dev.azure.com/maidsafe/SafeAuthenticator/_apis/build/status/SafeAuthenticator-CI?branchName=master)](https://dev.azure.com/maidsafe/SafeAuthenticator/_build/latest?definitionId=17&branchName=master) |

## Table of Contents

1. [Overview](#Overview)
2. [Features](#Features)
3. [User Guide](#User-Guide)
4. [Development](#Development)
    * [Project Structure](#Project-structure)
    * [Interfacing with SCL](#Interfacing-with-Safe-Client-Libs)
    * [Platform Invoke](#Interoperability-between-C-managed-and-unmanaged-code)
    * [Tests](#Tests)
    * [Tools required](#Tools-required)
5. [Useful resources](#Useful-resources)
6. [Further Help](#Further-Help)
7. [License](#License)
8. [Contributing](#Contributing)

## Overview

SAFE Authenticator mobile is a cross platform mobile (Android, iOS) application that can be used to create an account and login into the SAFE Network. The app also helps the user to authenticate other SAFE apps and manage access permissions for the already registered applications.

The app is developed using [Xamarin.Forms](https://github.com/xamarin/Xamarin.Forms). Xamarin.Forms is a cross-platform UI toolkit that allows developers to efficiently create native user interface layouts that can be shared across iOS, Android.

The app contains .NET wrapper for [safe_authenticator](https://github.com/maidsafe/safe_client_libs/tree/master/safe_authenticator), C# bindings and `safe_authenticator` native libraries. `safe_authenticator` provides authenticator functions, which include users account registration, login, apps authentication, apps revocation, and communication with apps through the IPC mechanism.

## Features

* **Create your account & login to the SAFE network:**
  * Start by providing a secret & password that serves as your digital fingerprint on the SAFE Network to create an account.
  * Login to the SAFE Network
  * Auto-reconnect: Turn on auto-reconnect to automatically log into the app when resuming from background mode.

* **Handle Access Requests from Apps:**
  The SAFE application authorises through the Authenticator with the required access permissions. The application can create its own container and request access to default containers of the SAFE Network i.e. documents, downloads, music, pictures, videos, public and public names, or other application's containers through the authorisation request.

  * **Auth Request:**
    Allow apps to request default container access & also to create an app's own private container.
  * **Container Request:**
    Application request access for user's default containers only.
  * **Shared Mutable Data Request:**
    Request access to mutable data owned by the user.
  * **Unregistered Access:**
    Allow an app to read public unencrypted content.

* **User Grants Access:** When the user approves the request, application specific encryption keys are generated. The application will be identified in the network using its keys. When the user grants or denies authorisation, the application will receive a URI. The user has the option to grant permission to an application to access user data.

* **User can revoke app access:** User has the option to revoke access for an app that they have granted access to previously.

* **Select network:** User can connect to the different networks using different network connection files from the settings page.

## User Guide

### Installation

The latest version of the Authenticator app can be downloaded using following links and QR code for Android and iOS devices.

|Platform|OS & Architecture |Download Link| QR Code|
|-|-|-|-|
|Android| 5.0+ (armeabi-v7a, x86_64) | [AppCenter](https://appcenter.ms/orgs/MaidSafe-Apps/apps/Safe-Authenticator-Mobile/distribute/distribution-groups/Community%20Release/releases), [GitHub](https://github.com/maidsafe/safe-authenticator-mobile/releases/latest) | <img src="docs/AppCenter-QR/android.png"  width="100" alt="Android-QR" /> |
|iOS    | iOS 11+ (ARM64, x64)       | [AppCenter](https://appcenter.ms/orgs/MaidSafe-Apps/apps/Safe-Mobile-Authenticator/distribute/distribution-groups/Authenticator%20iOS/releases) | <img src="docs/AppCenter-QR/ios.png"  width="100" alt="iOS-QR" /> |

_**Note:** We use Azure App Center to distribute iOS builds. Please register [here](https://forms.gle/Svp7PU6dcf4ywmu19) so we can add you in our testing group so you can download and install the app._

### Screenshots

<img alt="Tutorial" src="docs/Screenshots/Tutorial.png?raw=true" width="250"/>  <img alt="Login" src="docs/Screenshots/Login.png?raw=true" width="250"/> <img alt="Create Account" src="docs/Screenshots/CreateAccount1.png?raw=true" width="250"/>

<img alt="Home" src="docs/Screenshots/Home.png?raw=true" width="250"/>  <img alt="Request Details" src="docs/Screenshots/RequestDetail.png?raw=true" width="250"/> <img alt="Settings" src="docs/Screenshots/Settings.png?raw=true" width="250"/>

## Development

### Project structure

* **SafeAuthenticator:**
  * C# API wrapper for the `safe_authenticator` bindings
  * C# safe_authenticator bindings generated from `safe_client_libs`
  * Binding utilities and helper functions
  * Common UI code and SAFE logic for mobile app
* **SafeAuthenticator.Platform:**
  * Platform: Android, iOS
  * Contains native libraries for the platform
  * C# `safe_authenticator` platform bindings
  * Platform specific/dependent code
    * Custom controls for native UI
    * Platform assets
    * Platform dependent service code
* **Tests:**
  * Contains shared unit test project for Safe Authenticator API.
  * Platform specific projects containing setup required to run shared tests on Android and iOS.

### Interfacing with Safe Client Libs

The package uses native code written in Rust and compiled into the platform specific code. Learn more about the `safe_client_libs` in [the SAFE client libraries wiki](https://github.com/maidsafe/safe_client_libs/wiki).

Instructions to update the `safe_authnenticator` bindings in the Safe Authenticator App:

* [Generate bindings](https://github.com/maidsafe/safe_client_libs/wiki/Building-Client-Libs) for `safe_authenticator`.
* Update `AuthTypes`, `BindingUtils` and `IAuthBindings` file in `SafeAuthenticator` project.
* Update binding classes `AuthBindings.cs` and`AuthBindings.Manual.cs` in platform specific code with newly generated bindings.

***Note:** Please make sure the changes made in the manual files in SAFE Authenticator are synced with safe_client_libs and vice versa.*

### Interoperability between C# managed and unmanaged code

[Platform invoke](https://www.mono-project.com/docs/advanced/pinvoke/) is a service that enables managed code to call unmanaged functions that are implemented in dynamic link libraries or native libraries. It locates and invokes an exported function and marshals its arguments (integers, strings, arrays, structures, and so on) across the interoperation boundary as needed. Check links in [useful resources](#Useful-resources) section to know more about how P/Invoke works in different .NET environments and platforms.

### Tests

We use shared unit tests for `safe_authenticator` API which can be run on Android and iOS platforms.

### Tools required

* [Visual Studio](https://visualstudio.microsoft.com/) 2017 or later editions:
  * [Mobile development with .NET (Xamarin)](https://docs.microsoft.com/en-us/xamarin/get-started/installation/?pivots=windows)
* [Cake](https://cakebuild.net/) - Cross-platform build script tool used to build the projects and run the tests.

## Useful resources

* [Using High-Performance C++ Libraries in Cross-Platform Xamarin.Forms Applications](https://devblogs.microsoft.com/xamarin/using-c-libraries-xamarin-forms-apps/)
* [Native interoperability](https://docs.microsoft.com/en-us/dotnet/standard/native-interop/)
* [Interop with Native Libraries](https://www.mono-project.com/docs/advanced/pinvoke/)
* [Using Native Libraries in Xamarin.Android](https://docs.microsoft.com/en-us/xamarin/android/platform/native-libraries)
* [Referencing Native Libraries in Xamarin.iOS](https://docs.microsoft.com/en-us/xamarin/ios/platform/native-interop)

## Further help

Get your developer related questions clarified on [SAFE Dev Forum](https://forum.safedev.org/). If you're looking to share any ideas or thoughts on the SAFE Network you can reach out on [SAFE Network Forum](https://safenetforum.org/)

## License

This SAFE Network library is dual-licensed under the Modified BSD ([LICENSE-BSD](LICENSE-BSD) https://opensource.org/licenses/BSD-3-Clause) or the MIT license ([LICENSE-MIT](LICENSE-MIT) https://opensource.org/licenses/MIT) at your option.

## Contributing

Want to contribute? Great :tada:

There are many ways to give back to the project, whether it be writing new code, fixing bugs, or just reporting errors. All forms of contributions are encouraged!

For instructions on how to contribute, see our [Guide to contributing](https://github.com/maidsafe/QA/blob/master/CONTRIBUTING.md).
