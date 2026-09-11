#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using GamePush.Data;
using UnityEngine;

namespace GamePush
{
    public enum GP_PaymentsStubKind
    {
        None,
        Purchase,
        Subscribe
    }

    public static class GP_PaymentsStubSession
    {
        public static bool Enabled => ProjectData.PAYMENTS_STUBS && !GP_Play2Web.Enabled;

        public static GP_PaymentsStubKind Kind { get; private set; }
        public static string IdOrTag { get; private set; }
        public static FetchProducts Product { get; private set; }
        public static bool IsOpen => Kind != GP_PaymentsStubKind.None;

        private static List<FetchPlayerPurchases> _purchases;
        private static bool _loaded;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Kind = GP_PaymentsStubKind.None;
            IdOrTag = null;
            Product = null;
            _purchases = null;
            _loaded = false;
        }

        public static List<FetchPlayerPurchases> GetPurchases()
        {
            EnsureLoaded();
            return new List<FetchPlayerPurchases>(_purchases);
        }

        public static void BeginPurchase(string idOrTag) => Begin(GP_PaymentsStubKind.Purchase, idOrTag);

        public static void BeginSubscribe(string idOrTag) => Begin(GP_PaymentsStubKind.Subscribe, idOrTag);

        public static void Confirm()
        {
            if (!IsOpen)
                return;

            GP_PaymentsStubKind kind = Kind;
            string idOrTag = IdOrTag;
            FetchProducts product = Product;
            CloseOverlay();

            EnsureLoaded();
            _purchases.Add(CreatePurchase(product, idOrTag, kind == GP_PaymentsStubKind.Subscribe));

            if (kind == GP_PaymentsStubKind.Subscribe)
                GP_Payments.FireSubscribeSuccess(idOrTag);
            else
                GP_Payments.FirePurchaseSuccess(idOrTag);

            GP_Payments.FireClose();
            GP_Game.FireResume();
        }

        public static void Cancel()
        {
            if (!IsOpen)
                return;

            GP_PaymentsStubKind kind = Kind;
            CloseOverlay();

            if (kind == GP_PaymentsStubKind.Subscribe)
                GP_Payments.FireSubscribeError();
            else
                GP_Payments.FirePurchaseError();

            GP_Payments.FireClose();
            GP_Game.FireResume();
        }

        public static bool Consume(string idOrTag)
        {
            EnsureLoaded();
            int index = IndexOf(idOrTag);
            if (index < 0)
                return false;
            _purchases.RemoveAt(index);
            return true;
        }

        public static bool Unsubscribe(string idOrTag)
        {
            EnsureLoaded();
            int index = IndexOf(idOrTag);
            if (index < 0)
                return false;

            FetchPlayerPurchases purchase = _purchases[index];
            purchase.subscribed = false;
            _purchases[index] = purchase;
            return true;
        }

        private static void Begin(GP_PaymentsStubKind kind, string idOrTag)
        {
            if (IsOpen)
            {
                GP_Logger.ModuleLog("PAYMENT STUB ALREADY OPEN", ModuleName.Payments);
                return;
            }

            if (!IsAvailable(kind))
            {
                if (kind == GP_PaymentsStubKind.Subscribe)
                    GP_Payments.FireSubscribeError();
                else
                    GP_Payments.FirePurchaseError();
                return;
            }

            Kind = kind;
            IdOrTag = idOrTag;
            Product = FindProduct(idOrTag);
            GP_Payments.FireOpen();
            GP_Game.FirePause();
        }

        private static void CloseOverlay()
        {
            Kind = GP_PaymentsStubKind.None;
            IdOrTag = null;
            Product = null;
        }

        private static void EnsureLoaded()
        {
            if (_loaded)
                return;
            _purchases = GP_Settings.instance.GetPlayerPurchases() ?? new List<FetchPlayerPurchases>();
            _loaded = true;
        }

        private static FetchProducts FindProduct(string idOrTag)
        {
            foreach (FetchProducts product in GP_Settings.instance.GetProducts())
            {
                if (Matches(product, idOrTag))
                    return product;
            }

            var fallback = new FetchProducts { tag = idOrTag, name = idOrTag };
            if (int.TryParse(idOrTag, out int id))
                fallback.id = id;
            return fallback;
        }

        private static bool Matches(FetchProducts product, string idOrTag)
        {
            if (product == null || string.IsNullOrEmpty(idOrTag))
                return false;
            if (product.tag == idOrTag)
                return true;
            return int.TryParse(idOrTag, out int id) && product.id == id;
        }

        private static int IndexOf(string idOrTag)
        {
            if (string.IsNullOrEmpty(idOrTag))
                return -1;

            for (int i = 0; i < _purchases.Count; i++)
            {
                FetchPlayerPurchases purchase = _purchases[i];
                if (purchase.tag == idOrTag)
                    return i;
                if (int.TryParse(idOrTag, out int id) && purchase.productId == id)
                    return i;
            }

            return -1;
        }

        private static FetchPlayerPurchases CreatePurchase(FetchProducts product, string idOrTag, bool subscribed)
        {
            int productId = product != null && product.id != 0
                ? product.id
                : int.TryParse(idOrTag, out int parsed) ? parsed : 0;

            return new FetchPlayerPurchases
            {
                tag = product != null && !string.IsNullOrEmpty(product.tag) ? product.tag : idOrTag,
                productId = productId,
                createdAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+0000"),
                subscribed = subscribed
            };
        }

        private static bool IsAvailable(GP_PaymentsStubKind kind)
        {
            GP_PlatformSettings platformSettings = GP_Settings.instance.platformSettings;
            if (platformSettings == null || platformSettings.Settings == null || platformSettings.Settings.Count == 0)
                return true;

            bool hasMatch = false;
            foreach (PlatformSettings row in platformSettings.Settings)
            {
                if (row.Platform != platformSettings.PlatformToEmulate)
                    continue;
                hasMatch = true;
                return kind == GP_PaymentsStubKind.Subscribe
                    ? row.IsSubscriptionsAvailable
                    : row.IsPaymentsAvailable;
            }

            return !hasMatch;
        }
    }
}
#endif
