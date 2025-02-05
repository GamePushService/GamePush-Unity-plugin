using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePush
{
    [Serializable]
    public class ProductData
    {
        public int id;
        public string icon;
        public string tag;
        public int price;
        public string currency;
        public string currencySymbol;
        public bool isSubscription;
        public int period;
        public int trialPeriod;
        public string yandexId;
        public string xsollaId;
        public Dictionary<string, string> names;
        public Dictionary<string, string> descriptions;
    }

    [Serializable]
    public class FetchProduct
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

        public FetchProduct() { }
        public FetchProduct(ProductData productData)
        {
            id = productData.id;
            tag = productData.tag;
            icon = productData.icon;
            iconSmall = productData.icon;
            price = productData.price;
            currency = productData.currency;
            currencySymbol = productData.currencySymbol;
            isSubscription = productData.isSubscription;
            period = productData.period;
            trialPeriod = productData.trialPeriod;
        }
    }

    [Serializable]
    public enum OrderStatus
    {
        New, Paid
    }

    [Serializable]
    public class PlayerPurchase
    {
        public string _id;
        public int productId;
        public string tag;
        public Dictionary<string, object> payload;
        public string createdAt;
        public string expiredAt;
        public bool gift;
        public bool subscribed;
        public OrderStatus orderStatus;
    }

    [Serializable]
    public class FetchPlayerPurchase
    {
        public string tag;
        public int productId;
        public string payload;
        public string createdAt;
        public string expiredAt;
        public bool gift;
        public bool subscribed;
    }

    [Serializable]
    public class PurchasePlayerPurchaseInput
    {
        public Dictionary<string, object> payload;

        public int? id;
        public string tag;
    }

    [Serializable]
    public class ConsumePlayerPurchaseInput
    {
        public int? id;
        public string tag;
    }

    [Serializable]
    public class PurchaseOutput
    {
        [FormerlySerializedAs("product")] public ProductData productData;
        public PlayerPurchase purchase;
    }
}

