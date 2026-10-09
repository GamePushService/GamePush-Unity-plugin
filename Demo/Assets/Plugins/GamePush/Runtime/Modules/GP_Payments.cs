using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Utilities;
using GamePush.Native;

namespace GamePush
{
    public class GP_Payments : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Payments);

        public static List<FetchProducts> Products = new List<FetchProducts>();
        public static List<FetchPlayerPurchases> Purchases = new List<FetchPlayerPurchases>();
        public string CurrencySymbol() => Products[0]?.ToString();
        
        #region Events
        public static event UnityAction<List<FetchProducts>> OnFetchProducts;
        public static event UnityAction OnFetchProductsError;

        public static event UnityAction<List<FetchPlayerPurchases>> OnFetchPlayerPurchases;

        public static event UnityAction<string> OnPurchaseSuccess;
        public static event UnityAction OnPurchaseError;
        public static event UnityAction<string> OnConsumeSuccess;
        public static event UnityAction OnConsumeError;

        public static event UnityAction<string> OnSubscribeSuccess;
        public static event UnityAction OnSubscribeError;
        public static event UnityAction<string> OnUnsubscribeSuccess;
        public static event UnityAction OnUnsubscribeError;
        public static event UnityAction OnOpen;
        public static event UnityAction OnClose;
        
        private static event Action<List<FetchProducts>> _onFetchProducts;
        private static event Action _onFetchProductsError;

        private static event Action<List<FetchPlayerPurchases>> _onFetchPlayerPurchases;

        private static event Action<string> _onPurchaseSuccess;
        private static event Action _onPurchaseError;

        private static event Action<string> _onConsumeSuccess;
        private static event Action _onConsumeError;

        private static event Action<string> _onSubscribeSuccess;
        private static event Action _onSubscribeError;

        private static event Action<string> _onUnsubscribeSuccess;
        private static event Action _onUnsubscribeError;
        private static event Action _onOpen;
        private static event Action _onClose;
        #endregion

        #region DLL Import
#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Payments_FetchProducts();
        [DllImport("__Internal")]
        private static extern void GP_Payments_Purchase(string idOrTag);
        [DllImport("__Internal")]
        private static extern void GP_Payments_Consume(string idOrTag);
        [DllImport("__Internal")]
        private static extern string GP_Payments_IsAvailable();
        [DllImport("__Internal")]
        private static extern string GP_Payments_Products();
        [DllImport("__Internal")]
        private static extern string GP_Payments_Purchases();
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Payments_Has(string idOrTag);
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Payments_IsSubscriptionsAvailable();
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Payments_Subscribe(string idOrTag);
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Payments_Unsubscribe(string idOrTag);
        #endif
#endif
        #endregion
        
        #region Callbacks

        private void CallPaymentsFetchProducts(string data)
        {
            Products = ParseList<FetchProducts>(data);
            _onFetchProducts?.Invoke(Products);
            OnFetchProducts?.Invoke(Products);
        }
        private void CallPaymentsFetchPlayerPurchases(string data)
        { 
            Purchases = ParseList<FetchPlayerPurchases>(data);
            _onFetchPlayerPurchases?.Invoke(Purchases);
            OnFetchPlayerPurchases?.Invoke(Purchases);
        }

        private void CallPaymentsFetchProductsError()
        {
            _onFetchProductsError?.Invoke();
            OnFetchProductsError?.Invoke();
        }

        private void CallPaymentsPurchase(string PuchasedIdOrTag)
        {
            _onPurchaseSuccess?.Invoke(PuchasedIdOrTag);
            OnPurchaseSuccess?.Invoke(PuchasedIdOrTag);
        }
        private void CallPaymentsPurchaseError()
        {
            _onPurchaseError?.Invoke();
            OnPurchaseError?.Invoke();
        }

        private void CallPaymentsConsume(string idOrTag)
        {
            _onConsumeSuccess?.Invoke(idOrTag);
            OnConsumeSuccess?.Invoke(idOrTag);
        }
        private void CallPaymentsConsumeError()
        {
            _onConsumeError?.Invoke();
            OnConsumeError?.Invoke();
        }

        private void CallPaymentsSubscribeSuccess(string idOrTag)
        {
            OnSubscribeSuccess?.Invoke(idOrTag);
            _onSubscribeSuccess?.Invoke(idOrTag);
        }
        private void CallPaymentsSubscribeError()
        {
            _onSubscribeError?.Invoke();
            OnSubscribeError?.Invoke();
        }

        private void CallPaymentsUnsubscribeSuccess(string idOrTag)
        {
            _onUnsubscribeSuccess?.Invoke(idOrTag);
            OnUnsubscribeSuccess?.Invoke(idOrTag);
        }
        private void CallPaymentsUnsubscribeError()
        {
            _onUnsubscribeError?.Invoke();
            OnUnsubscribeError?.Invoke();
        }

        private void CallPaymentsOpen()
        {
            _onOpen?.Invoke();
            OnOpen?.Invoke();
        }

        private void CallPaymentsClose()
        {
            _onClose?.Invoke();
            OnClose?.Invoke();
        }
        #endregion

        public static void Open(Action onOpen = null, Action onClose = null)
        {
            _onOpen = onOpen;
            _onClose = onClose;
        }

        private async void Start()
        {
            await GP_Init.Ready;
            if (GamePushHost.UseNativeCore)
            {
                NativePayments.Fetch();
                return;
            }
            GetProducts();
            GetPurchases();
        }

        public static List<FetchProducts> GetProducts()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            Products = ParseList<FetchProducts>(GP_Payments_Products());
