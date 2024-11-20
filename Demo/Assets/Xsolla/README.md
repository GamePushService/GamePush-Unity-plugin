# Enterprise-level Xsolla SDK for Unity

Interested in new ways to extend the reach of payment options while staying compliant with platform requirements? Learn more about [Xsolla Mobile SDK](https://developers.xsolla.com/sdk/mobile/) that brings the power of Xsolla Pay Station onto mobile stores!
\
\
\
The SDK makes it easier to integrate Xsolla products into your Unity project by providing out-of-the-box data structures and methods for working with Xsolla API.

After integration, you can use:

* [Login](https://developers.xsolla.com/doc/login/) for authenticating users and managing the friend system and user account.
* [In-Game Store](https://developers.xsolla.com/doc/in-game-store/) for managing in-game purchases and player inventory in your application.
* [Pay Station](https://developers.xsolla.com/doc/pay-station/) for setting up payments.
* [Subscriptions](https://developers.xsolla.com/doc/subscriptions/) to provide users with access to a package of services under specified conditions (subscription cost and duration).

[Learn more about supported features →](#Features)

To start with the SDK, you need to install this asset and set up a project in [Xsolla Publisher Account](https://publisher.xsolla.com/signup?utm_source=sdk&utm_medium=unity-store/).

[Go to the integration guide →](https://developers.xsolla.com/sdk/unity/)

## Features

### Authentication

* [OAuth 2.0](https://oauth.net/2/) protocol-based authentication.
* Classic login (username/email and password).
* Social login.
* Social login on the user’s device via a social network client (Google, Facebook, WeChat, and QQ).
* Login via a launcher (Steam or [Xsolla Launcher](https://developers.xsolla.com/doc/launcher/)).
* Login via a device ID.
* Passwordless login via a one-time code or a link sent via SMS or email.
* Xsolla Login widget that currently supports classic, social, and passwordless login. The widget opens in the built-in browser and can’t be used for native social login.

### User management

* User attributes to manage additional information.
* Cross-platform account linking.
* Secure Xsolla storage for user data. Alternatively, you can connect PlayFab, Firebase, or your custom storage.

**INFO:** You can also manage user accounts and access rights via Xsolla Publisher Account.

### Catalog

* Virtual currency:
    * Sell virtual currency in any amount or in packages (for real money or other virtual currency).
    * Sell hard currency (for real money only).
* Virtual items:
    * Set up a catalog of in-game items.
    * Sell virtual items for real and virtual currency.
* Bundles:
    * Sell bundles for real or virtual currency.
* Promotional campaigns:
    * Reward users with virtual currency packages, game keys, or virtual items for coupons.
    * Give users bonuses or apply a discount on items in the cart with promo codes.

**INFO:** You can add items in the catalog or manage campaigns with discounts, coupons, and promo codes via Xsolla Publisher Account.

### Subscriptions

* Selling subscriptions.
* Subscription renewal and cancelation.
* Subscription management from a user’s dashboard.

**INFO:** You can add and manage subscription plans via Xsolla Publisher Account.

### Item purchase

* Sell items in one click or via the shopping cart.
* Provide users with a convenient payment UI. Main features:
    * 700+ payment methods in 200+ countries, including bank cards, digital wallets, mobile payments, cash kiosks, gift cards, and special offers.
    * 130+ currencies.
    * UI localized into 20+ languages.
    * Desktop and mobile versions.

**INFO:** Xsolla Publisher Account provides you with purchase analytics, transaction history, and other statistics.

### Player inventory

* Get and verify an inventory.
* Consume items according to the game logic.
* Consume virtual currency according to the in-game logics (for example, when opening a location or purchasing level for some currency).
* Synchronize all purchases and premium rewards of the user across all platforms.

## Requirements

### System requirements

* 64-bit OS
* Windows 7 SP1 and later
* macOS 10.12 and later
* The version of Unity not earlier than 2019.4.19f1

### Target OS

* Android
* iOS
* macOS
* Windows 64-bit

Additionally, the asset supports [creating WebGL build](https://developers.xsolla.com/sdk/unity/how-tos/application-build/#unity_sdk_how_to_build_webgl) to run your application in a browser.

The SDK uses [Google Mobile Services](https://www.android.com/gms/) and doesn’t support builds for devices without Google Mobile Services, such as Huawei.

**NOTE:**
We recommend you use the Mono compiler for desktop platforms as it’s compatible with the provided in-game browser. If you use other browser solutions, you can use the IL2CPP compiler instead.
You can use either Mono or IL2CPP compilers to create game builds for Android.

## Install SDK

### Import package from an archive

1. Download the SDK from [CDN](https://cdn.xsolla.net/sdk-unity-assets/xsolla-commerce-sdk.zip) or [GitHub](https://github.com/xsolla/store-unity-sdk/releases).

**NOTE:**
Only the latest version of the SDK is available through CDN. If you need a different version, you can download it from GitHub.

2. Unzip the package.
3. In the Unity editor, go to **Assets > Import Package > Custom Package** in the main menu and select the SDK.
4. Follow the [integration guide](https://developers.xsolla.com/sdk/unity/integrate-complete-solution/get-started/) to configure project on Xsolla side.

### Import package from git repository

**NOTE:**

For the package manager to work correctly, [git client](https://git-scm.com/) should be installed. For detailed information about the prerequisites for using the package manager, refer to the [Unity documentation](https://docs.unity3d.com/Manual/upm-ui-giturl.html).

1. Open your Unity project or create a new one.
2. In the main menu, click **Window > Package Manager**.
3. Add a package as a dependence:
 a. Click the **+** icon and select  Add package from git URL.
 b. Specify the git repository URL: `https://github.com/xsolla/store-unity-sdk.git?path=Assets/Xsolla`.
 c. Click **Add** and wait for the import to finish.

## Usage

To send requests to Xsolla servers and receive responses, the SDK provides the `XsollaAuth`, `XsollaCatalog`, `XsollaCart`, and `XsollaOrders` classes. If you want to implement your own logic for buying items or inventory management, and don’t want to write boilerplate code for API calls, these classes are a good place to start.

Follow the [step-by-step tutorials](https://developers.xsolla.com/sdk/unity/tutorials/) to get started with basic SDK features.

Explore [code reference documentation](https://developers.xsolla.com/sdk-code-references/unity-store/) to learn more about SDK methods.

# Known issues

## Unable to resolve reference UnityEditor.iOS.Extensions.Xcode

### Issue description

The issue appears when using External Dependency Manager on Unity version 2020.1.0f1 and later.

When building the application, an error message is displayed:

```
Assembly 'Packages/com.google.external-dependency-manager/ExternalDependencyManager/Editor/Google.IOSResolver_v1.2.161.dll' will not be loaded due to errors:
Unable to resolve reference 'UnityEditor.iOS.Extensions.Xcode'. Is the assembly missing or incompatible with the current platform?
Reference validation can be disabled in the Plugin Inspector.
```

**Issue status**: Fixed in 0.6.4.5.

### Workaround

Install iOS Build Support module from Unity Hub.

## Error occurred running Unity content on page of WebGL build

### Issue description

The issue may appear when logging in WebGL build. The following error message is displayed:

![WebGL error message](https://i.imgur.com/me3ADT4.png "WebGL error message")

See details on cause of the issue on [Unity Issue Tracker](https://issuetracker.unity3d.com/issues/il2cpp-notsupportedexceptions-exception-is-thrown-in-build-with-newtonsoft-dot-json-plugin).

**Issue status:** Won’t fix.


### Workaround

1. Open Unity project.
2. Click **Edit > Project Settings** in the main menu.
3. In the **Player** section, go to the WebGL build settings tab.
4. Go to the **Other Settings** section.
5. Uncheck **Strip engine code** box.
6. Go to the **Publishing Settings** section.
7. Check the **Decompression Fallback** box.
8. Create a new WebGL build.


## Pricing

Xsolla offers the necessary tools to help you build and grow your gaming business, including personalized support at every stage. The terms of payment are determined by the contract that you can sign via Publisher Account.

The cost of using all Xsolla products is 5% of the amount you receive for the sale of the game and in-game goods via Xsolla Pay Station. If you don’t use Xsolla Pay Station in your application, but use other products, contact your Account Manager to clarify the terms and conditions.

Explore [legal information](https://xsolla.com/pricing) that helps you work with Xsolla.

## License

See the [LICENSE](https://github.com/xsolla/store-unity-sdk/blob/master/LICENSE.txt) file.

## Contacts

* [Support team and feedback](https://xsolla.com/partner-support)
* [Integration team](mailto:integration@xsolla.com)


## Additional resources

* [Xsolla official website](https://xsolla.com/)
* [Developers documentation](https://developers.xsolla.com/sdk/unity/)
* [Code reference documentation](https://developers.xsolla.com/sdk-code-references/unity-store/)
