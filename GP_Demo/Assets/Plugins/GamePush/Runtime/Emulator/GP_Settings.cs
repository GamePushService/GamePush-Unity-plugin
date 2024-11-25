using System;
using System.Collections.Generic;

namespace GamePush
{
    [Serializable]
    public sealed class GP_Settings
    {
        public bool viewLogs = true;
        public GP_PlatformSettings platformSettings;
        public GP_PaymentsStub paymentsStub;
        
        private static GP_Settings _instance;

        public static GP_Settings instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GP_Settings();
                return _instance;
            }
            set => _instance = value;
        }

        public PlatformSettings GetPlatformSettings(){
            if (platformSettings != null)
            {
                return platformSettings.GetPlatformSettings();
            }
            Console.Log("PLATFORM SETTINGS: ", "DEFAULT");
            return new PlatformSettings();
        }
        
        public GP_PlatformSettings GetFromPlatformSettings(){
            if (platformSettings != null)
            {
                return platformSettings;
            }
            Console.Log("PLATFORM SETTINGS: ", "NULL");
            return new GP_PlatformSettings();
        }

        public Language GetLanguage()
        {
            if (platformSettings != null)
            {
                return platformSettings.Language;
            }
            Console.Log("PLATFORM LANGUAGE: ", "DEFAULT - ENGLISH");
            return Language.English;
        }

        public List<FetchPlayerPurchase> GetPlayerPurchases()
        {
            if (paymentsStub != null)
            {
                return new List<FetchPlayerPurchase>(paymentsStub.Purchases);
            }
            Console.Log("PURCHASES SETTINGS: ", "EMPTY");
            return new List<FetchPlayerPurchase>();
        }
        public List<FetchProduct> GetProducts()
        {
            if (paymentsStub != null)
            {
                return new List<FetchProduct>(paymentsStub.Products);
            }
            Console.Log("PRODUCTS SETTINGS: ", "EMPTY");
            return new List<FetchProduct>();
        }
    }
}