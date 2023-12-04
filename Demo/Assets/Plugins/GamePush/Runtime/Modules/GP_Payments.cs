using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities;
using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Payments : MonoBehaviour
    {
        public static event UnityAction<List<FetchProducts>> OnFetchProducts;
        public static event UnityAction OnFetchProductsError;

        public static event UnityAction<List<FetchPlayerPurcahses>> OnFetchPlayerPurchases;

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


        [DllImport("__Internal")]
        private static extern void GP_Payments_FetchProducts();
        public static void Fetch()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_FetchProducts();
#else
            if (GP_ConsoleController.Instance.PaymentsConsoleLogs)
                Console.Log("PAYMENTS: ", "FETCH PRODUCTS");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Payments_Purchase(string idOrTag);
        public static void Purchase(string idOrTag, Action<string> onPurchaseSuccess = null, Action onPurchaseError = null)
        {
            _onPurchaseSuccess = onPurchaseSuccess;
            _onPurchaseError = onPurchaseError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_Purchase(idOrTag);
#else
            if (GP_ConsoleController.Instance.PaymentsConsoleLogs)
                Console.Log("PAYMENTS: ", "PURCHASE: " + idOrTag);
            _onPurchaseSuccess?.Invoke(idOrTag);
            OnPurchaseSuccess?.Invoke(idOrTag);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Payments_Consume(string idOrTag);
        public static void Consume(string idOrTag, Action<string> onConsumeSuccess = null, Action onConsumeError = null)
        {
            _onConsumeSuccess = onConsumeSuccess;
            _onConsumeError = onConsumeError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_Consume(idOrTag);
#else
            if (GP_ConsoleController.Instance.PaymentsConsoleLogs)
                Console.Log("PAYMENTS: ", "CONSUME: " + idOrTag);
            _onConsumeSuccess?.Invoke(idOrTag);
            OnConsumeSuccess?.Invoke(idOrTag);
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Payments_IsAvailable();
        public static bool IsPaymentsAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Payments_IsAvailable() == "true";
#else
            if (GP_ConsoleController.Instance.PaymentsConsoleLogs)
                Console.Log("IS PAYMENTS AVAILABLE: ", "TRUE");
            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Payments_IsSubscriptionsAvailable();
        public static bool IsSubscriptionsAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Payments_IsSubscriptionsAvailable() == "true";
#else
            if (GP_ConsoleController.Instance.PaymentsConsoleLogs)
                Console.Log("IS SUBSCRIPTIONS AVAILABLE: ", "TRUE");
            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Payments_Subscribe(string idOrTag);
        public static void Subscribe(string idOrTag, Action<string> onSubscribeSuccess = null, Action onSubscribeError = null)
        {
            _onSubscribeSuccess = onSubscribeSuccess;
            _onSubscribeError = onSubscribeError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_Subscribe(idOrTag);
#else
            if (GP_ConsoleController.Instance.PaymentsConsoleLogs)
                Console.Log("PAYMENTS: SUBSCRIBE: ", idOrTag);
            _onSubscribeSuccess?.Invoke(idOrTag);
            OnSubscribeSuccess?.Invoke(idOrTag);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Payments_Unsubscribe(string idOrTag);
        public static void Unsubscribe(string idOrTag, Action<string> onUnsubscribeSuccess = null, Action onUnsubscribeError = null)
        {
            _onUnsubscribeSuccess = onUnsubscribeSuccess;
            _onUnsubscribeError = onUnsubscribeError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Payments_Unsubscribe(idOrTag);
#else
            if (GP_ConsoleController.Instance.PaymentsConsoleLogs)
                Console.Log("PAYMENTS: UNSUBSCRIBE: ", idOrTag);
            _onUnsubscribeSuccess?.Invoke(idOrTag);
            OnUnsubscribeSuccess?.Invoke(idOrTag);
#endif
        }


        private void CallPaymentsFetchProducts(string data) => OnFetchProducts?.Invoke(GP_JSON.GetList<FetchProducts>(data));
        private void CallPaymentsFetchPlayerPurcahses(string data) => OnFetchPlayerPurchases?.Invoke(GP_JSON.GetList<FetchPlayerPurcahses>(data));

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

    [System.Serializable]
    public class FetchProducts
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public string icon;
        public string iconSmall;
        public int price;
        public string currency;
        public string currencySymbol;
        public bool isSubscription;
        public int period;
        public int trialPeriod;
    }

    [System.Serializable]
    public class FetchPlayerPurcahses
    {
        public int productId;
        public string payload;
        public string createdAt;
        public string expiredAt;
        public bool gift;
        public bool subscribed;
    }
}