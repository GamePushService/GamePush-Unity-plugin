/*
 * This file is a part of the Yandex Advertising Network
 *
 * Version for iOS (C) 2023 YANDEX
 *
 * You may not use this file except in compliance with the License.
 * You may obtain a copy of the License at https://legal.yandex.com/partner_ch/
 */

using System;
using YandexMobileAds.Base;
using System.Collections.Generic;

namespace YandexMobileAds.Platforms.iOS
{
    #if (UNITY_5 && UNITY_IOS) || UNITY_IPHONE

    public class AdRequestClient : IDisposable
    {
        public string ObjectId { get; private set; }
        private readonly string _adUnitId;
        private readonly LocationClient _location;
        private readonly string _contextQuery;
        private readonly ListClient _contextTags;
        private readonly DictionaryClient _parameters;
        private readonly string _age;
        private readonly string _gender;

        public AdRequestClient(AdRequest adRequest)
        {
            if (adRequest.Location != null)
            {
                this._location = new LocationClient(adRequest.Location);
            }
            this._contextQuery = adRequest.ContextQuery;
            this._age = adRequest.Age;
            this._gender = adRequest.Gender;
            this._contextTags = new ListClient();
            if (adRequest.ContextTags != null)
            {
                foreach (string item in adRequest.ContextTags)
                {
                    this._contextTags.Add(item);
                }
            }
            this._parameters = new DictionaryClient();
            if (adRequest.Parameters != null)
            {
                foreach (KeyValuePair<string, string> entry in adRequest.Parameters)
                {
                    this._parameters.Put(entry.Key, entry.Value);
                }
            }
            string locationId = this._location != null ?
                                    this._location.ObjectId : null;
            string contextTagsId = this._contextTags != null ?
                                       this._contextTags.ObjectId : null;
            string parametersId = this._parameters != null ?
                                      this._parameters.ObjectId : null;
            this.ObjectId = AdRequestBridge.YMAUnityCreateAdRequest(
                locationId,
                _contextQuery,
                contextTagsId,
                parametersId,
                _age,
                _gender);
        }

        public void Destroy()
        {
            ObjectBridge.YMAUnityDestroyObject(this.ObjectId);
        }

        public void Dispose()
        {
            this.Destroy();
        }

        ~AdRequestClient()
        {
            this.Destroy();
        }
    }

    #endif
}
