using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    [CreateAssetMenu(fileName = "GP_PaymentsStub", menuName = "GP_Settings/GP_PaymentsStub")]
    public class GP_PaymentsStub : ScriptableObject
    {
        [SerializeField] private List<FetchProducts> products = new();
        [SerializeField] private List<FetchPlayerPurchases> purchases = new();

        public IReadOnlyList<FetchProducts> Products => products;
        public IReadOnlyList<FetchPlayerPurchases> Purchases => purchases;

        public void MergeProducts(IReadOnlyList<FetchProducts> incoming)
        {
            if (incoming == null)
                return;

            foreach (FetchProducts product in incoming)
            {
                int index = IndexOfProduct(product);
                if (index >= 0)
                    products[index] = product;
                else
                    products.Add(product);
            }
        }

        private int IndexOfProduct(FetchProducts product)
        {
            for (int i = 0; i < products.Count; i++)
            {
                if (product.id != 0 && products[i].id == product.id)
                    return i;
                if (product.id == 0 && !string.IsNullOrEmpty(product.tag) && products[i].tag == product.tag)
                    return i;
            }

            return -1;
        }
    }
}