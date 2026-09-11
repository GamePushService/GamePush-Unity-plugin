using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GamePush;

namespace GamePush.Native
{
    public static class NativePaymentsService
    {
        public const string OrderPaid = "PAID";
        public const string OrderNew = "NEW";

        public static async Task<(List<FetchProducts> products, List<FetchPlayerPurchases> purchases)> Fetch()
        {
            var json = await NativeCore.Client.Fetch(NativeQueries.FetchPlayerPurchases);
            var result = GpJson.GetObject(json, "result") ?? json;
            ThrowIfProblem(result, "fetch_products");
            var products = ParseProducts(GpJson.GetObjectArray(result, "products"));
            var purchases = ParsePurchases(GpJson.GetObjectArray(result, "playerPurchases"), products);
            return (products, purchases);
        }

        public static async Task<(FetchProducts product, FetchPlayerPurchases purchase)> Purchase(string idOrTag,
            Dictionary<string, object> payload = null)
        {
            var input = BuildIdOrTag(idOrTag);
            input["payload"] = payload ?? new Dictionary<string, object>();
            var json = await NativeCore.Client.Fetch(NativeQueries.PurchasePlayerPurchase, input);
            return ParsePurchaseOutput(json, "purchase");
        }

        public static async Task<(FetchProducts product, FetchPlayerPurchases purchase)> Consume(string idOrTag)
        {
            var json = await NativeCore.Client.Fetch(NativeQueries.ConsumePlayerPurchase, BuildIdOrTag(idOrTag));
            return ParsePurchaseOutput(json, "consume");
        }

        public static async Task<(FetchProducts product, FetchPlayerPurchases purchase)> CancelSubscription(
            string idOrTag, string payloadJson)
        {
            var input = BuildIdOrTag(idOrTag);
            input["payload"] = new GpRawJson(string.IsNullOrEmpty(payloadJson) ? "{}" : payloadJson);
            var json = await NativeCore.Client.Fetch(NativeQueries.CancelPlayerSubscription, input);
            return ParsePurchaseOutput(json, "unsubscribe");
        }

        public static async Task<(FetchProducts product, FetchPlayerPurchases purchase)> ResumeSubscription(
            string idOrTag, string payloadJson)
        {
            var input = BuildIdOrTag(idOrTag);
            input["payload"] = new GpRawJson(string.IsNullOrEmpty(payloadJson) ? "{}" : payloadJson);
            var json = await NativeCore.Client.Fetch(NativeQueries.ResumePlayerSubscription, input);
            return ParsePurchaseOutput(json, "subscribe");
        }

        public static async Task<FetchPlayerPurchases> GetPlayerPurchase(string purchaseId,
            Dictionary<string, object> payload = null, int productId = 0, bool requirePaid = true)
        {
            var input = new Dictionary<string, object>
            {
                ["projectId"] = NativeCore.Client.ProjectId,
                ["purchaseId"] = purchaseId ?? ""
            };
            if (string.IsNullOrEmpty(purchaseId) && productId > 0)
                input["productId"] = productId;
            if (payload != null)
                input["payload"] = payload;
            if (requirePaid)
                input["orderStatus"] = OrderPaid;

            var json = await NativeCore.Client.Fetch(NativeQueries.GetPlayerPurchase, input);
            var result = GpJson.GetObject(json, "result") ?? json;
            if (IsProblem(result, out var message))
            {
                if (IsNotFound(message))
                    return null;
                throw new Exception(message);
            }

            var purchase = ParsePurchase(result, null);
            if (purchase == null)
                return null;
            if (requirePaid && !IsPaid(purchase))
                return null;
            return purchase;
        }

        public static async Task<bool> WaitUntilPaid(string purchaseId, Dictionary<string, object> payload,
            int productId, CancellationToken token, int maxAttempts = 0, int intervalMs = 3000)
        {
            var attempts = 0;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var purchase = await GetPlayerPurchase(purchaseId, payload, productId);
                    if (purchase != null && IsPaid(purchase))
                    {
                        GP_Logger.Info("PAYMENTS",
                            "Poll paid idLen=" + (purchaseId ?? "").Length + " attempt=" + (attempts + 1));
                        return true;
                    }
                    GP_Logger.Info("PAYMENTS",
                        "Poll wait idLen=" + (purchaseId ?? "").Length +
                        " attempt=" + (attempts + 1) + "/" + (maxAttempts > 0 ? maxAttempts.ToString() : "inf") +
                        " " + (purchase == null ? "not_found" : "status=" + (purchase.orderStatus ?? "")));
                }
                catch (Exception exception)
                {
                    if (!IsNotFound(exception.Message))
                        throw;
                    GP_Logger.Info("PAYMENTS",
                        "Poll wait idLen=" + (purchaseId ?? "").Length +
                        " attempt=" + (attempts + 1) + " not_found");
                }