#else
            if (GamePushHost.UseNativeCore)
            {
                ConsoleLog("PRODUCTS: " + Products.Count);
                return Products;
            }
            Products = GP_Settings.instance.GetProducts();
            ConsoleLog("PRODUCTS: " + Products.Count);
#endif
            return Products;
        }

        public static List<FetchPlayerPurchases> GetPurchases()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            Purchases = ParseList<FetchPlayerPurchases>(GP_Payments_Purchases());
#else
            if (GamePushHost.UseNativeCore)
            {
                ConsoleLog("PURCHASES: " + Purchases.Count);
                return Purchases;
            }
#if UNITY_EDITOR
            if (GP_PaymentsStubSession.Enabled)
            {
                Purchases = GP_PaymentsStubSession.GetPurchases();
                ConsoleLog("PURCHASES: " + Purchases.Count);
                return Purchases;
            }
#endif
            Purchases = GP_Settings.instance.GetPlayerPurchases();
            ConsoleLog("PURCHASES: " + Purchases.Count);
#endif
            return Purchases;
        }

        public static bool Has(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Payments_Has(idOrTag) == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativePayments.Has(idOrTag);
            bool isVal = HasCachedPurchase(idOrTag);
            ConsoleLog("HAS: " + idOrTag + " : " + isVal);
            return isVal;
#endif
        }

        public static void Fetch(Action<List<FetchProducts>> onFetchProducts = null, Action onFetchProductsError = null, Action<List<FetchPlayerPurchases>> onFetchPlayerPurchases = null)
        {
            _onFetchProducts = onFetchProducts;
            _onFetchProductsError = onFetchProductsError;
            _onFetchPlayerPurchases = onFetchPlayerPurchases;
            
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_FetchProducts();
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePayments.Fetch();
                return;
            }
            if (GP_Play2Web.Call("PaymentsFetchProducts"))
                return;
            ConsoleLog("FETCH PRODUCTS");
            Products = GetProducts();
            Purchases = GetPurchases();
            _onFetchProducts?.Invoke(Products);
            OnFetchProducts?.Invoke(Products);
            _onFetchPlayerPurchases?.Invoke(Purchases);
            OnFetchPlayerPurchases?.Invoke(Purchases);
#endif
        }

        public static void Purchase(string idOrTag, Action<string> onPurchaseSuccess = null, Action onPurchaseError = null)
        {
            _onPurchaseSuccess = onPurchaseSuccess;
            _onPurchaseError = onPurchaseError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_Purchase(idOrTag);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePayments.Purchase(idOrTag);
                return;
            }
            if (GP_Play2Web.Call("PaymentsPurchase", idOrTag))
                return;
            ConsoleLog("PURCHASE: " + idOrTag);
#if UNITY_EDITOR
            if (GP_PaymentsStubSession.Enabled)
            {
                GP_PaymentsStubSession.BeginPurchase(idOrTag);
                return;
            }
#endif
            FirePurchaseSuccess(idOrTag);
#endif
        }
        
        public static void Consume(string idOrTag, Action<string> onConsumeSuccess = null, Action onConsumeError = null)
        {
            _onConsumeSuccess = onConsumeSuccess;
            _onConsumeError = onConsumeError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_Consume(idOrTag);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePayments.Consume(idOrTag);
                return;
            }
            if (GP_Play2Web.Call("PaymentsConsume", idOrTag))
                return;
            ConsoleLog("CONSUME: " + idOrTag);
#if UNITY_EDITOR
            if (GP_PaymentsStubSession.Enabled)
            {
                if (GP_PaymentsStubSession.Consume(idOrTag))
                    FireConsumeSuccess(idOrTag);
                else
                    FireConsumeError();
                return;
            }
#endif
            FireConsumeSuccess(idOrTag);
#endif
        }

        public static bool IsPaymentsAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Payments_IsAvailable() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativePayments.IsAvailable;
            if (GP_Play2Web.TryGetBool("PaymentsIsAvailable", out var live))
                return live;
            bool isVal = GP_Settings.instance.GetPlatformSettings().IsPaymentsAvailable;
            ConsoleLog("IS PAYMENTS AVAILABLE: " + isVal);
            return isVal;
#endif
        }

        public static bool IsSubscriptionsAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Payments_IsSubscriptionsAvailable() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativePayments.IsSubscriptionsAvailable;
            if (GP_Play2Web.TryGetBool("PaymentsIsSubscriptionsAvailable", out var live))
                return live;
            bool isVal = GP_Settings.instance.GetPlatformSettings().IsSubscriptionsAvailable;
            ConsoleLog("IS SUBSCRIPTIONS AVAILABLE: " + isVal);
            return isVal;
