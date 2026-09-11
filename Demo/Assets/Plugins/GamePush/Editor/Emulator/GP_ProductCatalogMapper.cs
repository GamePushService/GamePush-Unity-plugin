using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GamePush;

namespace GamePushEditor
{
    public static class GP_ProductCatalogMapper
    {
        public static List<FetchProducts> Parse(string text, Platform platform)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<FetchProducts>();

            string trimmed = text.Trim();
            if (LooksLikeCsv(trimmed))
                return MapRows(ParseCsv(trimmed), platform);

            return MapJson(trimmed, platform);
        }

        public static List<FetchProducts> MapGraphqlItems(string itemsJson, Platform platform)
        {
            if (string.IsNullOrWhiteSpace(itemsJson))
                return new List<FetchProducts>();

            object parsed = MiniJson.Deserialize(itemsJson);
            return MapParsed(parsed, platform);
        }

        private static bool LooksLikeCsv(string text)
        {
            int newline = text.IndexOfAny(new[] { '\r', '\n' });
            string header = newline >= 0 ? text.Substring(0, newline) : text;
            return header.IndexOf("names.ru", StringComparison.OrdinalIgnoreCase) >= 0
                || header.IndexOf("realPrices.", StringComparison.OrdinalIgnoreCase) >= 0
                || (header.StartsWith("id,", StringComparison.OrdinalIgnoreCase) && header.Contains("tag"));
        }

        private static List<FetchProducts> MapJson(string json, Platform platform)
        {
            if (LooksLikeSdkArray(json))
            {
                List<FetchProducts> sdk = UtilitySafeSdkList(json);
                if (sdk.Count > 0)
                    return sdk;
            }

            object parsed = MiniJson.Deserialize(json);
            return MapParsed(parsed, platform);
        }

        private static bool LooksLikeSdkArray(string json)
        {
            return json.StartsWith("[", StringComparison.Ordinal)
                && json.IndexOf("\"name\"", StringComparison.Ordinal) >= 0
                && json.IndexOf("\"names\"", StringComparison.Ordinal) < 0;
        }

        private static List<FetchProducts> UtilitySafeSdkList(string json)
        {
            try
            {
                List<FetchProducts> list = GamePush.Utilities.UtilityJSON.GetList<FetchProducts>(json);
                return list ?? new List<FetchProducts>();
            }
            catch
            {
                return new List<FetchProducts>();
            }
        }

        private static List<FetchProducts> MapParsed(object parsed, Platform platform)
        {
            var rows = new List<Dictionary<string, string>>();
            CollectRows(parsed, rows, "");
            return MapRows(rows, platform);
        }

        private static void CollectRows(object parsed, List<Dictionary<string, string>> rows, string path)
        {
            switch (parsed)
            {
                case List<object> list:
                    foreach (object item in list)
                        CollectRows(item, rows, path);
                    return;
                case Dictionary<string, object> obj:
                    if (obj.TryGetValue("items", out object items))
                    {
                        CollectRows(items, rows, path);
                        return;
                    }

                    if (obj.TryGetValue("data", out object data) && !IsProductDict(obj))
                    {
                        CollectRows(data, rows, path);
                        return;
                    }

                    if (obj.TryGetValue("result", out object result) && !IsProductDict(obj))
                    {
                        CollectRows(result, rows, path);
                        return;
                    }

                    if (IsProductDict(obj))
                    {
                        rows.Add(Flatten(obj, ""));
                        return;
                    }

                    foreach (KeyValuePair<string, object> pair in obj)
                    {
                        if (pair.Value is Dictionary<string, object> nested && IsProductDict(nested))
                            rows.Add(Flatten(nested, ""));
                    }

                    return;
            }
        }

        private static bool IsProductDict(Dictionary<string, object> obj)
        {
            return obj.ContainsKey("id")
                || obj.ContainsKey("tag")
                || obj.ContainsKey("names")
                || obj.ContainsKey("prices")
                || obj.ContainsKey("name");
        }

        private static Dictionary<string, string> Flatten(Dictionary<string, object> obj, string prefix)
        {
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            FlattenInto(obj, prefix, row);
            return row;
        }

        private static void FlattenInto(Dictionary<string, object> obj, string prefix, Dictionary<string, string> row)
        {
            foreach (KeyValuePair<string, object> pair in obj)
            {
                string key = string.IsNullOrEmpty(prefix) ? pair.Key : prefix + "." + pair.Key;
                switch (pair.Value)
                {
                    case Dictionary<string, object> nested:
                        FlattenInto(nested, key, row);
                        break;
                    case null:
                        row[key] = string.Empty;
                        break;
                    default:
                        row[key] = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
                        break;
                }
            }
        }

