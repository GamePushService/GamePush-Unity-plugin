using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePush;
using UnityEngine;

namespace GamePush.Native
{
    public interface INativePaymentsSdk
    {
        bool IsAvailable { get; }
        bool IsSubscriptionsAvailable { get; }
        void Initialize();
        Task<List<FetchProducts>> MapProducts(List<FetchProducts> products);
        Task Purchase(FetchProducts product);
        Task Consume(string idOrTag);
        Task Subscribe(FetchProducts product, FetchPlayerPurchases existing);
        Task Unsubscribe(FetchProducts product, FetchPlayerPurchases existing);
    }

    public static class NativePaymentsSdks
    {
        static bool _loggedMissing;

        static INativePaymentsSdk _oneStore;

        public static INativePaymentsSdk Current { get; private set; } = NativeUnavailablePaymentsSdk.Instance;

        public static void RegisterOneStore(INativePaymentsSdk sdk)
        {
            _oneStore = sdk;
        }

        public static void Initialize()
        {
            var payments = NativeCore.Payments;
            if (payments.NeedsGooglePlay)
            {
#if GP_BUILD_PAYMENTS_GOOGLE && GP_UNITY_PURCHASING && UNITY_ANDROID && !UNITY_EDITOR
                Current = NativeGooglePlayPaymentsSdk.Instance;
                NativeGooglePlayPaymentsSdk.Instance.Initialize();
                GP_Logger.Info("PAYMENTS", "Google Play IAP adapter ready");
                return;
#else
                Current = NativeUnavailablePaymentsSdk.Instance;
                LogMissingOnce();
                return;
#endif
            }

            if (payments.NeedsOneStore)
            {
#if GP_BUILD_PAYMENTS_ONESTORE && UNITY_ANDROID && !UNITY_EDITOR
                if (_oneStore != null)
                {
                    Current = _oneStore;
                    Current.Initialize();
                    GP_Logger.Info("PAYMENTS", "One Store IAP adapter ready");
                    return;
                }
#endif
                Current = NativeUnavailablePaymentsSdk.Instance;
                LogMissingOnce();
                return;
            }

            if (payments.NeedsWebCheckout)
            {
#if GP_BUILD_PAYMENTS_WEB && UNITY_ANDROID && !UNITY_EDITOR
                Current = NativeWebCheckoutPaymentsSdk.Instance;
                NativePaymentsWebView.EnsureCallback();
                GP_Logger.Info("PAYMENTS", payments.ActiveService + " WebView checkout ready");
                return;
#else
                Current = NativeUnavailablePaymentsSdk.Instance;
                LogMissingOnce();
                return;
#endif
            }

            Current = NativeUnavailablePaymentsSdk.Instance;
        }

        public static void LogMissingOnce()
        {
            if (_loggedMissing)
                return;
            _loggedMissing = true;
            var service = NativeCore.Payments.ActiveService ?? "";
            if (service == NativePaymentsConfig.GooglePlay)
            {
                GP_Logger.Warn("PAYMENTS",
                    "This APK was built without Unity IAP for Google Play. Enable Google Play payments for the store in GamePush, Save in Tools/GamePush, then rebuild.");
            }
            else if (service == NativePaymentsConfig.OneStore)
            {
                GP_Logger.Warn("PAYMENTS",
                    "This APK was built without the ONE store IAP plugin. Install it in Tools/GamePush → Android, Save, then rebuild.");
            }
            else if (service == NativePaymentsConfig.Xsolla || service == NativePaymentsConfig.Robokassa ||
                     service == NativePaymentsConfig.Stripe)
            {
                GP_Logger.Warn("PAYMENTS",
                    "This APK was built without the payments WebView. Enable Robokassa, Xsolla, or Stripe for the store in GamePush, Save in Tools/GamePush, then rebuild.");
            }
            else
            {
                GP_Logger.Warn("PAYMENTS", "Native payments are not wired for " + service + ".");
            }
        }
    }

    sealed class NativeUnavailablePaymentsSdk : INativePaymentsSdk
    {
        public static readonly NativeUnavailablePaymentsSdk Instance = new NativeUnavailablePaymentsSdk();
        public bool IsAvailable => false;
        public bool IsSubscriptionsAvailable => false;
        public void Initialize() { }

        public Task<List<FetchProducts>> MapProducts(List<FetchProducts> products)
        {
            return Task.FromResult(products ?? new List<FetchProducts>());
        }

