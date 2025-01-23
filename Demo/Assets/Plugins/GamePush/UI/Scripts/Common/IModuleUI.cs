using System;

namespace GamePush.UI
{
    public interface IModuleUI
    {
        public event Action OnOpen;
        public event Action OnClose;
        public void Show();
        public void Close();
    }
}

