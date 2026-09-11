using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePush;

namespace GamePush.Native
{
    public static class NativePayments
    {
        static bool _busy;

        public static bool IsAvailable =>
            NativeCore.Payments.HasPayments && NativePaymentsSdks.Current.IsAvailable;

        public static bool IsSubscriptionsAvailable =>
            IsAvailable && NativeCore.Payments.SupportsSubscriptions &&
            NativePaymentsSdks.Current.IsSubscriptionsAvailable;

        public static List<FetchProducts> GetProducts()
        {
            return GP_Payments.Products ?? new List<FetchProducts>();
        }

        public static List<FetchPlayerPurchases> GetPurchases()
        {
            return GP_Payments.Purchases ?? new List<FetchPlayerPurchases>();
        }

        public static bool Has(string idOrTag)
        {
            foreach (var purchase in GetPurchases())
            {
                if (NativePaymentsService.MatchesPurchase(purchase, idOrTag))
                    return true;
            }
            return false;
        }

        public static FetchProducts FindProduct(string idOrTag)
        {
            foreach (var product in GetProducts())
            {
                if (NativePaymentsService.Matches(product, idOrTag))
                    return product;
            }
            return null;
        }

        public static FetchPlayerPurchases FindPurchase(string idOrTag)
        {
            foreach (var purchase in GetPurchases())
            {
                if (NativePaymentsService.MatchesPurchase(purchase, idOrTag))
                    return purchase;
            }
            return null;
        }

        public static void Fetch()
        {
            _ = FetchAsync();
        }

        public static void Purchase(string idOrTag)
        {
            _ = PurchaseAsync(idOrTag);
        }

        public static void Consume(string idOrTag)
        {
            _ = ConsumeAsync(idOrTag);
        }

        public static void Subscribe(string idOrTag)
        {
            _ = SubscribeAsync(idOrTag);
        }

        public static void Unsubscribe(string idOrTag)
        {
            _ = UnsubscribeAsync(idOrTag);
        }

        static async Task FetchAsync()
        {
            try
            {
                if (NativeCore.Client == null)
                {
                    GP_Logger.Warn("PAYMENTS", "Fetch skipped: GraphQL client is not ready");
                    NativeMainThread.Run(GP_Payments.FireFetchProductsError);
                    return;
                }

                var (fetchedProducts, purchases) = await NativePaymentsService.Fetch();
                var products = PreferCatalog(fetchedProducts);
                products = await NativePaymentsSdks.Current.MapProducts(products);
                GP_Logger.Info("PAYMENTS", (NativeCore.Catalog != null && NativeCore.Catalog.Count > 0
                                   ? "catalog source=config "
                                   : "catalog source=fetch ") +
                               DescribeCatalog(products, purchases));
                NativeMainThread.Run(() =>
                {
                    GP_Payments.Products = products;
                    GP_Payments.Purchases = purchases;
                    GP_Payments.FireFetchProducts(products);
                    GP_Payments.FireFetchPlayerPurchases(purchases);
                });
            }
            catch (Exception exception)
            {
                GP_Logger.Error("PAYMENTS", "Fetch failed: " + exception.Message);
                NativeMainThread.Run(GP_Payments.FireFetchProductsError);
            }
        }

        static async Task PurchaseAsync(string idOrTag)
        {
            GP_Logger.Info("PAYMENTS", "Purchase click tag=" + (idOrTag ?? "") +
                                        " busy=" + _busy +
                                        " service=" + (NativeCore.Payments.ActiveService ?? "") +
                                        " sdk=" + NativePaymentsSdks.Current.GetType().Name);
            if (!Begin())
            {
                NativeMainThread.Run(GP_Payments.FirePurchaseError);
                return;
            }

            try
            {
                var product = await EnsureProduct(idOrTag);
                if (product == null)
                {
                    GP_Logger.Warn("PAYMENTS", "Product not found for tag=" + (idOrTag ?? "") +
                                               " catalog=" + DescribeCatalog(GetProducts(), GetPurchases()));
                    NativeMainThread.Run(GP_Payments.FirePurchaseError);
                    return;
                }

                GP_Logger.Info("PAYMENTS", "Purchase start " + DescribeProduct(product));
                await NativePaymentsSdks.Current.Purchase(product);
                await RefreshPurchases();
                NativeMainThread.Run(() => GP_Payments.FirePurchaseSuccess(IdOrTag(product, idOrTag)));
                GP_Logger.Info("PAYMENTS", "Purchase success tag=" + IdOrTag(product, idOrTag));
            }
            catch (Exception exception)
            {
                GP_Logger.Error("PAYMENTS", "Purchase failed: " + exception.Message);
                NativeMainThread.Run(GP_Payments.FirePurchaseError);
            }
            finally
            {
                _busy = false;
            }
        }

        static async Task ConsumeAsync(string idOrTag)
        {
            if (!Begin())
            {
                NativeMainThread.Run(GP_Payments.FireConsumeError);
                return;
            }

            try
            {
                await NativePaymentsSdks.Current.Consume(idOrTag);
                var (_, purchase) = await NativePaymentsService.Consume(idOrTag);
                RemovePurchase(idOrTag, purchase);
                NativeMainThread.Run(() => GP_Payments.FireConsumeSuccess(idOrTag));
            }
            catch (Exception exception)
            {
                GP_Logger.Error("PAYMENTS", "Consume failed: " + exception.Message);
                NativeMainThread.Run(GP_Payments.FireConsumeError);
            }
            finally
            {
                _busy = false;
            }
        }

