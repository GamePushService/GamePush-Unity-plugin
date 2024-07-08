using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    public class GP_Module : MonoBehaviour, IGP_Module
    {
        private ModuleName _name;
        public ModuleName Name
        {
            get => _name;
            set => _name = value;
        }
    }
}
