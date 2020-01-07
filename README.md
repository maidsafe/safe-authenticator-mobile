# Safe Authenticator Mobile

The SAFE Authenticator acts as a gateway to the [SAFE Network](https://safenetwork.tech/) by enabling users to create an account & authenticate themselves onto the SAFE Network. It helps users ensure they have full control over the permissions they grant to SAFE apps.

**Maintainer:** Ravinder Jangra (ravinder.jangra@maidsafe.net)

## Build Status

|CI service|Platform|Status|
|---|---|---|
|Azure DevOps|MacOS| [![Build status](https://dev.azure.com/maidsafe/SafeApp/_apis/build/status/SafeApp-Mobile-CI)](https://dev.azure.com/maidsafe/SafeApp/_build/latest?definitionId=7) |

## Table of Contents

1. [Overview](#Overview)
2. [Features](#Features)
3. [Supported platforms](#Documentation)
4. [Development](#Development)
    * [Project Structure](#Project-structure)
    * [Interfacing with SCL](#Interfacing-with-Safe-Client-Libs)
    * [Platform Invoke](#Interoperability-between-C-managed-and-unmanaged-code)
    * [Tests](#Tests)
    * [Tools required](#Tools-required)
5. [Contributing](#Contributing)
6. [Useful resources](#Useful-resources)
7. [Copyrights](#Copyrights)
8. [Further Help](#Further-Help)
9. [License](#License)

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
  The SAFE application authorizes through the Authenticator with the required access permissions. The application can create its own container and request access to default containers of the SAFE Network i.e. documents, downloads, music, pictures, videos, public and public names, or other application's containers through the authorization request.
  
  * **Auth Request:**
    Allow apps to request default container access & also to create an app's own private container.
  * **Container Request:**
    Application request access for user's default containers only.
  * **Shared Mutable Data Request:**
    Request access to mutable data owned by the user.
  * **Unregistered Access:**
    Allow an app to read public unencrypted content.

* **User Grants Access:** When the user approves the request, application specific encryption keys are generated. The application will be identified in the network using its keys. When the user grants or denies authorization, the application will receive a URI. The user has the option to grant permission to an application to access user data.

* **User can revoke app access:** User has the option to revoke access for an app that they have granted access to previously.

### Screenshots

<img alt="Tutorial" src="/Screenshots/Tutorial.png?raw=true" width="250"/>  <img alt="Login" src="/Screenshots/Login.png?raw=true" width="250"/> <img alt="Create Account" src="/Screenshots/CreateAccount1.png?raw=true" width="250"/>

<img alt="Home" src="/Screenshots/Home.png?raw=true" width="250"/>  <img alt="Request Details" src="/Screenshots/RequestDetail.png?raw=true" width="250"/> <img alt="Settings" src="/Screenshots/Settings.png?raw=true" width="250"/>

## Supported platforms

* Android 5.0+ (armeabi-v7a, x86_64)
* iOS 11+ (ARM64, x64)

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

* [Visual Studio](https://visualstudio.microsoft.com/) 2017 or later editions with the following workloads:
  * [Mobile development with .NET (Xamarin)](https://visualstudio.microsoft.com/vs/visual-studio-workloads/)
* [Cake](https://cakebuild.net/) - Cross-platform build script tool used to build the projects and run the tests.

## Contributing

As an open source project, we're excited to accept contributions to the code from outside of MaidSafe and are striving to make that as easy and clean as possible.

With enforced linting and commit style clearly laid out, as well as a list of more accessible issues for any project labelled with Help Wanted.

### Project board

GitHub project boards are used by the maintainers of this repository to keep track and organise development priorities.

There could be one or more active project boards for a repository. One main project will be used to manage all tasks corresponding to the main development stream (master branch). A separate project may be used to manage each PoC and/or prototyping development, and each of them will track a dedicated development branch.

New features which imply a big number of changes will be developed in a separate branch but tracked in the same main project board, re-basing it with master branch regularly and fully testing the feature on its branch before it's merged into the master branch after it was fully approved.

The main project contains the following Kanban columns to track the status of each development task:

* `Triage`: New issues which need to be reviewed and evaluated before taking the decision to implement it.
* `Low Priority`: Issues that will be picked up in the current milestone.
* `In Progress`: Task is assigned to a person and it's in progress.
* `Needs Review`: A Pull Request which completes the task has been sent and it needs to be reviewed.
* `Reviewer approved`: The PR sent was approved by reviewer/s and it's ready for merge.
* `Ready for QA`: The fix for the issue has been merged into master and is ready for final QA testing.
* `Done`: QA has verified that the fix is complete and does not affect anything else.

### Issues

Issues should clearly lay out the problem, platforms experienced on, as well as steps to reproduce the issue.

This aids in fixing the issues but also quality assurance, to check that the issue has indeed been fixed.

Issues are labelled in the following way depending on its type:

* `bug`: The issue is a bug in the product.
* `feature`: The issue is a new and inexistent feature to be implemented.
* `enhancement`: The issue is an enhancement to either an existing feature in the product or to the infrastructure around the development process.
* `blocked`: The issue cannot be resolved as it is blocked by another task. In this case, the task that it is blocked by should be referenced.
* `documentation`: A documentation-related task.
* `e/__`: Specifies the effort required for the task.
* `p/__`: Specifies the priority of the task.

### Commits and Pull Requests

Commit message should follow [these guidelines](https://github.com/autumnai/leaf/blob/master/CONTRIBUTING.md#git-commit-guidelines) and should therefore strive to tackle one issue/feature, and code should be pre-linted before commit.

PRs should clearly link to an issue to be tracked on the project board. A PR that implements/fixes an issue is linked using one of the [GitHub keywords](https://help.github.com/articles/closing-issues-using-keywords). Although these type of PRs will not be added themselves to a project board (just to avoid redundancy with the linked issue). However, PRs which were sent spontaneously and not linked to any existing issue will be added to the project and should go through the same process as any other tasks/issues.

Where appropriate, commits should _always_ contain tests for the code in question.

### Changelog and releases

The changelog is currently maintained manually, each PR sent is expected to have the corresponding modification in the CHANGELOG file, under the 'Not released' section.

The release process is triggered by the maintainers of the package once it is merged to master.

## Useful resources

* [Using High-Performance C++ Libraries in Cross-Platform Xamarin.Forms Applications](https://devblogs.microsoft.com/xamarin/using-c-libraries-xamarin-forms-apps/)
* [Native interoperability](https://docs.microsoft.com/en-us/dotnet/standard/native-interop/)
* [Interop with Native Libraries](https://www.mono-project.com/docs/advanced/pinvoke/)
* [Using Native Libraries in Xamarin.Android](https://docs.microsoft.com/en-us/xamarin/android/platform/native-libraries)
* [Referencing Native Libraries in Xamarin.iOS](https://docs.microsoft.com/en-us/xamarin/ios/platform/native-interop)

## Copyrights

Copyrights in the SAFE Network are retained by their contributors. No copyright assignment is required to contribute to this project.

## Further help

Get your developer related questions clarified on [SAFE Dev Forum](https://forum.safedev.org/). If you're looking to share any ideas or thoughts on the SAFE Network you can reach out on [SAFE Network Forum](https://safenetforum.org/)

## License

Licensed under the General Public License (GPL), version 3 [LICENSE](http://www.gnu.org/licenses/gpl-3.0.en.html).
