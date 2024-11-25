using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    [CreateAssetMenu(fileName = "GP_PaymentsStub", menuName = "GP_Settings/GP_PaymentsStub")]
    public class GP_PaymentsStub : ScriptableObject
    {
        [SerializeField] private List<FetchProduct> products = new();
        [SerializeField] private List<FetchPlayerPurchase> purchases = new();

        public IReadOnlyList<FetchProduct> Products => products;
        public IReadOnlyList<FetchPlayerPurchase> Purchases => purchases;
    }
}