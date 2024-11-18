using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Tools;

namespace GamePush
{
    public class GP_Payments : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Payments);


        #region Actions
        public static event UnityAction<List<FetchProduct>> OnFetchProducts;
        public static event UnityAction OnFetchProductsError;

        public static event UnityAction<List<FetchPlayerPurchase>> OnFetchPlayerPurchases;

        public static event UnityAction<string> OnPurchaseSuccess;
        public static event UnityAction OnPurchaseError;
        public static event UnityAction<string> OnConsumeSuccess;
        public static event UnityAction OnConsumeError;

        public static event UnityAction<string> OnSubscribeSuccess;
        public static event UnityAction OnSubscribeError;
        public static event UnityAction<string> OnUnsubscribeSuccess;
        public static event UnityAction OnUnsubscribeError;


        private static event Action<string> _onPurchaseSuccess;
        private static event Action _onPurchaseError;

        private static event Action<string> _onConsumeSuccess;
        private static event Action _onConsumeError;

        private static event Action<string> _onSubscribeSuccess;
        private static event Action _onSubscribeError;

        private static event Action<string> _onUnsubscribeSuccess;
        private static event Action _onUnsubscribeError;
        #endregion

#if !UNITY_EDITOR && UNITY_WEBGL
        #region DllImport
        [DllImport("__Internal")]
        private static extern void GP_Payments_FetchProducts();
        [DllImport("__Internal")]
        private static extern void GP_Payments_Purchase(string idOrTag);
        [DllImport("__Internal")]
        private static extern void GP_Payments_Consume(string idOrTag);
        [DllImport("__Internal")]
        private static extern string GP_Payments_IsAvailable();
        [DllImport("__Internal")]
        private static extern string GP_Payments_IsSubscriptionsAvailable();
        [DllImport("__Internal")]
        private static extern void GP_Payments_Subscribe(string idOrTag);
        [DllImport("__Internal")]
        private static extern void GP_Payments_Unsubscribe(string idOrTag);
        #endregion
#endif

        public static void Fetch()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_FetchProducts();
#else
            ConsoleLog("FETCH PRODUCTS");
            CoreSDK.payments.Fetch();
            //OnFetchProducts?.Invoke(GP_Settings.instance.GetProducts());
            //OnFetchPlayerPurchases?.Invoke(GP_Settings.instance.GetPlayerPurchases());
#endif
        }

        public static void Purchase(string idOrTag, Action<string> onPurchaseSuccess = null, Action onPurchaseError = null)
        {
            _onPurchaseSuccess = onPurchaseSuccess;
            _onPurchaseError = onPurchaseError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_Purchase(idOrTag);
#else
            ConsoleLog("PURCHASE: " + idOrTag);
            CoreSDK.payments.Purchase(idOrTag);
            
            //_onPurchaseSuccess?.Invoke(idOrTag);
            //OnPurchaseSuccess?.Invoke(idOrTag);
#endif
        }

        public static void Consume(string idOrTag, Action<string> onConsumeSuccess = null, Action onConsumeError = null)
        {
            _onConsumeSuccess = onConsumeSuccess;
            _onConsumeError = onConsumeError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_Consume(idOrTag);
#else

            ConsoleLog("CONSUME: " + idOrTag);
            _onConsumeSuccess?.Invoke(idOrTag);
            OnConsumeSuccess?.Invoke(idOrTag);
#endif
        }

        public static bool IsPaymentsAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Payments_IsAvailable() == "true";
#else
            //bool isVal = GP_Settings.instance.GetPlatformSettings().IsPaymentsAvailable;
            //ConsoleLog("IS PAYMENTS AVAILABLE: " + isVal);
            return CoreSDK.payments.IsPaymentsAvailable();
#endif
        }


        public static bool IsSubscriptionsAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Payments_IsSubscriptionsAvailable() == "true";
#else
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

            ConsoleLog("SUBSCRIBE: " +  idOrTag);
            _onSubscribeSuccess?.Invoke(idOrTag);
            OnSubscribeSuccess?.Invoke(idOrTag);
#endif
        }

        public static void Unsubscribe(string idOrTag, Action<string> onUnsubscribeSuccess = null, Action onUnsubscribeError = null)
        {
            _onUnsubscribeSuccess = onUnsubscribeSuccess;
            _onUnsubscribeError = onUnsubscribeError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_Unsubscribe(idOrTag);
#else

            ConsoleLog("UNSUBSCRIBE: " + idOrTag);
            _onUnsubscribeSuccess?.Invoke(idOrTag);
            OnUnsubscribeSuccess?.Invoke(idOrTag);
#endif
        }


        private void CallPaymentsFetchProducts(string data) => OnFetchProducts?.Invoke(UtilityJSON.GetList<FetchProduct>(data));
        private void CallPaymentsFetchPlayerPurcahses(string data) => OnFetchPlayerPurchases?.Invoke(UtilityJSON.GetList<FetchPlayerPurchase>(data));

        private void CallPaymentsFetchProductsError() => OnFetchProductsError?.Invoke();

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
    }

}