        public Task Purchase(FetchProducts product)
        {
            NativePaymentsSdks.LogMissingOnce();
            return Task.FromException(new Exception("payments_not_available"));
        }

        public Task Consume(string idOrTag) => Task.CompletedTask;

        public Task Subscribe(FetchProducts product, FetchPlayerPurchases existing)
        {
            NativePaymentsSdks.LogMissingOnce();
            return Task.FromException(new Exception("subscriptions_not_available"));
        }

        public Task Unsubscribe(FetchProducts product, FetchPlayerPurchases existing)
        {
            NativePaymentsSdks.LogMissingOnce();
            return Task.FromException(new Exception("subscriptions_not_available"));
        }
    }

#if GP_BUILD_PAYMENTS_WEB && UNITY_ANDROID && !UNITY_EDITOR
    sealed class NativeWebCheckoutPaymentsSdk : INativePaymentsSdk
    {
        public static readonly NativeWebCheckoutPaymentsSdk Instance = new NativeWebCheckoutPaymentsSdk();
        const string RobokassaDonePrefix = "https://cdn.gamepush.com/pages/loader.html";

        public bool IsAvailable => true;
        public bool IsSubscriptionsAvailable => NativeCore.Payments.ActiveService == NativePaymentsConfig.Xsolla;
        public void Initialize() { }

        public Task<List<FetchProducts>> MapProducts(List<FetchProducts> products)
        {
            var list = products ?? new List<FetchProducts>();
            var lang = Application.systemLanguage == SystemLanguage.Russian ? "RU" : "EN";
            foreach (var product in list)
            {
                if (product == null)
                    continue;
                product.price = (float)Math.Round(product.price + 1e-10, 2, MidpointRounding.AwayFromZero);
                product.currencySymbol = RealCurrencySymbol(product.currency, lang, product.currencySymbol);
            }
            return Task.FromResult(list);
        }

        public Task Consume(string idOrTag) => Task.CompletedTask;

        public async Task Purchase(FetchProducts product)
        {
            await Checkout(product);
        }

        public async Task Subscribe(FetchProducts product, FetchPlayerPurchases existing)
        {
            if (!IsSubscriptionsAvailable)
                throw new Exception("subscriptions_not_available");
            if (existing != null && !existing.subscribed && IsFuture(existing.expiredAt))
            {
                await NativePaymentsService.ResumeSubscription(PurchaseKey(product), existing.payload);
                return;
            }

            await Checkout(product);
        }

        public async Task Unsubscribe(FetchProducts product, FetchPlayerPurchases existing)
        {
            if (!IsSubscriptionsAvailable)
                throw new Exception("subscriptions_not_available");
            await NativePaymentsService.CancelSubscription(PurchaseKey(product), existing != null ? existing.payload : "{}");
        }

        async Task Checkout(FetchProducts product)
        {
            if (product == null)
                throw new Exception("product_not_found");
            if (product.price <= 0f)
                throw new Exception("product_price_empty " + NativePayments.DescribeProduct(product));

            GP_Logger.Info("PAYMENTS", "Checkout GraphQL " + NativePayments.DescribeProduct(product) +
                                       " service=" + (NativeCore.Payments.ActiveService ?? ""));
            var created = await NativePaymentsService.Purchase(PurchaseKey(product));
            var purchase = created.purchase;
            var purchaseId = purchase != null ? purchase.purchaseId : "";
            var payload = purchase != null ? purchase.payload : "";
            var url = NativePaymentsService.CheckoutUrl(payload);
            GP_Logger.Info("PAYMENTS", "Checkout order idLen=" + (purchaseId ?? "").Length +
                                       " status=" + (purchase != null ? purchase.orderStatus : "") +
                                       " " + NativePaymentsService.DescribePayload(payload) +
                                       " " + NativePaymentsService.DescribeCheckoutUrl(url));
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(purchaseId))
                throw new Exception("purchase failed: " + NativePaymentsService.DescribePayload(payload));

            var prefix = UsesLoaderRedirect(NativeCore.Payments.ActiveService)
                ? RobokassaDonePrefix
                : "";

