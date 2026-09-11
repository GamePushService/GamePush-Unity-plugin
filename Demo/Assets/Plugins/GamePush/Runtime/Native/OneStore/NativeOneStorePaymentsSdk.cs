#if GP_ONESTORE_IAP && UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using GamePush;
using OneStore.Purchasing;
using OneStore.Purchasing.Internal;
using UnityEngine;

namespace GamePush.Native
{
    sealed class NativeOneStorePaymentsSdk : INativePaymentsSdk, IPurchaseCallback
    {
        public static readonly NativeOneStorePaymentsSdk Instance = new NativeOneStorePaymentsSdk();

        PurchaseClientImpl _client;
        TaskCompletionSource<List<ProductDetail>> _productsGate;
        TaskCompletionSource<PurchaseData> _purchaseGate;
        TaskCompletionSource<bool> _consumeGate;
        string _pendingProductId = "";
        Exception _pendingError;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            NativePaymentsSdks.RegisterOneStore(Instance);
        }

        public bool IsAvailable => true;
        public bool IsSubscriptionsAvailable => false;

        public void Initialize()
        {
            if (_client != null)
                return;
            _client = new PurchaseClientImpl(NativeCore.Payments.PublicKey ?? "");
            _client.Initialize(this);
        }

        public async Task<List<FetchProducts>> MapProducts(List<FetchProducts> products)
        {
            var list = products ?? new List<FetchProducts>();
            var ids = new List<string>();
            var seen = new HashSet<string>();
            foreach (var product in list)
            {
                if (product == null || string.IsNullOrEmpty(product.onestoreId) || !seen.Add(product.onestoreId))
                    continue;
                ids.Add(product.onestoreId);
            }

            if (ids.Count == 0)
                return list;

            Initialize();
            _productsGate = new TaskCompletionSource<List<ProductDetail>>();
            await RunOnMain(() => _client.QueryProductDetails(ids.AsReadOnly(), ProductType.ALL));
            List<ProductDetail> details;
            try
            {
                details = await _productsGate.Task;
            }
            catch
            {
                ApplyKrwFallback(list);
                return list;
            }
            finally
            {
                _productsGate = null;
            }

            if (details == null || details.Count == 0)
            {
                ApplyKrwFallback(list);
                return list;
            }

            foreach (var product in list)
            {
                if (product == null || string.IsNullOrEmpty(product.onestoreId))
                    continue;
                ProductDetail match = null;
                foreach (var detail in details)
                {
                    if (detail != null && detail.productId == product.onestoreId)
                    {
                        match = detail;
                        break;
                    }
                }
                if (match == null)
                    continue;
                if (match.priceAmountMicros > 0)
                    product.price = match.priceAmountMicros / 1_000_000f;
                else if (TryParsePrice(match.price, out var parsed))
                    product.price = parsed;
                var currency = string.IsNullOrEmpty(match.priceCurrencyCode) ? "KRW" : match.priceCurrencyCode;
                product.currency = currency;
                product.currencySymbol = currency;
            }

            return list;
        }

        public async Task Purchase(FetchProducts product)
        {
            if (product == null || string.IsNullOrEmpty(product.onestoreId))
                throw new Exception("product_not_found");

            Initialize();
            var idOrTag = IdOrTag(product);
            var created = await NativePaymentsService.Purchase(idOrTag);
            var signature = SignatureOf(created.purchase != null ? created.purchase.payload : "");
            if (string.IsNullOrEmpty(signature))
                throw new Exception("purchase failed");

            var type = product.isSubscription ? ProductType.SUBS : ProductType.INAPP;
            var flow = new PurchaseFlowParams.Builder()
                .SetProductId(product.onestoreId)
                .SetProductType(type)
                .SetDeveloperPayload(signature)
                .Build();

            _pendingProductId = product.onestoreId;
            _pendingError = null;
            _purchaseGate = new TaskCompletionSource<PurchaseData>();
            await RunOnMain(() => _client.Purchase(flow));
            PurchaseData purchased;
            try
            {
                purchased = await _purchaseGate.Task;
            }
            finally
            {
                _pendingProductId = "";
                _purchaseGate = null;
            }

            if (purchased == null)
                throw _pendingError ?? new Exception("purchase failed");

            var receipt = purchased.JsonReceipt ?? "";
            GpJson.TryGetString(receipt, "json", out var purchaseData);
            GpJson.TryGetString(receipt, "signature", out var storeSignature);
            var payload = new Dictionary<string, object>
            {
                ["productId"] = purchased.ProductId ?? "",
#pragma warning disable 0618
                ["purchaseId"] = purchased.PurchaseId ?? "",
#pragma warning restore 0618
                ["purchaseToken"] = purchased.PurchaseToken ?? "",
                ["orderId"] = purchased.OrderId ?? "",
                ["developerPayload"] = purchased.DeveloperPayload ?? "",
                ["purchaseTime"] = purchased.PurchaseTime,
                ["purchaseData"] = purchaseData ?? "",
                ["signature"] = storeSignature ?? ""
            };
            await NativePaymentsService.Purchase(idOrTag, payload);
        }

