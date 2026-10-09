#if GP_BUILD_PAYMENTS_GOOGLE && GP_UNITY_PURCHASING && UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GamePush;
using UnityEngine;
using UnityEngine.Purchasing;

namespace GamePush.Native
{
    sealed class NativeGooglePlayPaymentsSdk : INativePaymentsSdk
    {
        public static readonly NativeGooglePlayPaymentsSdk Instance = new NativeGooglePlayPaymentsSdk();

        StoreController _store;
        bool _connecting;
        bool _connected;
        TaskCompletionSource<bool> _productsFetched;
        TaskCompletionSource<bool> _purchaseGate;
        string _pendingGooglePlayId = "";
        Exception _purchaseError;
        readonly HashSet<string> _handledTokens = new HashSet<string>();

        public bool IsAvailable => true;
        public bool IsSubscriptionsAvailable => true;

        public void Initialize()
        {
            if (_store != null)
                return;
            _store = UnityIAPServices.StoreController();
            _store.OnProductsFetched += OnProductsFetched;
            _store.OnProductsFetchFailed += OnProductsFetchFailed;
            _store.OnPurchasePending += OnPurchasePending;
            _store.OnPurchaseFailed += OnPurchaseFailed;
            _store.OnPurchasesFetched += OnPurchasesFetched;
            _store.ProcessPendingOrdersOnPurchasesFetched(false);
        }

        public async Task<List<FetchProducts>> MapProducts(List<FetchProducts> products)
        {
            var list = products ?? new List<FetchProducts>();
            await EnsureConnected(list);
            foreach (var product in list)
            {
                if (product == null || string.IsNullOrEmpty(product.googlePlayId))
                    continue;
                var storeProduct = _store.GetProductById(product.googlePlayId);
                if (storeProduct == null || storeProduct.metadata == null)
                    continue;
                var meta = storeProduct.metadata;
                if (meta.localizedPrice > 0)
                    product.price = (float)meta.localizedPrice;
                if (!string.IsNullOrEmpty(meta.isoCurrencyCode))
                {
                    product.currency = meta.isoCurrencyCode;
                    product.currencySymbol = meta.isoCurrencyCode;
                }
            }
            return list;
        }

        public async Task Purchase(FetchProducts product)
        {
            if (product == null || string.IsNullOrEmpty(product.googlePlayId))
                throw new Exception("product_not_found");
            await EnsureConnected(NativePayments.GetProducts());
            _pendingGooglePlayId = product.googlePlayId;
            _purchaseError = null;
            _purchaseGate = new TaskCompletionSource<bool>();
            _store.PurchaseProduct(product.googlePlayId);
            var ok = await _purchaseGate.Task;
            _pendingGooglePlayId = "";
            _purchaseGate = null;
            if (!ok)
                throw _purchaseError ?? new Exception("purchase failed");
        }

        public Task Consume(string idOrTag) => Task.CompletedTask;

        public Task Subscribe(FetchProducts product, FetchPlayerPurchases existing)
        {
            return Purchase(product);
        }

        public Task Unsubscribe(FetchProducts product, FetchPlayerPurchases existing)
        {
            return Task.CompletedTask;
        }

        async Task EnsureConnected(List<FetchProducts> products)
        {
            Initialize();
            if (!_connected)
            {
                if (!_connecting)
                {
                    _connecting = true;
                    await _store.Connect();
                    _connected = true;
                    _connecting = false;
                }
                else
                {
                    while (!_connected)
                        await Task.Yield();
                }
            }

            var definitions = new List<ProductDefinition>();
            var seen = new HashSet<string>();
            if (products != null)
            {
                foreach (var product in products)
                {
                    if (product == null || string.IsNullOrEmpty(product.googlePlayId) || !seen.Add(product.googlePlayId))
                        continue;
                    var type = product.isSubscription ? ProductType.Subscription : ProductType.Consumable;
                    definitions.Add(new ProductDefinition(product.googlePlayId, type));
                }
            }

            if (definitions.Count == 0)
                return;

            _productsFetched = new TaskCompletionSource<bool>();
            _store.FetchProducts(definitions);
            await _productsFetched.Task;
            _store.FetchPurchases();
        }

        void OnProductsFetched(List<Product> products)
        {
            _productsFetched?.TrySetResult(true);
        }

        void OnProductsFetchFailed(ProductFetchFailed failed)
        {
            _productsFetched?.TrySetException(new Exception(failed != null ? failed.ToString() : "fetch_products"));
        }

        void OnPurchaseFailed(FailedOrder order)
        {
            if (_purchaseGate == null)
                return;
            _purchaseError = new Exception(order != null ? order.FailureReason.ToString() : "purchase failed");
            _purchaseGate.TrySetResult(false);
        }

        void OnPurchasePending(PendingOrder order)
        {
            _ = Fulfill(order, false);
        }

        void OnPurchasesFetched(Orders orders)
        {
            if (orders == null || orders.PendingOrders == null)
                return;
            foreach (var pending in orders.PendingOrders)
                _ = Fulfill(pending, true);
        }

