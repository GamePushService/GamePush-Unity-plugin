using System;
using System.Collections.Generic;
using GamePush;

namespace GamePush.Core
{
    public class UniquesModule
    {

        public event Action<UniquesData> OnUniqueValueRegister;
        public event Action<string> OnUniqueValueRegisterError;
        public event Action<UniquesData> OnUniqueValueCheck;
        public event Action<string> OnUniqueValueCheckError;
        public event Action<UniquesData> OnUniqueValueDelete;
        public event Action<string> OnUniqueValueDeleteError;

        private List<UniquesData> uniquesList;
        private Dictionary<string, string> uniquesDict;

        private static string SUCCESS_TAG = "gp_success";
        private static string ERROR_TAG = "gp_error";

        public void Init()
        {
            uniquesDict = new Dictionary<string, string>();
            uniquesList = new List<UniquesData>();
        }

        public void SetUniques(List<UniquesData> uniquesData)
        {
            uniquesList = uniquesData;
            uniquesDict = new Dictionary<string, string>();
            foreach (UniquesData data in uniquesList)
            {
                uniquesDict.TryAdd(data.tag, data.value);
            }
        }

        private void AddUnique(UniquesData uniquesData)
        {
            if (uniquesDict.ContainsKey(uniquesData.tag))
                RemoveUnique(uniquesData);

            uniquesList.Add(uniquesData);
            uniquesDict.TryAdd(uniquesData.tag, uniquesData.value);
        }

        private void RemoveUnique(UniquesData uniquesData)
        {
            uniquesList.RemoveAll(item => item.tag == uniquesData.tag && item.value == uniquesData.value);
            uniquesDict.Remove(uniquesData.tag);
        }

        public string Get(string tag)
        {
            if (uniquesDict.TryGetValue(tag, out string value))
                return value;
            return null;
        }

        public UniquesData[] List()
        {
            if(uniquesList.Count > 0)
                return uniquesList.ToArray();

            return null;
        }

        public async void Check(string tag, string value)
        {
            UniquesData data = new UniquesData(tag, value);

            TagValueData result = await DataFetcher.CheckUniqueValue(data, false);
            if (result.tag == ERROR_TAG)
                OnUniqueValueCheckError?.Invoke(result.value.ToString());

            if (result.tag == SUCCESS_TAG && (bool)result.value)
                OnUniqueValueCheck?.Invoke(data);
            else
                OnUniqueValueCheckError?.Invoke("Already exist");
        }

        public async void Register(string tag, string value)
        {
            UniquesData data = new UniquesData(tag, value);

            TagValueData result = await DataFetcher.RegisterUniqueValue(data, false);
            
            if (result.tag == ERROR_TAG)
                OnUniqueValueRegisterError?.Invoke(result.value.ToString());

            if (result.tag == SUCCESS_TAG)
            {
                AddUnique(data);
                OnUniqueValueRegister?.Invoke(data);
            }
        }

        public async void Delete(string tag)
        {
            UniquesData data = new UniquesData(tag, Get(tag));
            if(data.value == null)
            {
                OnUniqueValueDeleteError?.Invoke("No such value on this player");
                return;
            }

            TagValueData result = await DataFetcher.DeleteUniqueValue(new TagData(tag), false);

            if (result.tag == ERROR_TAG)
                OnUniqueValueDeleteError?.Invoke(result.value.ToString());

            if (result.tag == SUCCESS_TAG && (bool)result.value)
            {
                RemoveUnique(data);
                OnUniqueValueDelete?.Invoke(data);
            }
            else
                OnUniqueValueDeleteError?.Invoke("Value not exist");
        }
    }
}
