# Safe Authenticator Mobile
The SAFE Authenticator acts as a gateway to the [SAFE Network](https://safenetwork.tech/) by enabling users to create an account & authenticate themselves onto the SAFE Network. It helps users ensure they have full control over the permissions they grant to SAFE apps. 

**Maintainer:** Krishna Kumar (krishna.kumar@maidsafe.net)

|Build Status | 
|------------ | 
|[![Build Status](https://dev.azure.com/maidsafe/SafeAuthenticator/_apis/build/status/SafeAuthenticator)](https://dev.azure.com/maidsafe/SafeAuthenticator/_build/latest?definitionId=1)| 
 
## Features
- **Create your account & login to the SAFE network:**
    - Start by providing a secret & password that serves as your digital fingerprint on the SAFE Network to create an account.
    - Login to the SAFE Network
    - Auto-reconnect: Turn on auto-reconnect to automatically log into the app when resuming from background mode.

- **Handle Access Requests from Apps:** 
  The SAFE application authorizes through the Authenticator with the required access permissions. The application can create its own container and request access to default containers of the SAFE Network i.e. documents, downloads, music, pictures, videos, public and public names, or other application's containers through the authorization request.
  
    - **Container Request:**
    Application will request access for user's default containers only.
    - **Auth Request:**
    Allow apps to request default container access & also to create an apps own private container.
    - **Shared Mutable Data Request:**
    Application can request access to mutable data owned by the user. 
    - **Unregistered Access:**
    Allow an app to read public unencrypted content.

- **User Grants Access:** When the user approves the request, application specific encryption keys are generated. The application will be identified in the network using its keys. When the user grants or denies authorization, the application will receive a URI. User has the option to grant permission to an application to access user data.

- **User can revoke app access:** User has the option to revoke access for an app that they have granted access to previously.


## Building

### Pre-requisites
If building on Visual Studio 2017, you will need the following SDKs and workloads installed:

### Workloads required:
- Xamarin

### Required SDK/Tools
- Android 9.0 SDK
- Xcode 10+

### Supported platforms
- Android 4.1+ (armeabi-v7a, x86_64)
- iOS 8+ (ARM64, x64)

### Screenshot
<img src="https://i.imgur.com/ctuMXbh.png" width="250" height="350"> <img src="https://i.imgur.com/1403il6.png" width="250" height="350">

## Further help
Get your developer related questions clarified on [SAFE Dev Forum](https://forum.safedev.org/). If you're looking to share any ideas or thoughts on the SAFE Network you can reach out on [SAFE Network Forum](https://safenetforum.org/)

## License
Licensed under the General Public License (GPL), version 3 [LICENSE](http://www.gnu.org/licenses/gpl-3.0.en.html).

## Contribution
Copyrights in the SAFE Network are retained by their contributors. No copyright assignment is required to contribute to this project.
