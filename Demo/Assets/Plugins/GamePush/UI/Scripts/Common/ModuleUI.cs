using System;
using UnityEngine;

namespace GamePush.UI
{
    public class ModuleUI : MonoBehaviour, IModuleUI
    {
        public event Action OnOpen;
        public event Action OnClose;

        public virtual void Show()
        {
            OnOpen?.Invoke();
        }

        public virtual void Close()
        {
            OnClose?.Invoke();
            OverlayCanvas.Controller.Close();
        }
    }
}