        async Task Fulfill(PendingOrder order, bool restore)
        {
            if (order == null)
                return;
            string googlePlayId = "";
            try
            {
                var info = Extract(order, out googlePlayId);
                if (string.IsNullOrEmpty(info.Token))
                    throw new Exception("empty purchase token");
                if (!_handledTokens.Add(info.Token) && restore)
                    return;

                await NativePaymentsService.ValidateGooglePlay(BuildValidatorBody(info));
                var gpProduct = FindByGooglePlayId(googlePlayId);
                var payload = new Dictionary<string, object>
                {
                    ["productId"] = googlePlayId,
                    ["purchaseToken"] = info.Token,
                    ["transactionId"] = info.TransactionId ?? "",
                    ["purchaseTime"] = info.PurchaseTime
                };
                var paid = await NativePaymentsService.WaitUntilPaid("", payload,
                    gpProduct != null ? gpProduct.id : 0,
                    System.Threading.CancellationToken.None, 15, 3000);
                if (!paid)
                    throw new Exception("purchase_timeout");
                _store.ConfirmPurchase(order);

                if (!restore && googlePlayId == _pendingGooglePlayId)
                    _purchaseGate?.TrySetResult(true);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[GamePush Native] Google Play fulfill failed: " + exception.Message);
                if (!restore && (googlePlayId == _pendingGooglePlayId || string.IsNullOrEmpty(_pendingGooglePlayId)))
                {
                    _purchaseError = exception;
                    _purchaseGate?.TrySetResult(false);
                }
            }
        }

        struct ReceiptInfo
        {
            public string Token;
            public string TransactionId;
            public string Receipt;
            public string Signature;
            public long PurchaseTime;
        }

        ReceiptInfo Extract(PendingOrder order, out string googlePlayId)
        {
            googlePlayId = "";
            var info = new ReceiptInfo { PurchaseTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
            try
            {
                foreach (var item in order.CartOrdered.Items())
                {
                    if (item == null)
                        continue;
                    if (!string.IsNullOrEmpty(item.CatalogListingId))
                        googlePlayId = item.CatalogListingId;
                    else if (item.Product != null && item.Product.definition != null)
                        googlePlayId = item.Product.definition.id;
                    break;
                }
            }
            catch
            {
            }

            info.Receipt = order.Info != null ? order.Info.Receipt : "";
            if (order.Info != null && !string.IsNullOrEmpty(order.Info.TransactionID))
                info.TransactionId = order.Info.TransactionID;
            ParseUnityReceipt(info.Receipt, ref info, ref googlePlayId);
            if (string.IsNullOrEmpty(info.TransactionId))
                info.TransactionId = googlePlayId;
            return info;
        }

        static void ParseUnityReceipt(string receipt, ref ReceiptInfo info, ref string googlePlayId)
        {
            if (string.IsNullOrEmpty(receipt))
                return;
            var payload = GpJson.GetObject(receipt, "Payload");
            if (string.IsNullOrEmpty(payload) && GpJson.TryGetString(receipt, "Payload", out var payloadText))
                payload = Unquote(payloadText);
            if (string.IsNullOrEmpty(payload))
                payload = receipt;

            var innerJson = GpJson.GetObject(payload, "json");
            if (string.IsNullOrEmpty(innerJson) && GpJson.TryGetString(payload, "json", out var jsonText))
                innerJson = Unquote(jsonText);
            var source = string.IsNullOrEmpty(innerJson) ? payload : innerJson;

            if (GpJson.TryGetString(source, "purchaseToken", out var token) && !string.IsNullOrEmpty(token))
                info.Token = token;
            if (GpJson.TryGetString(source, "orderId", out var orderId) && !string.IsNullOrEmpty(orderId))
                info.TransactionId = orderId;
            if (string.IsNullOrEmpty(googlePlayId) && GpJson.TryGetString(source, "productId", out var pid))
                googlePlayId = pid ?? "";
            var purchaseTime = GpJson.GetLong(source, "purchaseTime");
            if (purchaseTime > 0)
                info.PurchaseTime = purchaseTime;
            if (GpJson.TryGetString(payload, "signature", out var signature))
                info.Signature = signature ?? "";
            info.Receipt = source;
        }

        static string Unquote(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 2 || value[0] != '"')
                return value;
            return value.Substring(1, value.Length - 2).Replace("\\\"", "\"").Replace("\\\\", "\\");
        }

        static string BuildValidatorBody(ReceiptInfo info)
        {
            var sb = new StringBuilder();
            sb.Append("{\"id\":");
            sb.Append(GpJson.Quote(info.TransactionId ?? ""));
            sb.Append(",\"type\":\"android-playstore\",\"transaction\":{");
            sb.Append("\"type\":\"android-playstore\",\"id\":");
            sb.Append(GpJson.Quote(info.TransactionId ?? ""));
            sb.Append(",\"purchaseToken\":");
            sb.Append(GpJson.Quote(info.Token ?? ""));
            sb.Append(",\"receipt\":");
            sb.Append(GpJson.Quote(info.Receipt ?? ""));
            sb.Append(",\"signature\":");
            sb.Append(GpJson.Quote(info.Signature ?? ""));
            sb.Append("}}");
            return sb.ToString();
        }

        static FetchProducts FindByGooglePlayId(string googlePlayId)
        {
            if (string.IsNullOrEmpty(googlePlayId))
                return null;
            foreach (var product in NativePayments.GetProducts())
            {
                if (product != null && product.googlePlayId == googlePlayId)
                    return product;
            }
            return null;
        }
    }
}
#endif
