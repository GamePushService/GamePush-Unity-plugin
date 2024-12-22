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


        public void Register(string tag, string value)
        {
            
        }

        public string Get(string tag)
        {

            return null;
        }

        public UniquesData[] List()
        {

            return null;

        }

        public async void Check(string tag, string value)
        {
            UniquesData data = new UniquesData(tag, value);

            bool result = await DataFetcher.CheckUniqueValue(data, false);
            if (result)
                OnUniqueValueCheck?.Invoke(data);
            else
                OnUniqueValueCheckError?.Invoke("Already exist");
        }

        public void Delete(string tag)
        {
           
        }
    }
}
