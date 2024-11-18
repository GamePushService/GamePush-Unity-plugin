using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if XSOLLA_SERVICE
using GamePush.Xsolla;
#endif

namespace GamePush.Core
{
    public class PaymentsModule
    {
        private bool isPaymentsAvailable;
        private List<Product> allProducts;
        private Dictionary<int, Product> allProductsWithId;
        private Dictionary<string, Product> allProductsWithTag;

        public bool IsPaymentsAvailable() => isPaymentsAvailable;

        public void Init(List<Product> products)
        {
            allProducts = new List<Product>();
            allProductsWithId = new Dictionary<int, Product>();
            allProductsWithTag = new Dictionary<string, Product>();

            foreach (Product product in products)
            {
                allProducts.Add(product);
                allProductsWithId.Add(product.id, product);
                allProductsWithTag.Add(product.tag, product);
            }
        }

        public async void Fetch()
        {
            await DataFetcher.FetchPlayerPurchases(true);
        }

        //public async Task<Transaction<PlayerPurchasesOutput>> FetchProductsAsync()
        //{
        //    var transaction = CreateTransaction<PlayerPurchasesOutput>();
        //    gp.Loader.Increment();

        //    try
        //    {
        //        var playerPurchases = purchases;
        //        var products = await adapter.MapProductsAsync(gp, productsList);
        //        _productsList = products;
        //        RefreshProductsMap();

        //        transaction.Done(new PlayerPurchasesOutput { Products = products, PlayerPurchases = playerPurchases });
        //        _events.Emit("fetchProducts", new { products, playerPurchases });
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction.Abort(ex);
        //        _events.Emit("error:fetchProducts", ex);
        //    }
        //    finally
        //    {
        //        gp.Loader.Decrement();
        //    }

        //    return await transaction.Ready;
        //}

        public void Purchase(string idOrTag)
        {
#if XSOLLA_SERVICE
            PaymentService.PurchaseItem(idOrTag);
#endif
        }

        public void Consume()
        {

        }


    }
}