#endif
        }
        
        public static void Subscribe(string idOrTag, Action<string> onSubscribeSuccess = null, Action onSubscribeError = null)
        {
            _onSubscribeSuccess = onSubscribeSuccess;
            _onSubscribeError = onSubscribeError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_Subscribe(idOrTag);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePayments.Subscribe(idOrTag);
                return;
            }
            if (GP_Play2Web.Call("PaymentsSubscribe", idOrTag))
                return;
            ConsoleLog("SUBSCRIBE: " +  idOrTag);
#if UNITY_EDITOR
            if (GP_PaymentsStubSession.Enabled)
            {
                GP_PaymentsStubSession.BeginSubscribe(idOrTag);
                return;
            }
#endif
            FireSubscribeSuccess(idOrTag);
#endif
        }

        public static void Unsubscribe(string idOrTag, Action<string> onUnsubscribeSuccess = null, Action onUnsubscribeError = null)
        {
            _onUnsubscribeSuccess = onUnsubscribeSuccess;
            _onUnsubscribeError = onUnsubscribeError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_Unsubscribe(idOrTag);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePayments.Unsubscribe(idOrTag);
                return;
            }
            if (GP_Play2Web.Call("PaymentsUnsubscribe", idOrTag))
                return;
            ConsoleLog("UNSUBSCRIBE: " + idOrTag);
#if UNITY_EDITOR
            if (GP_PaymentsStubSession.Enabled)
            {
                if (GP_PaymentsStubSession.Unsubscribe(idOrTag))
                    FireUnsubscribeSuccess(idOrTag);
                else
                    FireUnsubscribeError();
                return;
            }
#endif
            FireUnsubscribeSuccess(idOrTag);
#endif
        }

        internal static void FireFetchProducts(List<FetchProducts> products)
        {
            _onFetchProducts?.Invoke(products);
            OnFetchProducts?.Invoke(products);
        }

        internal static void FireFetchPlayerPurchases(List<FetchPlayerPurchases> purchases)
        {
            _onFetchPlayerPurchases?.Invoke(purchases);
            OnFetchPlayerPurchases?.Invoke(purchases);
        }

        internal static void FireFetchProductsError()
        {
            _onFetchProductsError?.Invoke();
            OnFetchProductsError?.Invoke();
        }

        internal static void FirePurchaseSuccess(string idOrTag)
        {
            _onPurchaseSuccess?.Invoke(idOrTag);
            OnPurchaseSuccess?.Invoke(idOrTag);
        }

        internal static void FirePurchaseError()
        {
            _onPurchaseError?.Invoke();
            OnPurchaseError?.Invoke();
        }

        internal static void FireConsumeSuccess(string idOrTag)
        {
            _onConsumeSuccess?.Invoke(idOrTag);
            OnConsumeSuccess?.Invoke(idOrTag);
        }

        internal static void FireConsumeError()
        {
            _onConsumeError?.Invoke();
            OnConsumeError?.Invoke();
        }

        internal static void FireSubscribeSuccess(string idOrTag)
        {
            _onSubscribeSuccess?.Invoke(idOrTag);
            OnSubscribeSuccess?.Invoke(idOrTag);
        }

        internal static void FireSubscribeError()
        {
            _onSubscribeError?.Invoke();
            OnSubscribeError?.Invoke();
        }

        internal static void FireUnsubscribeSuccess(string idOrTag)
        {
            _onUnsubscribeSuccess?.Invoke(idOrTag);
            OnUnsubscribeSuccess?.Invoke(idOrTag);
        }

        internal static void FireUnsubscribeError()
        {
            _onUnsubscribeError?.Invoke();
            OnUnsubscribeError?.Invoke();
        }

        internal static void FireOpen()
        {
            _onOpen?.Invoke();
            OnOpen?.Invoke();
        }

        internal static void FireClose()
        {
            _onClose?.Invoke();
            OnClose?.Invoke();
        }

        private static bool HasCachedPurchase(string idOrTag)
        {
            if (string.IsNullOrEmpty(idOrTag))
                return false;

            foreach (FetchPlayerPurchases purchase in GetPurchases())
            {
                if (purchase.tag == idOrTag)
                    return true;
                if (int.TryParse(idOrTag, out int id) && purchase.productId == id)
                    return true;
            }

            return false;
        }

        private static List<T> ParseList<T>(string data)
        {
            if (string.IsNullOrEmpty(data) || data == "null" || data == "undefined")
                return new List<T>();

            return UtilityJSON.GetList<T>(data) ?? new List<T>();
        }

    }

    [System.Serializable]
    public class FetchProducts
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public string icon;
        public string iconSmall;
        public float price;
        public string currency;
        public string currencySymbol;
        public bool isSubscription;
        public int period;
        public int trialPeriod;
        public string googlePlayId;
        public string onestoreId;
        public string xsollaId;
    }

    [System.Serializable]
    public class FetchPlayerPurchases
    {
        public string purchaseId;
        public string tag;
        public int productId;
        public string payload;
        public string createdAt;
        public string expiredAt;
        public bool gift;
        public bool subscribed;
        public string orderStatus;
    }
}