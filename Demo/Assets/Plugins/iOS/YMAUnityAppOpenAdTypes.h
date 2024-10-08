/*
 * This file is a part of the Yandex Advertising Network
 *
 * Version for iOS (C) 2023 YANDEX
 *
 * You may not use this file except in compliance with the License.
 * You may obtain a copy of the License at https://legal.yandex.com/partner_ch/
 */

// Unity appOpen client reference is needed to pass banner client in callback.
typedef const void *YMAUnityAppOpenAdClientRef;

typedef void (*YMAUnityAppOpenAdDidFailToShowCallback)(YMAUnityAppOpenAdClientRef *appOpenAdClient, char *error);
typedef void (*YMAUnityAppOpenAdDidShowCallback)(YMAUnityAppOpenAdClientRef *appOpenAdClient);
typedef void (*YMAUnityAppOpenAdDidDismissCallback)(YMAUnityAppOpenAdClientRef *appOpenAdClient);
typedef void (*YMAUnityAppOpenAdDidClickCallback)(YMAUnityAppOpenAdClientRef *appOpenAdClient);
typedef void (*YMAUnityAppOpenAdDidTrackImpressionCallback)(YMAUnityAppOpenAdClientRef *appOpenAdClient, char *rawData);