                attempts++;
                if (maxAttempts > 0 && attempts >= maxAttempts)
                    return false;
                try
                {
                    await Task.Delay(intervalMs, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            try
            {
                var last = await GetPlayerPurchase(purchaseId, payload, productId);
                return last != null && IsPaid(last);
            }
            catch
            {
                return false;
            }
        }

        public static async Task ValidateGooglePlay(string body)
        {
            var path = "google_play/payments?projectId=" + NativeCore.Client.ProjectId +
                       "&playerId=" + NativePlayer.Id;
            await NativeCore.Client.PostRaw(path, body ?? "{}");
        }

        public static FetchProducts ParseProduct(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            var product = new FetchProducts
            {
                id = GpJson.GetInt(json, "id"),
                tag = GpJson.TryGetString(json, "tag", out var tag) ? tag ?? "" : "",
                name = GpJson.TryGetString(json, "name", out var name) ? name ?? "" : "",
                description = GpJson.TryGetString(json, "description", out var desc) ? desc ?? "" : "",
                icon = GpJson.TryGetString(json, "icon", out var icon) ? icon ?? "" : "",
                iconSmall = GpJson.TryGetString(json, "iconSmall", out var iconSmall) ? iconSmall ?? "" : "",
                price = GpJson.GetFloat(json, "price"),
                currency = GpJson.TryGetString(json, "currency", out var currency) ? currency ?? "" : "",
                currencySymbol = GpJson.TryGetString(json, "currencySymbol", out var symbol) ? symbol ?? "" : "",
                isSubscription = GpJson.GetBool(json, "isSubscription"),
                period = GpJson.GetInt(json, "period"),
                trialPeriod = GpJson.GetInt(json, "trialPeriod"),
                googlePlayId = GpJson.TryGetString(json, "googlePlayId", out var gpId) ? gpId ?? "" : "",
                onestoreId = GpJson.TryGetString(json, "onestoreId", out var osId) ? osId ?? "" : "",
                xsollaId = GpJson.TryGetString(json, "xsollaId", out var xsId) ? xsId ?? "" : ""
            };
            if (string.IsNullOrEmpty(product.currencySymbol))
                product.currencySymbol = product.currency;
            return product;
        }

        public static FetchPlayerPurchases ParsePurchase(string json, IList<FetchProducts> products)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            var productId = GpJson.GetInt(json, "productId");
            var tag = "";
            if (products != null)
            {
                foreach (var product in products)
                {
                    if (product != null && product.id == productId)
                    {
                        tag = product.tag ?? "";
                        break;
                    }
                }
            }

            var payload = GpJson.GetObject(json, "payload");
            if (string.IsNullOrEmpty(payload) && GpJson.TryGetString(json, "payload", out var payloadText))
                payload = payloadText ?? "";

            return new FetchPlayerPurchases
            {
                purchaseId = GpJson.TryGetString(json, "_id", out var id) ? id ?? "" : "",
                tag = tag,
                productId = productId,
                payload = payload ?? "",
                createdAt = GpJson.TryGetString(json, "createdAt", out var created) ? created ?? "" : "",
                expiredAt = GpJson.TryGetString(json, "expiredAt", out var expired) ? expired ?? "" : "",
                gift = GpJson.GetBool(json, "gift"),
                subscribed = GpJson.GetBool(json, "subscribed"),
                orderStatus = GpJson.TryGetString(json, "orderStatus", out var status) ? status ?? "" : ""
            };
        }

        public static bool IsPaid(FetchPlayerPurchases purchase)
        {
            if (purchase == null)
                return false;
            if (purchase.subscribed)
                return true;
            return string.Equals(purchase.orderStatus, OrderPaid, StringComparison.OrdinalIgnoreCase);
        }

        public static bool Matches(FetchProducts product, string idOrTag)
        {
            if (product == null || string.IsNullOrEmpty(idOrTag))
                return false;
            if (product.tag == idOrTag)
                return true;
            return int.TryParse(idOrTag, out var id) && product.id == id;
        }

