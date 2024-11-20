using System;
using System.Collections.Generic;
using UnityEngine;
using GamePush;
using GamePush.Data;

#if XSOLLA_SERVICE
using GamePush.Xsolla;
#endif

namespace GamePush.Core
{
    public class PaymentsModule
    {
        private bool isPaymentsAvailable;
        private bool isSubscriptionAvailable;

        private PaymentsConfig paymentsConfig;

        private List<Product> _allProducts;
        private Dictionary<int, Product> _allProductsWithId;
        private Dictionary<string, Product> _allProductsWithTag;

        private List<FetchProduct> _fetchProducts;
        private Dictionary<int, FetchProduct> _fetchProductsWithId;

        public event Action<List<FetchProduct>> OnFetchProducts;
        public event Action OnFetchProductsError;

        public event Action<List<FetchPlayerPurchase>> OnFetchPlayerPurchases;

        public event Action<string> OnPurchaseSuccess;
        public event Action OnPurchaseError;
        public event Action<string> OnConsumeSuccess;
        public event Action OnConsumeError;

        public event Action<string> OnSubscribeSuccess;
        public event Action OnSubscribeError;
        public event Action<string> OnUnsubscribeSuccess;
        public event Action OnUnsubscribeError;


        public void Init(List<Product> products, PlatformConfig platformConfig)
        {
            paymentsConfig = platformConfig.paymentsConfig;
            SetAvailables();

            _allProducts = new List<Product>();
            _allProductsWithId = new Dictionary<int, Product>();
            _allProductsWithTag = new Dictionary<string, Product>();
            _fetchProducts = new List<FetchProduct>();
            _fetchProductsWithId = new Dictionary<int, FetchProduct>();

            foreach (Product product in products)
            {
                _allProducts.Add(product);
                _allProductsWithId.Add(product.id, product);
                _allProductsWithTag.Add(product.tag, product);

                FetchProduct fetchProduct = ProductToFetchProduct(product);
                _fetchProducts.Add(fetchProduct);
                _fetchProductsWithId.Add(product.id, fetchProduct);
            }
        }

        private void SetAvailables()
        {
            isPaymentsAvailable = paymentsConfig.id != "";
            isSubscriptionAvailable = paymentsConfig.id != "";
        }

        private FetchProduct ProductToFetchProduct(Product product)
        {
            string lang = CoreSDK.currentLang;

            FetchProduct fetchProduct = new FetchProduct(product);

            fetchProduct.name = product.names[lang];
            fetchProduct.description = product.descriptions[lang];

            return fetchProduct;
        }

        public void Fetch()
        {
            OnFetchProducts?.Invoke(_fetchProducts);
        }


        public bool IsPaymentsAvailable() => isPaymentsAvailable;
        public bool IsSubscriptionAvailable() => isSubscriptionAvailable;

        public void Purchase(string idOrTag, Action<string> onPurchaseSuccess = null, Action onPurchaseError = null)
        {
            CloudPurchase(idOrTag);

            Action<string> combinedSuccess = (string product) =>
            {
                OnPurchaseSuccess?.Invoke(product);
                onPurchaseSuccess?.Invoke(product);
                CloudPurchase(product);
            };

            Action combinedError = () =>
            {
                OnPurchaseError?.Invoke();
                onPurchaseError?.Invoke();
            };

#if XSOLLA_SERVICE
            //PaymentService.PurchaseItem(idOrTag, combinedSuccess, combinedError);
#else
            onPurchaseError?.Invoke()
#endif
        }

        public void Consume(string idOrTag)
        {

        }

        private async void CloudPurchase(string product)
        {
            PurchasePlayerPurchaseInput playerInput = new PurchasePlayerPurchaseInput();
            playerInput.tag = product;

            await DataFetcher.PurchaseProduct(playerInput);

        }

    }
}