        private static List<FetchProducts> MapRows(List<Dictionary<string, string>> rows, Platform platform)
        {
            var products = new List<FetchProducts>(rows.Count);
            foreach (Dictionary<string, string> row in rows)
            {
                FetchProducts product = MapRow(row, platform);
                if (product.id != 0 || !string.IsNullOrEmpty(product.tag))
                    products.Add(product);
            }

            return products;
        }

        private static FetchProducts MapRow(Dictionary<string, string> row, Platform platform)
        {
            var product = new FetchProducts
            {
                id = ReadInt(row, "id"),
                tag = ReadString(row, "tag"),
                icon = ReadString(row, "icon"),
                iconSmall = FirstNonEmpty(ReadString(row, "iconSmall"), ReadString(row, "icon")),
                name = FirstNonEmpty(ReadString(row, "names.ru"), ReadString(row, "names.en"), ReadString(row, "name")),
                description = FirstNonEmpty(ReadString(row, "descriptions.ru"), ReadString(row, "descriptions.en"), ReadString(row, "description")),
                isSubscription = ReadBool(row, "isSubscription"),
                period = ReadInt(row, "period"),
                trialPeriod = ReadInt(row, "trialPeriod"),
                googlePlayId = ReadString(row, "googlePlayId"),
                onestoreId = ReadString(row, "onestoreId"),
                xsollaId = ReadString(row, "xsollaId")
            };

            AssignPrice(product, row, platform);
            return product;
        }

        private static void AssignPrice(FetchProducts product, Dictionary<string, string> row, Platform platform)
        {
            string platformKey = "prices." + PricePlatformKey(platform);
            if (TryGetPositive(row, platformKey, out float platformPrice))
            {
                SetPrice(product, platformPrice, CurrencyForPlatform(platform), row);
                return;
            }

            if (TryGetPositive(row, "realPrices.RUB", out float rub))
            {
                SetPrice(product, rub, "RUB", row);
                return;
            }

            foreach (KeyValuePair<string, string> pair in row)
            {
                if (!pair.Key.StartsWith("prices.", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!TryParseFloat(pair.Value, out float value) || value <= 0f)
                    continue;

                SetPrice(product, value, pair.Key.Substring("prices.".Length), row);
                return;
            }

            if (TryGetPositive(row, "basePrice", out float basePrice))
            {
                SetPrice(product, basePrice, FirstNonEmpty(ReadString(row, "baseCurrency"), "GP"), row);
                return;
            }

            if (TryGetPositive(row, "baseRealPrice", out float baseReal))
            {
                SetPrice(product, baseReal, FirstNonEmpty(ReadString(row, "baseRealCurrency"), "RUB"), row);
                return;
            }

            if (TryGetPositive(row, "price", out float flat))
                SetPrice(product, flat, FirstNonEmpty(ReadString(row, "currency"), "GP"), row);
        }

        private static void SetPrice(FetchProducts product, float value, string currency,
            Dictionary<string, string> row)
        {
            product.price = value;
            product.currency = currency ?? "";
            product.currencySymbol = FirstNonEmpty(
                ReadString(row, "currencySymbol"),
                product.currency);
        }

        private static string PricePlatformKey(Platform platform)
        {
            return platform == Platform.ARCADIUM ? "ARKADIUM" : platform.ToString();
        }

        private static string CurrencyForPlatform(Platform platform)
        {
            switch (platform)
            {
                case Platform.YANDEX: return "YAN";
                case Platform.VK: return "VK";
                case Platform.OK: return "OK";
                default: return platform.ToString();
            }
        }

        private static bool TryGetPositive(Dictionary<string, string> row, string key, out float value)
        {
            value = 0f;
            return TryGet(row, key, out string raw) && TryParseFloat(raw, out value) && value > 0f;
        }

        private static bool TryParseFloat(string raw, out float value)
        {
            return float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static int ReadInt(Dictionary<string, string> row, string key)
        {
            return TryGet(row, key, out string raw) && int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                ? value
                : 0;
        }

        private static bool ReadBool(Dictionary<string, string> row, string key)
        {
            if (!TryGet(row, key, out string raw))
                return false;
            return raw == "1" || raw.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        private static string ReadString(Dictionary<string, string> row, string key)
        {
            return TryGet(row, key, out string value) ? value : string.Empty;
        }

        private static bool TryGet(Dictionary<string, string> row, string key, out string value)
        {
            if (row.TryGetValue(key, out value))
                return true;
            foreach (KeyValuePair<string, string> pair in row)
            {
                if (pair.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    value = pair.Value;
                    return true;
                }
            }

            value = null;
            return false;
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }

            return string.Empty;
        }

        private static List<Dictionary<string, string>> ParseCsv(string text)
        {
            var rows = new List<Dictionary<string, string>>();
            List<string> lines = SplitCsvLines(text);
            if (lines.Count < 2)
                return rows;

            List<string> headers = SplitCsvLine(lines[0]);
            for (int i = 1; i < lines.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                List<string> cells = SplitCsvLine(lines[i]);
                var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int c = 0; c < headers.Count; c++)
                {
                    string header = headers[c].Trim();
                    if (string.IsNullOrEmpty(header))
                        continue;
                    row[header] = c < cells.Count ? cells[c] : string.Empty;
                }

                rows.Add(row);
            }

            return rows;
        }

        private static List<string> SplitCsvLines(string text)
        {
            var lines = new List<string>();
            var current = new StringBuilder();
            bool quoted = false;
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch == '"')
                {
                    quoted = !quoted;
                    current.Append(ch);
                    continue;
                }

                if (!quoted && (ch == '\n' || ch == '\r'))
                {
                    if (ch == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
                        i++;
                    lines.Add(current.ToString());
                    current.Length = 0;
                    continue;
                }

                current.Append(ch);
            }

            if (current.Length > 0)
                lines.Add(current.ToString());
            return lines;
        }

