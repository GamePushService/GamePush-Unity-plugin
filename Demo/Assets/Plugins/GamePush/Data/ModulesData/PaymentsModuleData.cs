using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    [Serializable]
    public class Product
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
        public FetchProduct(Product product)
        {
            id = product.id;
            tag = product.tag;
            icon = product.icon;
            iconSmall = product.icon;
            price = product.price;
            currency = product.currency;
            currencySymbol = product.currencySymbol;
            isSubscription = product.isSubscription;
            period = product.period;
            trialPeriod = product.trialPeriod;
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
        public Product product;
        public PlayerPurchase purchase;
    }
}