        public static bool MatchesPurchase(FetchPlayerPurchases purchase, string idOrTag)
        {
            if (purchase == null || string.IsNullOrEmpty(idOrTag))
                return false;
            if (purchase.tag == idOrTag || purchase.purchaseId == idOrTag)
                return true;
            return int.TryParse(idOrTag, out var id) && purchase.productId == id;
        }

        public static string CheckoutUrl(string payloadJson)
        {
            var payload = string.IsNullOrEmpty(payloadJson) ? "" : payloadJson;
            if (!payload.TrimStart().StartsWith("{"))
                payload = "{" + payload + "}";
            if (GpJson.TryGetString(payload, "url", out var url) && !string.IsNullOrEmpty(url))
                return url;
            if (GpJson.TryGetString(payload, "token", out var token) && !string.IsNullOrEmpty(token))
            {
                var host = NativeCore.Payments.Sandbox
                    ? "https://sandbox-secure.xsolla.com/paystation4/?token="
                    : "https://secure.xsolla.com/paystation4/?token=";
                return host + token;
            }
            return "";
        }

        public static string DescribePayload(string payloadJson)
        {
            var payload = string.IsNullOrEmpty(payloadJson) ? "" : payloadJson.Trim();
            if (payload.Length == 0)
                return "payload=empty";
            var hasUrl = GpJson.TryGetString(payload, "url", out var url) && !string.IsNullOrEmpty(url);
            var hasToken = GpJson.TryGetString(payload, "token", out var token) && !string.IsNullOrEmpty(token);
            var hasSignature = GpJson.TryGetString(payload, "signature", out var signature) &&
                               !string.IsNullOrEmpty(signature);
            var keys = GpJson.ObjectKeys(payload);
            return "payloadLen=" + payload.Length +
                   " keys=" + (keys.Count == 0 ? "none" : string.Join(",", keys)) +
                   " url=" + (hasUrl ? "yes" : "no") +
                   " token=" + (hasToken ? "yes" : "no") +
                   " signature=" + (hasSignature ? "yes" : "no");
        }

        public static string DescribeCheckoutUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "checkoutUrl=empty";
            var host = "";
            var path = "";
            try
            {
                var uri = new Uri(url);
                host = uri.Host ?? "";
                path = uri.AbsolutePath ?? "";
            }
            catch
            {
                host = "unparsed";
            }

            var map = ParseQuery(url);
            var keys = map.Count == 0 ? "none" : string.Join(",", map.Keys);
            var outSum = QueryValue(map, "OutSum");
            return "host=" + host + " path=" + path + " keys=" + keys +
                   " OutSum=" + (string.IsNullOrEmpty(outSum) ? "none" : outSum);
        }

