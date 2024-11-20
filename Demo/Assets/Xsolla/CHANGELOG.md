# Changelog

## [2.4.1] - 2024-07-19
### Added
- Unity 6 support

### Changed
- Third-party dependency manager updated to latest version 
- `PurchaseParams` structure:
  - Added `close_button` parameter, that defines the way the Google Pay payment method is displayed. If true, the button for quick payment via Google Pay is displayed at the top of the payment UI. If `false`, Google Pay is displayed in the list of payment methods according to the PayRank algorithm. `false` by default
  - Added `close_button_icon` parameter, that defines the icon of the **Close** button in the payment UI. Can be arrow or cross
  - Added `google_pay_quick_payment_button` parameter, that brings the Google Pay button to the top of the payment methods list

### Fixed:
- “Malformed URL” error  when requesting a catalog on some iOS devices

## [2.4.0] - 2024-04-17
### Added
- Ability to import SDK from a Git URL
- Ability to close Pay Station in Android Web View

### Changed
- Default Pay Station version was changed to 4
- The Xsolla settings asset is no longer included in the SDK package. It is now generated automatically upon first use to
- `has_more` parameter added to response data for the following SDK methods:
  - `XsollaCatalog.GetCatalog`
  - `XsollaCatalog.GetItemGroups`
  - `XsollaCatalog.GetVirtualCurrencyList`
  - `XsollaCatalog.GetVirtualCurrencyPackagesList`
- `PurchaseParams` data structure. Added the `payment_method` parameter
- `XsollaAuth.AuthWithXsollaWidget` SDK methods. Added the `locale` parameter

### Fixed
- Token auto refresh logic

### Removed
- Deprecated `size` and `version` parameters from `PayStationUI` data

## [2.3.0] - 2024-02-01
### Changed
- `AuthWithXsollaWidget` SDK method. Added the ‘locale’ parameter

## [2.2.0] - 2023-11-29
### Added
- Fullscreen mode for Xsolla built-in browser

### Changed
- `LinkDeviceToAccount` and `AuthViaDeviceID` SDK methods. Methods are simplified, the `DeviceInfo` parameter is not required

### Fixed
- `AuthViaDeviceID` SDK method backward compatibility for migration from 1.5.x version

## [2.1.0] - 2023-09-13
### Added
- Centrifugo integration

### Changed
- `AuthWithXsollaWidget` SDK method. Supports opening the widget on mobile platforms
- Project settings:
  - `paystationTheme` renamed to `paystationThemeId`
  - Added `FacebookClientToken` parameter for project settings. Allows to set up native user authentication via Facebook Android application
- SDK methods for getting catalog. Added the `limits` parameter for items and promotions

### Fixed
- `onCancel` event is invoked even on successful social login
- Closing browser after payment failure
- Infinite check order short polling when request returns an error
- Destruction of the `WebRequestHelper` Instance when unloading scenes


## [2.0.0] - 2023-05-18
### Added
- `XsollaAuth.AuthViaSocialNetwork` SDK method for cross-platform social network authentication. Method incapsulates web-based and native authentication methods
- `XsollaAuth.AuthViaXsollaLauncher` SDK method for authentication via Xsolla Launcher
- `XsollaAuth.AuthViaSavedToken` SDK method for authentication with a saved token which was received during previous authentication
- `XsollaAuth.IsUserAuthenticated` SDK method for checking if the user is authenticated
- `XsollaCatalog.Purchase` SDK method for purchasing virtual items for real money
- `XsollaCatalog.PurchaseForVirtualCurrency` SDK method for purchasing virtual items for virtual currency
- `XsollaCatalog.PurchaseFreeItem` SDK method for purchasing free virtual items
- `XsollaCart.Purchase` SDK method for purchasing virtual items from the cart for real money
- `XsollaCart.PurchaseFreeCart` method for purchasing free virtual items from the cart
- `SteamUtils` class which provides additional methods for authentication and purchasing via Steam

