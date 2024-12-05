using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;

namespace GamePush.Core
{
    public class SystemModule
    {
        private bool _isAllowedOrigin;
        private bool _isDev;

        public bool IsAllowedOrigin() => _isAllowedOrigin;
        public bool IsDev() => _isDev;

        public SystemModule()
        {

        }

        public void Init(AllConfigData data)
        {
            _isAllowedOrigin = data.isAllowedOrigin;
            _isDev = data.isDev;
        }
    }
}