        public async Task Consume(string idOrTag)
        {
            var purchase = NativePayments.FindPurchase(idOrTag);
            var payload = purchase != null ? purchase.payload : "";
            if (string.IsNullOrEmpty(payload))
                return;
            if (!GpJson.TryGetString(payload, "purchaseData", out var json) || string.IsNullOrEmpty(json))
                return;
            GpJson.TryGetString(payload, "signature", out var signature);
            if (!PurchaseData.FromJson(json, signature ?? "", out var data) || data == null)
                throw new Exception("consume failed");

            Initialize();
            _consumeGate = new TaskCompletionSource<bool>();
            _pendingError = null;
            await RunOnMain(() => _client.ConsumePurchase(data));
            try
            {
                var ok = await _consumeGate.Task;
                if (!ok)
                    throw _pendingError ?? new Exception("consume failed");
            }
            finally
            {
                _consumeGate = null;
            }
        }

        public Task Subscribe(FetchProducts product, FetchPlayerPurchases existing)
        {
            return Task.FromException(new Exception("subscriptions_not_available"));
        }

        public Task Unsubscribe(FetchProducts product, FetchPlayerPurchases existing)
        {
            return Task.FromException(new Exception("subscriptions_not_available"));
        }

        public void OnSetupFailed(IapResult iapResult)
        {
            FailPending(ResultText(iapResult));
        }

        public void OnProductDetailsSucceeded(List<ProductDetail> productDetails)
        {
            _productsGate?.TrySetResult(productDetails ?? new List<ProductDetail>());
        }

        public void OnProductDetailsFailed(IapResult iapResult)
        {
            _productsGate?.TrySetException(new Exception(ResultText(iapResult)));
        }

        public void OnPurchaseSucceeded(List<PurchaseData> purchases)
        {
            if (_purchaseGate == null)
                return;
            PurchaseData match = null;
            if (purchases != null)
            {
                foreach (var item in purchases)
                {
                    if (item == null)
                        continue;
                    if (string.IsNullOrEmpty(_pendingProductId) || item.ProductId == _pendingProductId)
                    {
                        match = item;
                        break;
                    }
                }
            }
            if (match == null)
            {
                _purchaseGate.TrySetException(new Exception("purchase failed"));
                return;
            }
            _purchaseGate.TrySetResult(match);
        }

        public void OnPurchaseFailed(IapResult iapResult)
        {
            _pendingError = new Exception(ResultText(iapResult));
            _purchaseGate?.TrySetException(_pendingError);
        }

        public void OnConsumeSucceeded(PurchaseData purchase)
        {
            _consumeGate?.TrySetResult(true);
        }

        public void OnConsumeFailed(IapResult iapResult)
        {
            _pendingError = new Exception(ResultText(iapResult));
            _consumeGate?.TrySetResult(false);
        }

        public void OnAcknowledgeSucceeded(PurchaseData purchase, ProductType type) { }

        public void OnAcknowledgeFailed(IapResult iapResult) { }

        public void OnManageRecurringProduct(IapResult iapResult, PurchaseData purchase, RecurringAction action) { }

        public void OnNeedUpdate()
        {
            FailPending("need_update");
        }

        public void OnNeedLogin()
        {
            FailPending("need_login");
        }

        void FailPending(string message)
        {
            var error = new Exception(string.IsNullOrEmpty(message) ? "iap_error" : message);
            _pendingError = error;
            _productsGate?.TrySetException(error);
            _purchaseGate?.TrySetException(error);
            _consumeGate?.TrySetResult(false);
        }

        static string SignatureOf(string payload)
        {
            if (string.IsNullOrEmpty(payload))
                return "";
            return GpJson.TryGetString(payload, "signature", out var signature) ? signature ?? "" : "";
        }

        static string IdOrTag(FetchProducts product)
        {
            if (product == null)
                return "";
            return string.IsNullOrEmpty(product.tag) ? product.id.ToString() : product.tag;
        }

        static void ApplyKrwFallback(List<FetchProducts> products)
        {
            if (products == null)
                return;
            foreach (var product in products)
            {
                if (product == null || string.IsNullOrEmpty(product.onestoreId))
                    continue;
                if (string.IsNullOrEmpty(product.currency))
                    product.currency = "KRW";
                if (string.IsNullOrEmpty(product.currencySymbol))
                    product.currencySymbol = "KRW";
            }
        }

        static bool TryParsePrice(string raw, out float price)
        {
            price = 0;
            if (string.IsNullOrEmpty(raw))
                return false;
            var filtered = "";
            foreach (var ch in raw)
            {
                if (char.IsDigit(ch) || ch == '.' || ch == ',')
                    filtered += ch == ',' ? '.' : ch;
            }
            return float.TryParse(filtered, NumberStyles.Float, CultureInfo.InvariantCulture, out price);
        }

        static string ResultText(IapResult result)
        {
            return result != null ? result.ToString() : "iap_error";
        }

        static Task RunOnMain(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();
            NativeMainThread.Run(() =>
            {
                try
                {
                    action();
                    tcs.TrySetResult(true);
                }
                catch (Exception exception)
                {
                    tcs.TrySetException(exception);
                }
            });
            return tcs.Task;
        }
    }
}
#endif
