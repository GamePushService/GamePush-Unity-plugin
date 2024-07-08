using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    public interface IGP_Module
    {
        ModuleName Name
        {
            get;
            set;
        }
    }
}