        private static List<string> SplitCsvLine(string line)
        {
            var cells = new List<string>();
            var current = new StringBuilder();
            bool quoted = false;
            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if (ch == '"')
                {
                    if (quoted && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        quoted = !quoted;
                    }

                    continue;
                }

                if (ch == ',' && !quoted)
                {
                    cells.Add(current.ToString());
                    current.Length = 0;
                    continue;
                }

                current.Append(ch);
            }

            cells.Add(current.ToString());
            return cells;
        }

        private static class MiniJson
        {
            public static object Deserialize(string json)
            {
                int index = 0;
                return ParseValue(json, ref index);
            }

            private static object ParseValue(string json, ref int i)
            {
                SkipWs(json, ref i);
                if (i >= json.Length)
                    return null;

                char ch = json[i];
                if (ch == '{')
                    return ParseObject(json, ref i);
                if (ch == '[')
                    return ParseArray(json, ref i);
                if (ch == '"')
                    return ParseString(json, ref i);
                if (ch == 't' || ch == 'f')
                    return ParseBool(json, ref i);
                if (ch == 'n')
                {
                    i += 4;
                    return null;
                }

                return ParseNumber(json, ref i);
            }

            private static Dictionary<string, object> ParseObject(string json, ref int i)
            {
                var obj = new Dictionary<string, object>();
                i++;
                while (i < json.Length)
                {
                    SkipWs(json, ref i);
                    if (i < json.Length && json[i] == '}')
                    {
                        i++;
                        break;
                    }

                    string key = ParseString(json, ref i);
                    SkipWs(json, ref i);
                    i++;
                    obj[key] = ParseValue(json, ref i);
                    SkipWs(json, ref i);
                    if (i < json.Length && json[i] == ',')
                        i++;
                }

                return obj;
            }

            private static List<object> ParseArray(string json, ref int i)
            {
                var list = new List<object>();
                i++;
                while (i < json.Length)
                {
                    SkipWs(json, ref i);
                    if (i < json.Length && json[i] == ']')
                    {
                        i++;
                        break;
                    }

                    list.Add(ParseValue(json, ref i));
                    SkipWs(json, ref i);
                    if (i < json.Length && json[i] == ',')
                        i++;
                }

                return list;
            }

            private static string ParseString(string json, ref int i)
            {
                var sb = new StringBuilder();
                i++;
                while (i < json.Length)
                {
                    char ch = json[i++];
                    if (ch == '"')
                        break;
                    if (ch == '\\' && i < json.Length)
                    {
                        char esc = json[i++];
                        sb.Append(esc == 'n' ? '\n' : esc == 'r' ? '\r' : esc == 't' ? '\t' : esc);
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                }

                return sb.ToString();
            }

            private static bool ParseBool(string json, ref int i)
            {
                if (json[i] == 't')
                {
                    i += 4;
                    return true;
                }

                i += 5;
                return false;
            }

            private static double ParseNumber(string json, ref int i)
            {
                int start = i;
                if (i < json.Length && json[i] == '-')
                    i++;
                while (i < json.Length && (char.IsDigit(json[i]) || json[i] == '.' || json[i] == 'e' || json[i] == 'E' || json[i] == '+'))
                    i++;
                double.TryParse(json.Substring(start, i - start), NumberStyles.Float, CultureInfo.InvariantCulture, out double value);
                return value;
            }

            private static void SkipWs(string json, ref int i)
            {
                while (i < json.Length && char.IsWhiteSpace(json[i]))
                    i++;
            }
        }
    }
}
