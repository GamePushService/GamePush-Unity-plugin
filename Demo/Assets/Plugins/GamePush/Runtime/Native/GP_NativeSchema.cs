using System.Collections.Generic;

namespace GamePush.Native
{
    public static class GP_NativeSchema
    {
        public static string FilterReadonly(string state, string existing, string schemaJson)
        {
            if (string.IsNullOrEmpty(schemaJson) || schemaJson == "{}")
                return state;
            var schema = GpJson.ParseObject(schemaJson);
            if (schema == null || schema.Count == 0)
                return state;
            var filtered = FilterReadonlyFields(GpJson.Parse(state), GpJson.Parse(existing), schema);
            return GpJson.Stringify(filtered);
        }

        public static object FilterReadonlyFields(object state, object existingState, Dictionary<string, object> schema)
        {
            if (state == null || schema == null)
                return state;
            if (state is List<object> list)
            {
                var existingList = existingState as List<object>;
                var result = new List<object>(list.Count);
                for (var i = 0; i < list.Count; i++)
                {
                    var existingItem = existingList != null && i < existingList.Count ? existingList[i] : null;
                    var item = list[i];
                    result.Add(item is Dictionary<string, object>
                        ? FilterReadonlyFields(item, existingItem, schema)
                        : item);
                }
                return result;
            }

            if (!(state is Dictionary<string, object> stateObj))
                return state;
            var existingObj = existingState as Dictionary<string, object>;
            var filtered = new Dictionary<string, object>();
            foreach (var pair in stateObj)
            {
                schema.TryGetValue(pair.Key, out var field);
                object existingValue = null;
                var hasExisting = existingObj != null && existingObj.TryGetValue(pair.Key, out existingValue);
                if (IsReadonlyField(field) && hasExisting)
                {
                    filtered[pair.Key] = existingValue;
                    continue;
                }

                if (pair.Value is Dictionary<string, object> || pair.Value is List<object>)
                {
                    var nested = NestedSchema(field);
                    filtered[pair.Key] = FilterReadonlyFields(pair.Value, existingValue, nested);
                }
                else
                    filtered[pair.Key] = pair.Value;
            }
            return filtered;
        }

        static bool IsReadonlyField(object field)
        {
            return field is Dictionary<string, object> obj
                   && obj.TryGetValue("readonly", out var flag)
                   && flag is bool readonlyFlag
                   && readonlyFlag;
        }

        static Dictionary<string, object> NestedSchema(object field)
        {
            if (!(field is Dictionary<string, object> obj))
                return new Dictionary<string, object>();
            var nested = new Dictionary<string, object>();
            foreach (var pair in obj)
            {
                if (pair.Key == "readonly" || pair.Key == "interpolate")
                    continue;
                nested[pair.Key] = pair.Value;
            }
            return nested.Count == 0 ? obj : nested;
        }
    }
}