        static async Task SubscribeAsync(string idOrTag)
        {
            if (!IsSubscriptionsAvailable)
            {
                NativeMainThread.Run(GP_Payments.FireSubscribeError);
                return;
            }

            if (!Begin())
            {
                NativeMainThread.Run(GP_Payments.FireSubscribeError);
                return;
            }

            try
            {
                var product = await EnsureProduct(idOrTag);
                if (product == null)
                {
                    NativeMainThread.Run(GP_Payments.FireSubscribeError);
                    return;
                }

                await NativePaymentsSdks.Current.Subscribe(product, FindPurchase(idOrTag));
                await RefreshPurchases();
                NativeMainThread.Run(() => GP_Payments.FireSubscribeSuccess(IdOrTag(product, idOrTag)));
            }
            catch (Exception exception)
            {
                GP_Logger.Error("PAYMENTS", "Subscribe failed: " + exception.Message);
                NativeMainThread.Run(GP_Payments.FireSubscribeError);
            }
            finally
            {
                _busy = false;
            }
        }

        static async Task UnsubscribeAsync(string idOrTag)
        {
            if (!IsSubscriptionsAvailable)
            {
                NativeMainThread.Run(GP_Payments.FireUnsubscribeError);
                return;
            }

            if (!Begin())
            {
                NativeMainThread.Run(GP_Payments.FireUnsubscribeError);
                return;
            }

            try
            {
                var product = FindProduct(idOrTag);
                var purchase = FindPurchase(idOrTag);
                if (product == null || purchase == null)
                {
                    NativeMainThread.Run(GP_Payments.FireUnsubscribeError);
                    return;
                }

                await NativePaymentsSdks.Current.Unsubscribe(product, purchase);
                purchase.subscribed = false;
                NativeMainThread.Run(() => GP_Payments.FireUnsubscribeSuccess(IdOrTag(product, idOrTag)));
            }
            catch (Exception exception)
            {
                GP_Logger.Error("PAYMENTS", "Unsubscribe failed: " + exception.Message);
                NativeMainThread.Run(GP_Payments.FireUnsubscribeError);
            }
            finally
            {
                _busy = false;
            }
        }

        static bool Begin()
        {
            if (_busy)
            {
                GP_Logger.Warn("PAYMENTS", "Purchase ignored: another payment is already in progress");
                return false;
            }
            if (!NativeCore.Payments.HasPayments)
            {
                GP_Logger.Warn("PAYMENTS", "Payments are not connected for this platform impl=" +
                                           (NativeCore.Payments.Implementation ?? "") +
                                           " service=" + (NativeCore.Payments.ActiveService ?? ""));
                return false;
            }
            if (!NativePaymentsSdks.Current.IsAvailable)
            {
                NativePaymentsSdks.LogMissingOnce();
                return false;
            }
            _busy = true;
            return true;
        }

        static List<FetchProducts> PreferCatalog(List<FetchProducts> fetched)
        {
            if (NativeCore.Catalog != null && NativeCore.Catalog.Count > 0)
                return NativeCore.Catalog;
            return fetched ?? new List<FetchProducts>();
        }

        static async Task<FetchProducts> EnsureProduct(string idOrTag)
        {
            var product = FindProduct(idOrTag);
            if (product != null)
                return product;
            var (fetchedProducts, purchases) = await NativePaymentsService.Fetch();
            var products = PreferCatalog(fetchedProducts);
            products = await NativePaymentsSdks.Current.MapProducts(products);
            GP_Payments.Products = products;
            GP_Payments.Purchases = purchases;
            return FindProduct(idOrTag);
        }

        static async Task RefreshPurchases()
        {
            try
            {
                var (fetchedProducts, purchases) = await NativePaymentsService.Fetch();
                var products = PreferCatalog(fetchedProducts);
                products = await NativePaymentsSdks.Current.MapProducts(products);
                GP_Payments.Products = products;
                GP_Payments.Purchases = purchases;
            }
            catch (Exception exception)
            {
                GP_Logger.Warn("PAYMENTS", "Refresh failed: " + exception.Message);
            }
        }

        static void RemovePurchase(string idOrTag, FetchPlayerPurchases consumed)
        {
            var list = GP_Payments.Purchases;
            if (list == null)
                return;
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (NativePaymentsService.MatchesPurchase(list[i], idOrTag) ||
                    (consumed != null && list[i].purchaseId == consumed.purchaseId))
                    list.RemoveAt(i);
            }
        }

        static string IdOrTag(FetchProducts product, string fallback)
        {
            if (product == null)
                return fallback;
            return string.IsNullOrEmpty(product.tag) ? product.id.ToString() : product.tag;
        }

        static string DescribeCatalog(List<FetchProducts> products, List<FetchPlayerPurchases> purchases)
        {
            var n = products != null ? products.Count : 0;
            var p = purchases != null ? purchases.Count : 0;
            var sb = new System.Text.StringBuilder();
            sb.Append("catalog n=").Append(n).Append(" owned=").Append(p)
                .Append(" platform=").Append(NativeCore.PlatformType ?? "")
                .Append(" service=").Append(NativeCore.Payments.ActiveService ?? "");
            if (products != null)
            {
                var shown = 0;
                foreach (var product in products)
                {
                    if (product == null)
                        continue;
                    sb.Append(" | ").Append(DescribeProduct(product));
                    shown++;
                    if (shown >= 6)
                        break;
                }
            }
            return sb.ToString();
        }

        internal static string DescribeProduct(FetchProducts product)
        {
            if (product == null)
                return "product=null";
            return "id=" + product.id + " tag=" + (product.tag ?? "") +
                   " price=" + product.price.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                   " currency=" + (product.currency ?? "") +
                   " symbol=" + (product.currencySymbol ?? "");
        }
    }
}
