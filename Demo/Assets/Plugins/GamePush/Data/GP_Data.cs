using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Utilities;

namespace GamePush
{
    public class GP_Data
    {
        private string _data;
        public string Data => _data;

        public GP_Data(string data) => _data = data;

        public T Get<T>() => UtilityJSON.Get<T>(_data);
        public List<T> GetList<T>() => UtilityJSON.GetList<T>(_data);
        public T[] GetArray<T>() => UtilityJSON.GetArray<T>(_data);
    }
}
