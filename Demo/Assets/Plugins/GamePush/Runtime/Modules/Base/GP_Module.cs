using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    public class GP_Module : MonoBehaviour, IGP_Module
    {
        private static ModuleName _name = ModuleName.None;
        protected static ModuleName Name
        {
            get => _name;
            set => _name = value;
        }

        protected static void SetModuleName(ModuleName name) =>
            Name = name;

        protected static void ConsoleLog(string log) =>
            GP_Logger.ModuleLog(Name, log);
    }
}
