> [!TIP]
> Integrate [Xsolla Mobile SDK](https://developers.xsolla.com/sdk/mobile/) to enable In-App Payments across mobile platforms within and outside of stores in a platform-compliant way, powered by Pay Station with over 700 payment methods.

The demo project represents one of the possible logics for the login system, inventory, and in-game store based on Xsolla products ([Login](https://developers.xsolla.com/doc/login/), [In-Game Store](https://developers.xsolla.com/doc/in-game-store/), and [Pay Station](https://developers.xsolla.com/doc/pay-station/)).

You can use the demo project in two ways:
* As an example.
* As an initial version of the login system, inventory, and in-game store. You can get a quick result and expand the demo features if needed.

To try the demo, open this project in Unity Editor and launch **Xsollus** scene from the `Assets\Xsolla.Demo\Common\Scene` directory.

The demo project covers the following user scenarios:

* Classic login via a username/email and password
* Social login (not supported for WebGL builds)
* Passwordless login via a one-time code or a link sent in the SMS or email
* Purchase for real or virtual currency
* Purchase in one click or via the shopping cart
* Consuming items from the player inventory

**NOTE:** By default, the demo is working in sandbox mode. To test the purchase process, use [test cards](https://developers.xsolla.com/doc/pay-station/references/test-cards/). Sandbox mode only simulates the behavior of a payment method, no real money is involved. You can try out sandbox mode without signing any agreements with Xsolla or paying any fees.

The demo also shows the following user management features:

* User account management
* User attributes customization


## How to use snippets from demo in your project

1. Import the demo project into your Unity project.
2. To connect the demo to your project in [Publisher Account](https://publisher.xsolla.com/signup?utm_source=sdk&utm_medium=unity-store/), specify project parameters in the **Inspector** panel:
    1. In the **Login ID** field, specify the Login ID from Publisher Account. If you use your own authorization system, leave the **Login ID** field empty.
    2. In the **Project ID** field, specify the project ID from Publisher Account.
3. Change other settings in the **Inspector** panel according to your needs (e.g., specify callback URL to redirect a user to after successful authentication, email confirmation, or password reset).
4. Customize prefabs.
5. Customize UI. To create your own solution, follow [Unity instructions](https://learn.unity.com/search/?k=%5B%22tag%3A5818e455090915002eeb1b8a%22%2C%22lang%3Aen%22%2C%22t%3Atutorial%22%5D). To adapt the demo scene UI to your application, use the [UI builder](https://developers.xsolla.com/sdk/unity/demo/how-to-use-ui-builder/).
6. Modify callbacks and set up event handling according to your application logic using SDK methods.

**NOTE:**
Follow the [step-by-step tutorials](https://developers.xsolla.com/sdk/unity/integrate-complete-solution/integrate-on-app-side/) to get started with basic SDK features.
Explore [code reference documentation](https://developers.xsolla.com/sdk-code-references/unity-store/) to learn more about SDK methods.


## Contacts

* [Support team and feedback](https://xsolla.com/partner-support)
* [Integration team](mailto:integration@xsolla.com)


## Additional resources

* [Xsolla official website](https://xsolla.com/)
* [Developer documentation](https://developers.xsolla.com/sdk/unity/)