        public static string BuildRobokassaIframeHtml(string url)
        {
            var map = ParseQuery(url);
            if (map.Count == 0)
                return "";
            if (!map.ContainsKey("Culture"))
                map["Culture"] = "ru";
            if (!map.ContainsKey("Encoding"))
                map["Encoding"] = "utf-8";
            map["Settings"] = "{\"Mode\":\"widget\"}";

            var sb = new StringBuilder();
            sb.Append("<!DOCTYPE html><html><head><meta charset=\"utf-8\"/>");
            sb.Append("<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"/>");
            sb.Append("<style>html,body,#robokassa_iframe{margin:0;width:100%;height:100%;border:0}</style>");
            sb.Append("<script>");
            sb.Append("(function(){");
            sb.Append("function gpClose(){try{if(window.GpPayment)window.GpPayment.done('done');}catch(x){}}");
            sb.Append("window.addEventListener('message',function(e){");
            sb.Append("try{var data=e.data;if(typeof data==='string')data=JSON.parse(data);");
            sb.Append("if(data&&(data.type==='gs:pageReady'||data.type==='GS_PAYMENT_RESULT_MESSAGE'");
            sb.Append("||data.type==='requestToCloseWindow'||data.action==='closeRobokassaFrame'))gpClose();");
            sb.Append("}catch(x){}});");
            sb.Append("})();");
            sb.Append("</script></head><body>");
            sb.Append("<iframe id=\"robokassa_iframe\" name=\"robokassa_iframe\"></iframe>");
            sb.Append("<form id=\"gp_rk\" method=\"POST\" action=\"https://auth.robokassa.ru/merchant/v1/iframe\" target=\"robokassa_iframe\">");
            foreach (var pair in map)
            {
                sb.Append("<input type=\"hidden\" name=\"");
                sb.Append(HtmlAttr(pair.Key));
                sb.Append("\" value=\"");
                sb.Append(HtmlAttr(pair.Value));
                sb.Append("\"/>");
            }
            sb.Append("</form><script>document.getElementById('gp_rk').submit();</script>");
            sb.Append("</body></html>");
            return sb.ToString();
        }

        static Dictionary<string, string> ParseQuery(string url)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var query = ExtractQuery(url);
            if (string.IsNullOrEmpty(query))
                return map;
            query = query.Replace("&amp;", "&");
            var hash = query.IndexOf('#');
            if (hash >= 0)
                query = query.Substring(0, hash);
            var parts = query.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var eq = part.IndexOf('=');
                var name = eq >= 0 ? part.Substring(0, eq) : part;
                var raw = eq >= 0 ? part.Substring(eq + 1) : "";
                if (string.IsNullOrEmpty(name))
                    continue;
                try
                {
                    name = Uri.UnescapeDataString(name.Replace("+", " "));
                    raw = Uri.UnescapeDataString(raw.Replace("+", " "));
                }
                catch
                {
                }
                map[name] = raw;
            }
            return map;
        }

        static string ExtractQuery(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";
            var q = url.IndexOf('?');
            if (q >= 0)
                return url.Substring(q + 1);
            if (url.IndexOf("://", StringComparison.Ordinal) < 0 && url.IndexOf('=') >= 0)
                return url;
            try
            {
                return (new Uri(url).Query ?? "").TrimStart('?');
            }
            catch
            {
                return "";
            }
        }

        static string QueryValue(Dictionary<string, string> map, string key)
        {
            if (map == null || string.IsNullOrEmpty(key))
                return "";
            return map.TryGetValue(key, out var value) ? value ?? "" : "";
        }

        static string HtmlAttr(string value)
        {
            return (value ?? "")
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        public static List<FetchProducts> ParseProductList(List<string> items)
        {
            return ParseProducts(items);
        }

        static List<FetchProducts> ParseProducts(List<string> items)
        {
            var list = new List<FetchProducts>();
            if (items == null)
                return list;
            foreach (var item in items)
            {
                var product = ParseProduct(item);
                if (product != null && product.id > 0)
                    list.Add(product);
            }
            return list;
        }

        static List<FetchPlayerPurchases> ParsePurchases(List<string> items, List<FetchProducts> products)
        {
            var list = new List<FetchPlayerPurchases>();
            if (items == null)
                return list;
            foreach (var item in items)
            {
                var purchase = ParsePurchase(item, products);
                if (purchase != null && purchase.productId > 0)
                    list.Add(purchase);
            }
            return list;
        }

        static (FetchProducts product, FetchPlayerPurchases purchase) ParsePurchaseOutput(string json, string action)
        {
            var result = GpJson.GetObject(json, "result") ?? json;
            ThrowIfProblem(result, action);
            var product = ParseProduct(GpJson.GetObject(result, "product"));
            var purchase = ParsePurchase(GpJson.GetObject(result, "purchase"),
                product == null ? null : new List<FetchProducts> { product });
            if (product == null)
                throw new Exception("product_not_found");
            return (product, purchase);
        }

        static Dictionary<string, object> BuildIdOrTag(string idOrTag)
        {
            var input = new Dictionary<string, object>();
            if (int.TryParse(idOrTag, out var id) && id > 0)
                input["id"] = id;
            else
                input["tag"] = idOrTag ?? "";
            return input;
        }

        static void ThrowIfProblem(string json, string fallback)
        {
            if (IsProblem(json, out var message))
                throw new Exception(string.IsNullOrEmpty(message) ? fallback : message);
        }

        static bool IsProblem(string json, out string message)
        {
            message = "";
            if (string.IsNullOrEmpty(json))
                return false;
            if (!GpJson.TryGetString(json, "__typename", out var typeName) || typeName != "Problem")
                return false;
            message = GpJson.TryGetString(json, "message", out var msg) ? msg ?? "" : "";
            return true;
        }

        static bool IsNotFound(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;
            return message.IndexOf("ничего не найдено", StringComparison.OrdinalIgnoreCase) >= 0
                   || message.IndexOf("not found", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
