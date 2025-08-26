using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.ConsoleController
{
    [Serializable]
    public class ModuleLogSwitch
    {
        public ModuleName moduleName;
        public bool showLogs;

        public ModuleLogSwitch(ModuleName moduleName, bool showLogs)
        {
            this.moduleName = moduleName;
            this.showLogs = showLogs;
        }
    }
    
    public class GP_ConsoleController : MonoBehaviour
    {
        public static GP_ConsoleController Instance;
        [SerializeField] public List<ModuleLogSwitch> logSwitches;

        private void Awake()
        {
            Instance = this;

            logSwitches = new List<ModuleLogSwitch>();
            foreach (var module in Enum.GetValues(typeof(ModuleName)))
            {
                logSwitches.Add(new ModuleLogSwitch((ModuleName)module, true));
            }
        }

        public void SwitchAll(bool value)
        {
            foreach (var logSwitch in logSwitches)
            {
                logSwitch.showLogs = value;
            }
        }

        public bool IsModuleLogs(ModuleName moduleName)
        {
            if (GP_Settings.instance.viewLogs)
            {
                foreach (var logSwitch in logSwitches)
                {
                    if (logSwitch.moduleName == moduleName)
                        return logSwitch.showLogs;
                }
            }
            
            return false;
        }
    }


}