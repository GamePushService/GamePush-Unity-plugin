using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush;

public class FetchProducts : MonoBehaviour
{
    private void OnEnable()
    {
        GP_Payments.OnFetchProducts += OnFetchProducts;
    }

    private void OnDisable()
    {
        GP_Payments.OnFetchProducts -= OnFetchProducts;
    }

    public async void Start()
    {
        await GP_Init.Ready;
        GP_Payments.Fetch();
    }

    private void OnFetchProducts(List<GamePush.FetchProducts> products)
    {
        for (int i = 0; i < products.Count; i++)
        {
            Debug.Log("PRODUCT: ID: " + products[i].id);
            Debug.Log("PRODUCT: TAG: " + products[i].tag);
            Debug.Log("PRODUCT: NAME: " + products[i].name);
            Debug.Log("PRODUCT: DESCRIPTION: " + products[i].description);
            Debug.Log("PRODUCT: ICON: " + products[i].icon);
            Debug.Log("PRODUCT: ICON SMALL: " + products[i].iconSmall);
            Debug.Log("PRODUCT: PRICE: " + products[i].price);
            Debug.Log("PRODUCT: CURRENCY: " + products[i].currency);
            Debug.Log("PRODUCT: CURRENCY SYMBOL: " + products[i].currencySymbol);
            Debug.Log("PRODUCT: IS SUBSCRIPTION: " + products[i].isSubscription);
            Debug.Log("PRODUCT: PERIOD: " + products[i].period);
            Debug.Log("PRODUCT: TRIAL PERIOD: " + products[i].trialPeriod);
        }
    }
}
