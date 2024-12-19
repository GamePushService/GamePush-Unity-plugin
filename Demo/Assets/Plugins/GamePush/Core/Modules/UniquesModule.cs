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

        public void Check(string tag, string value)
        { 
            
        }

        public void Delete(string tag)
        {
           
        }
    }
}
