# safe-authenticator-mobile Change Log

## [0.2.0]

* Add new choose a vault page to choose different vaults to connect to.
* Add test coin permissions in request details dialog and app info page.
* Update create account and login page to use material design controls.
* Update UI to use passphrase/password instead of secret/password.
* Update minimum supported platform versions.
* Add better runtime permission check logic for Android devices.
* Update README file to reflect the project related changes.
* Remove invitation token related logic and text.
* Remove multiple custom controls and platform renderers for better UI and performance.
* Update Android to use AndroidX packages.
* Remove deprecated API tests
* Refactor to use solution level code analysis
* Update CI to create GitHub releases on version change PR.
* Update bindings and wrapper API to support the latest native libs.
* Update native libs to latest scl master [#09043cb](https://github.com/maidsafe/safe_client_libs/commit/09043cbfd1911e73e70875cb7501e5a2b7b3a146)

## [0.1.5]

* Fix the app crash caused by null reference exception in the material entry renderer in iOS  (#155) ([f55cd5c])(https://github.com/maidsafe/safe-authenticator-mobile/commit/f55cd5c47044d6332fc179550ca2646272683298)

## [0.1.4]

* Update iOS build from iPhone/iPad to universal (#150) ([476a09e](https://github.com/maidsafe/safe-authenticator-mobile/commit/476a09e)), closes [#150](https://github.com/maidsafe/safe-authenticator-mobile/issues/150)
* Update UI to improve the experience on big screen devices  (#152) ([3ffcf3b](https://github.com/maidsafe/safe-authenticator-mobile/commit/3ffcf3b)), closes [#152](https://github.com/maidsafe/safe-authenticator-mobile/issues/152)
* Update bindings to use v0.9.1 of safe_authenticator libs ([53e20e9](https://github.com/maidsafe/safe-authenticator-mobile/commit/53e20e9))
* Update packages for app and test projects (#151) ([ba3db1d](https://github.com/maidsafe/safe-authenticator-mobile/commit/ba3db1d)), closes [#151](https://github.com/maidsafe/safe-authenticator-mobile/issues/151)
* Update CI to timeout for TCP listener (#137) ([31ebfa7](https://github.com/maidsafe/safe-authenticator-mobile/commit/31ebfa7)), closes [#137](https://github.com/maidsafe/safe-authenticator-mobile/issues/137)
* Disable test builds for iPhone Release configuration (#153) ([e1ee071](https://github.com/maidsafe/safe-authenticator-mobile/commit/e1ee071)), closes [#153](https://github.com/maidsafe/safe-authenticator-mobile/issues/153)

## [0.1.3]

* Handle error when app URI not found (PR [#132](https://github.com/maidsafe/safe-authenticator-mobile/pull/132)).
* Handle invalid container names (PR [#133](https://github.com/maidsafe/safe-authenticator-mobile/pull/133)).
* Fix container name displayed for other app containers (PR [#138](https://github.com/maidsafe/safe-authenticator-mobile/pull/138)).
* Use AuthFlushAppRevocationQueueAsync API to flush revoke queue (PR [#140](https://github.com/maidsafe/safe-authenticator-mobile/pull/140)).

## [0.1.2]

* Add fallback option when opening hyperlinks using ChromeCustomTabs (PR [#123](https://github.com/maidsafe/safe-authenticator-mobile/pull/123)).
* Hide auto-reconnect option if API level < 19 (PR [#123](https://github.com/maidsafe/safe-authenticator-mobile/pull/123)). 
* Use an Image a with tap gesture instead of ImageButton (PR [#123](https://github.com/maidsafe/safe-authenticator-mobile/pull/123)).

## [0.1.1]

* Add tutorial page for feature showcase (PR [#49](https://github.com/maidsafe/safe-authenticator-mobile/pull/49)).
* Add Settings page (PR [#49](https://github.com/maidsafe/safe-authenticator-mobile/pull/49)).
* Add launch screens for Android and iOS (PR [#49](https://github.com/maidsafe/safe-authenticator-mobile/pull/49)).
* Use native controls for Android and iOS (PR [#49](https://github.com/maidsafe/safe-authenticator-mobile/pull/49)).
* Use SafariViewController and Chrome Custom tabs to open the links (PR [#49](https://github.com/maidsafe/safe-authenticator-mobile/pull/49)).
* Auto resize authorisation popup for different requests (PR [#49](https://github.com/maidsafe/safe-authenticator-mobile/pull/49)).
* Show first 6 digits of the hash in shared MData request if MData's meta name is empty (PR [#49](https://github.com/maidsafe/safe-authenticator-mobile/pull/49)).
* Update registered applist manually before sending authentication response (PR [#64](https://github.com/maidsafe/safe-authenticator-mobile/pull/64)). 
* Handle delay after authentication request popup (PR [#66](https://github.com/maidsafe/safe-authenticator-mobile/pull/66)).
* Update keyboard return key to move to next entry (PR [#71](https://github.com/maidsafe/safe-authenticator-mobile/pull/71)).
* Add tap animation for external links in SettingsPage (PR [#77](https://github.com/maidsafe/safe-authenticator-mobile/pull/77)).
* Display progress dialogs natively for Android and iOS (PR [#79](https://github.com/maidsafe/safe-authenticator-mobile/pull/79)).
* Display refresh icon in place of account info icon at the time of refresh (PR [#74](https://github.com/maidsafe/safe-authenticator-mobile/pull/74)).
* Enable scroll when keyboard is displayed (PR [#87](https://github.com/maidsafe/safe-authenticator-mobile/pull/87)).

## [0.1.0]

* Add revoke app feature: User has the option to revoke access for an app that they had previously granted access (PR [#17](https://github.com/maidsafe/safe-authenticator-mobile/pull/17)).
* Add support for container request, share MData request, unregistered app request (PR [#22](https://github.com/maidsafe/safe-authenticator-mobile/pull/22)).
* Update authentication popup to show request details (PR [#22](https://github.com/maidsafe/safe-authenticator-mobile/pull/22)).
* Add secret and password strength checker when creating an account (PR [#23](https://github.com/maidsafe/safe-authenticator-mobile/pull/23)).
* Bug Fixes: RegisteredApp list refresh, auto-reconnect (PR [#19](https://github.com/maidsafe/safe-authenticator-mobile/pull/19)).
* Updated alerts to show appropriate error messages for different error codes (PR [#28](https://github.com/maidsafe/safe-authenticator-mobile/pull/28)).
* Bindings: Update bindings to support v0.9.0 of safe_authenticator (PR [#29](https://github.com/maidsafe/safe-authenticator-mobile/pull/29)).
* Remove support for Android x86 and add support for Android x86_64 (PR [#29](https://github.com/maidsafe/safe-authenticator-mobile/pull/29)).
* Update build target framework versions to 9.0 and 12.1 for android and iOS respectively (PR [#18](https://github.com/maidsafe/safe-authenticator-mobile/pull/18)).
* Use Xamarin.Essentials instead of Xamarin.Auth for secure storage (PR [#16](https://github.com/maidsafe/safe-authenticator-mobile/pull/16)).