### Changed
- All methods and classes of requests are static now. You don't have to use the singleton `Instance` property anymore
- All authorization methods in `XsollaAuth` class don't pass `token` as success callback parameter now. They save token data locally for further use. Use `XsollaToken` class to get token data if you need it
-  All requests which required authorization has silent refresh token logic now
- `XsollaAuth.AuthViaDeviceID` SDK method. Device info parameters are optional now. SDK detects required parameters such as `device_id` automatically if not specified
- `XsollaAuth.Logout` SDK method. It invalidates local saved token data
- Class `Token` renamed to `XsollaToken` and made static
- SDK methods of `XsollaCart` class. Method overloads where removed. Parameter `cartId` is optional, if not specified method uses current cart ID
- `XsollaAuth.Register` SDK method. It passes `LoginLink` as success callback parameter
- `XsollaAuth.StartAuthByEmail` and `XsollaAuth.StartAuthByPhoneNumber` SDK methods. They pass `OperationId` as success callback parameter
- `BrowserUtils` renamed to `XsollaWebBrowser`
- `Canvas` component for `XsollaInAppBrowser`

### Removed
- Class `OrderTracking` is internal now and not available for use in the client code
- All Android and iOS helper classes are internal now and not available for use in the client code

### Fixed
- Disabling `bitcode` parameter of Xcode project for iOS builds


## [1.5.0] - 2023-03-27
### Added
- User login with Xsolla Login widget
- Social login helper for iOS builds
- Browser closed callback for `XsollaOrders.OpenPurchaseUi` method
- Ability to change the user agent of Xsolla in-app browser
- `Any payment` option for manual redirection in the redirect policy settings

### Fixed
- Cancel event invoking upon successful purchase for iOS builds
- Default redirect policy generation which caused error in some cases
- Use of obsolete methods for Unity 2020.3 and later
- Saving auth token outside main thread for Android builds
- Changing Xsolla in-app browser resolution after browser creation
- Endless redirect loop after purchase completion on Android


## [1.4.0] - 2022-12-16
### Added
- Xsolla Settings validation
- Xsolla Settings autofill tool for Editor
- Free items API
### Changed
- Order tracking now uses WebSockets in full with short polling left as a fallback option
- Short polling of order status now has configurable time limit and rate
- Xsolla Settings autofill tool edits
- Building SDK package now clears the settings
### Fixed
- Adding occupied email to account now returns correct error type
- Android deeplink with screen orientation change
### Removed
- Account linking demo


## [1.3.0] - 2022-10-31
### Added
- Authorization by device ID in demo
- Promotions to store methods
### Changed
- Reworked Android social login
- Merged Store payment settings and Subscriptions payment settings
### Fixed
- Mismatch project settings
- Remove temp objects between tests
- Clear token on platform auth select

## [1.2.1] - 2022-08-29
### Fixed
- DeviceID authorization

## [1.2.0] - 2022-08-19
### Added
- Locale parameter for passwordless authorization
- PS UI settings for subscription methods
- Unique Demo User generation
- Personalization for catalog requests
### Fixed
- Several API and documentation links
- Updated Newtonsoft JSON dependencies
- Built-in browser dependency on exact Newtonsoft JSON version
- Subscriptions array URL parameters
### Removed
- JWT authorization variant

## [1.1.0] - 2022-06-10
### Added
- Subscription methods
- ‘locale’ argument for email sending methods
### Fixed
- WebGL virtual currency purchase
- Resource optimization for demo
### Removed
- Custom access token authentication

## [1.0.0] - 2022-04-20
### Added
- Static SDK events
- Purchase UI via native SDK for Android
- Purchase UI via native SDK for iOS
- SDK autotest
- Scripted package export
- UI skin selection for demo
### Fixed
- Request error parsing and handling
- OAuth2.0 token refresh
- Use of redirect URL settings in several API calls
- User's inventory items that were removed from catalog
- Demo: Catalog and inventory initialization
- Other fixes and improvements
### Changed
- Updated wrapper methods for IGS&BB API calls
- Updated wrapper methods for Login API calls
- Separated 'Xsolla' and 'Xsolla.Demo'
- Separated 'Login' into several modules
- Separated 'Inventory' and 'Store' into several modules
- Updated Android libraries
- Reworked SDK settings
- Made naming of several SDK methods more consistent
- Demo: Moved user attributes to the 'User Account' page ('Character' page removed)
### Removed
- Demo: Battle pass prototype
- Demo: Email confirmation window if confirmation disabled in project settings
- Demo: Custom auth demo

