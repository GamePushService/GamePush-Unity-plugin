using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GamePush;
using Examples.Console;

namespace Examples.Payments
{
    public class Payments : MonoBehaviour
    {
        [SerializeField] private Button _isAvailableButton;
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private Button _consumeButton;
        [SerializeField] private Button _fetchButton;


        private void OnEnable()
        {
            _isAvailableButton.onClick.AddListener(IsPaymentsAvailable);
            _purchaseButton.onClick.AddListener(Purchase);
            _consumeButton.onClick.AddListener(Consume);
            _fetchButton.onClick.AddListener(Fetch);

            GP_Payments.OnFetchProducts += OnFetchProducts;
            GP_Payments.OnFetchProductsError += OnFetchProductsError;
            GP_Payments.OnFetchPlayerPurchases += OnFetchPlayerPurchases;
        }

        private void OnDisable()
        {
            _isAvailableButton.onClick.RemoveListener(IsPaymentsAvailable);
            _purchaseButton.onClick.RemoveListener(Purchase);
            _consumeButton.onClick.RemoveListener(Consume);
            _fetchButton.onClick.RemoveListener(Fetch);

            GP_Payments.OnFetchProducts -= OnFetchProducts;
            GP_Payments.OnFetchProductsError -= OnFetchProductsError;
            GP_Payments.OnFetchPlayerPurchases -= OnFetchPlayerPurchases;
        }


        public void IsPaymentsAvailable() => ConsoleUI.Instance.Log("IS PAYMENTS AVAILABLE: " + GP_Payments.IsPaymentsAvailable());

        public void Purchase() => GP_Payments.Purchase("EXTRA_GOLD", OnPurchaseSuccess, OnPurchaseError);
        public void Consume() => GP_Payments.Consume("EXTRA_GOLD", OnConsumeSuccess, OnConsumeError);

        public void Fetch() => GP_Payments.Fetch();


        private void OnPurchaseSuccess(string productIdOrTag) => ConsoleUI.Instance.Log("PURCHASE: SUCCESS: " + productIdOrTag);
        private void OnPurchaseError() => ConsoleUI.Instance.Log("PURCHASE: ERROR");

        private void OnConsumeSuccess(string productIdOrTag) => ConsoleUI.Instance.Log("CONSUME: SUCCESS: " + productIdOrTag);
        private void OnConsumeError() => ConsoleUI.Instance.Log("CONSUME: ERROR");

        private void OnFetchProducts(List<FetchProducts> products)
        {
            for (int i = 0; i < products.Count; i++)
            {
                ConsoleUI.Instance.Log("PRODUCT: ID: " + products[i].id);
                ConsoleUI.Instance.Log("PRODUCT: TAG: " + products[i].tag);
                ConsoleUI.Instance.Log("PRODUCT: NAME: " + products[i].name);
                ConsoleUI.Instance.Log("PRODUCT: DESCRIPTION: " + products[i].description);
                ConsoleUI.Instance.Log("PRODUCT: ICON: " + products[i].icon);
                ConsoleUI.Instance.Log("PRODUCT: ICON SMALL: " + products[i].iconSmall);
                ConsoleUI.Instance.Log("PRODUCT: PRICE: " + products[i].price);
                ConsoleUI.Instance.Log("PRODUCT: CURRENCY: " + products[i].currency);
                ConsoleUI.Instance.Log("PRODUCT: CURRENCY SYMBOL: " + products[i].currencySymbol);
                ConsoleUI.Instance.Log("PRODUCT: IS SUBSCRIPTION: " + products[i].isSubscription);
                ConsoleUI.Instance.Log("PRODUCT: PERIOD: " + products[i].period);
                ConsoleUI.Instance.Log("PRODUCT: TRIAL PERIOD: " + products[i].trialPeriod);
            }
        }

        private void OnFetchProductsError() => ConsoleUI.Instance.Log("FETCH PRODUCTS: ERROR");

        private void OnFetchPlayerPurchases(List<FetchPlayerPurchases> purcahses)
        {
            for (int i = 0; i < purcahses.Count; i++)
            {
                ConsoleUI.Instance.Log("PLAYER PURCHASES: PRODUCT TAG: " + purcahses[i].tag);
                ConsoleUI.Instance.Log("PLAYER PURCHASES: PRODUCT ID: " + purcahses[i].productId);
                ConsoleUI.Instance.Log("PLAYER PURCHASES: PAYLOAD: " + purcahses[i].payload);
                ConsoleUI.Instance.Log("PLAYER PURCHASES: CREATED AT: " + purcahses[i].createdAt);
                ConsoleUI.Instance.Log("PLAYER PURCHASES: EXPIRED AT: " + purcahses[i].expiredAt);
                ConsoleUI.Instance.Log("PLAYER PURCHASES: GIFT: " + purcahses[i].gift);
                ConsoleUI.Instance.Log("PLAYER PURCHASES: SUBSCRIBED: " + purcahses[i].subscribed);
            }
        }


    }
}