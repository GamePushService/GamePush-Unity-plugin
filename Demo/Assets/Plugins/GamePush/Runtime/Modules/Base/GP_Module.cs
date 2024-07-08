using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    public class GP_Module : MonoBehaviour, IGP_Module
    {
        private static ModuleName _name;
        public static ModuleName Name
        {
            get => _name;
            set => _name = value;
        }
    }
}