            using var cts = new System.Threading.CancellationTokenSource();
            NativeMainThread.Run(GP_Payments.FireOpen);
            Task<string> webTask;
            if (NativeCore.Payments.ActiveService == NativePaymentsConfig.Robokassa)
            {
                var html = NativePaymentsService.BuildRobokassaIframeHtml(url);
                if (string.IsNullOrEmpty(html))
                    throw new Exception("robokassa_params_empty " + NativePaymentsService.DescribeCheckoutUrl(url));
                GP_Logger.Info("PAYMENTS", "Open Robokassa iframe POST " +
                                           NativePaymentsService.DescribeCheckoutUrl(url));
                webTask = NativePaymentsWebView.OpenHtmlAsync(html, prefix);
            }
            else
            {
                GP_Logger.Info("PAYMENTS", "Open checkout WebView prefix=" +
                                           (string.IsNullOrEmpty(prefix) ? "none" : "loader"));
                webTask = NativePaymentsWebView.OpenAsync(url, prefix);
            }
            var pollTask = NativePaymentsService.WaitUntilPaid(purchaseId, null, 0, cts.Token, 0, 3000);

            var finished = await Task.WhenAny(webTask, pollTask);
            if (finished == pollTask && pollTask.Status == TaskStatus.RanToCompletion && pollTask.Result)
            {
                NativePaymentsWebView.Close();
                cts.Cancel();
                try { await webTask; } catch { }
                NativeMainThread.Run(GP_Payments.FireClose);
                GP_Logger.Info("PAYMENTS", "Checkout paid while WebView was open");
                return;
            }

            cts.Cancel();
            string webResult = "cancel";
            try { webResult = await webTask; } catch { }
            var paidInBackground = false;
            try { paidInBackground = await pollTask; } catch { }
            NativeMainThread.Run(GP_Payments.FireClose);
            GP_Logger.Info("PAYMENTS", "Checkout WebView result=" + webResult);
            if (paidInBackground)
            {
                GP_Logger.Info("PAYMENTS", "Checkout paid as WebView closed");
                return;
            }

            if (webResult == "done")
            {
                using var extra = new System.Threading.CancellationTokenSource();
                if (await NativePaymentsService.WaitUntilPaid(purchaseId, null, 0, extra.Token, 15, 5000))
                    return;
                if (await PurchaseAlreadyOnPlayer(purchaseId))
                    return;
                throw new Exception("purchase_timeout");
            }

            var paid = await NativePaymentsService.GetPlayerPurchase(purchaseId);
            if (paid == null || !NativePaymentsService.IsPaid(paid))
                throw new Exception("cancel");
        }

        static async Task<bool> PurchaseAlreadyOnPlayer(string purchaseId)
        {
            if (string.IsNullOrEmpty(purchaseId))
                return false;
            try
            {
                var (_, purchases) = await NativePaymentsService.Fetch();
                if (purchases == null)
                    return false;
                foreach (var item in purchases)
                {
                    if (item != null && item.purchaseId == purchaseId && NativePaymentsService.IsPaid(item))
                    {
                        GP_Logger.Info("PAYMENTS", "Checkout paid in player purchases");
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                GP_Logger.Warn("PAYMENTS", "Fetch after checkout: " + exception.Message);
            }
            return false;
        }

        static bool UsesLoaderRedirect(string service)
        {
            return service == NativePaymentsConfig.Robokassa || service == NativePaymentsConfig.Stripe;
        }

        static bool IsFuture(string iso)
        {
            if (string.IsNullOrEmpty(iso))
                return false;
            if (!DateTime.TryParse(iso, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expired))
                return false;
            var server = DateTime.UtcNow;
            if (DateTime.TryParse(NativeCore.ServerTime, null, System.Globalization.DateTimeStyles.RoundtripKind,
                    out var parsed))
                server = parsed.ToUniversalTime();
            return expired.ToUniversalTime() > server;
        }

        static string PurchaseKey(FetchProducts product)
        {
            if (product == null)
                return "";
            if (product.id > 0)
                return product.id.ToString();
            return product.tag ?? "";
        }

        static string RealCurrencySymbol(string currency, string lang, string fallback)
        {
            var code = (currency ?? "").Trim().ToUpperInvariant();
            var ru = lang == "RU";
            switch (code)
            {
                case "RUB":
                    return ru ? "РУБ" : "RUB";
                case "USD":
                    return ru ? "ДОЛ" : "USD";
                case "EUR":
                    return ru ? "ЕВРО" : "EUR";
                default:
                    return string.IsNullOrEmpty(fallback) ? currency ?? "" : fallback;
            }
        }
    }
#endif
}
