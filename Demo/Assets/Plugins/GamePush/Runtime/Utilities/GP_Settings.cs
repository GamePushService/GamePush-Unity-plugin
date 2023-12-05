using System.Collections.Generic;
using UnityEditor;

namespace GamePush
{
    [FilePath("UserSettings/GP_Settings.asset",
        FilePathAttribute.Location.ProjectFolder)]
    public sealed class GP_Settings : ScriptableSingleton<GP_Settings>
    {
        public bool viewLogs = true;
        public GP_PlatformSettings platformSettings;
        public GP_PaymentsStub paymentsStub;
        private void OnDisable() => Save();
        public void Save() => Save(true);

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

        public List<FetchPlayerPurchases> GetPlayerPurchases()
        {
            if (paymentsStub != null)
            {
                return new List<FetchPlayerPurchases>(paymentsStub.Purchases);
            }
            Console.Log("PURCHASES SETTINGS: ", "EMPTY");
            return new List<FetchPlayerPurchases>();
        }
        public List<FetchProducts> GetProducts()
        {
            if (paymentsStub != null)
            {
                return new List<FetchProducts>(paymentsStub.Products);
            }
            Console.Log("PRODUCTS SETTINGS: ", "EMPTY");
            return new List<FetchProducts>();
        }
    }
}