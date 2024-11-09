using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    public class PaymentsModuleData
    {
        
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
    public class FetchPlayerPurchases
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

