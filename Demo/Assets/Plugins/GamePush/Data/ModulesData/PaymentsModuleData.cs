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
}