## [0.7.1] - 2021-12-03
### Added
- 'Web Shop' button in demo
- Passwordless login via email
- Passwordless login via SMS
- Authentication via FaceID/TouchID for iOS builds
- Game keys and entitlement system functionality
- Checking order status using WebSocket
- 'Resend confirmation email' button
### Fixed
- Improved calculation of in-app browser size for VR projects
- Other fixes and improvements
### Changed
- Updated 'ItemPurchase' method (added 'quantity' parameter)
- Updated packages and dependencies
- Improved social networks authentication for iOS builds

## [0.7.0] - 2021-09-17
### Added
- Independent Pay Station UI parameters
- Pay Station for Safari browser (Apple Pay support)
- Virtual currency purchase order status tracking
### Fixed
- Various fixes and improvements
### Changed
- Improved refactoring and SDK Architecture
- Increased social friends recommendations list
- Updated API methods comments
- Moved Newtonsoft to packages

## [0.6.4.5] - 2021-07-16
### Added
- Vertical orientation for mobile demo scene
### Fixed
- WebGL build

## [0.6.4.4] - 2021-07-12
### Added
- Horizontal orientation for mobile demo scene
### Fixed
- Several payment systems flow for WebGL build
- Pay Station auto-closing after purchasing for Unity 2020 and later
- Demo behavior after social auth cancellation
- WeChat errors for Android build
- Errors occurring once after import for Unity 2020 and later
- Built-in browser render quality
- Included copy of Newtonsoft library now can be safely removed in case of conflict
- Various fixes and improvements
### Changed
- API methods update
- Third-party dependency manager is now included into Xsolla package
- Updated third-party Puppeteer Sharp library to match Unity's own Newtonsoft library

## [0.6.4.3] - 2021-06-10
### Added
- Device ID authorization
- OAuth2.0 state parameter for more API methods
- Pay Station UI settings for different build targets
- Additional Pay Station UI color themes
- 'Create payment token' method
- Xsolla assembly definitions
### Fixed
- In-game browser scroll
- In-game browser numpad
- In-game browser redirection links
- In-game browser post purchase behavior
- In-game browser window size now corresponds with Pay Station UI size
- Handling of a few purchase systems that become impossible to complete with in-game browser
- Various fixes and improvements
### Changed
- Changed: Pay Station opening version from 2 to 3
- Changed: Updated purchase API methods
### Removed
- Removed: Several surplus libraries (reworked corresponding functionality to use custom logic)

## [0.6.4.2] - 2021-04-23
### Added
- Item attributes support
- Items store/inventory hiding based on HideIn attribute
### Fixed
- Minor fixes

## [0.6.4.1] - 2021-04-16
### Added
- QQ Android authentication

## [0.6.4.0] - 2021-03-31
### Added
- Battle Pass
- UI Builder
- Xbox One/PS4 support
- Redirect policy settings
- Logging level settings
- API method for resend account confirmation email
### Fixed
- Payment status polling
- Apple ID authorization
### Changed
- Pay Station Widget version

## [0.6.3.4] - 2021-03-18
### Changed
- Minor update

## [0.6.3.3] - 2021-03-05
### Changed
- Added WeChat Android authentication

## [0.6.3.2] - 2021-02-05
### Changed
- Minimum supported version of Unity — 2019.4.19f1
- URL of user attributes documentation page
- Social login on Android

### Fixed
- Built-in browser for MacOS Big Sur
- Native authorization via Steam
- Automatic change of steam AppId in steam_appid.txt files when changing it in SDK settings
- Various bug fixes and improvements

## [0.6.3.1] - 2020-12-30
### Added
- Ability to pass custom parameters via API
- Access token authentication
- More social networks support
- Hashtag for user identification
- Browser background
- Two more gender options
- Custom parameters for access token auth
- Support for new Unity input system

### Changed
- Refactoring and rework

### Fixed
- Various bug fixes and improvements

## [1.0.0.1] - 2020-10-27

### Fixed
- Minor bugs

## [1.0.0.0] - 2020-10-08

### Added
- Friends system functionality
