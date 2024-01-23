using UnityEngine;
using UnityEngine.UI;

using GamePush;
using Examples.Console;

namespace Examples.Payments.Subscriptions
{
    public class Subscriptions : MonoBehaviour
    {
        [SerializeField] private Button _isAvailableButton;
        [SerializeField] private Button _subscribeButton;
        [SerializeField] private Button _unsubscribeButton;

        private void OnEnable()
        {
            _isAvailableButton.onClick.AddListener(IsSubscriptionsAvailable);
            _subscribeButton.onClick.AddListener(Subscribe);
            _unsubscribeButton.onClick.AddListener(Unsubscribe);
        }
        private void OnDisable()
        {
            _isAvailableButton.onClick.RemoveListener(IsSubscriptionsAvailable);
            _subscribeButton.onClick.RemoveListener(Subscribe);
            _unsubscribeButton.onClick.RemoveListener(Unsubscribe);
        }


        public void IsSubscriptionsAvailable() => ConsoleUI.Instance.Log("IS SUBSCRIPTIONS AVAILABLE: " + GP_Payments.IsSubscriptionsAvailable());

        public void Subscribe() => GP_Payments.Subscribe("VIP", OnSubscribeSuccess, OnSubscribeError);
        public void Unsubscribe() => GP_Payments.Unsubscribe("VIP", OnUnsubscribeSuccess, OnUnsubscribeError);

        private void OnSubscribeSuccess(string idOrTag) => ConsoleUI.Instance.Log("SUBSCRIBE: SUCCESS: " + idOrTag);
        private void OnSubscribeError() => ConsoleUI.Instance.Log("SUBSCRIBE: ERROR");

        private void OnUnsubscribeSuccess(string idOrTag) => ConsoleUI.Instance.Log("UNSUBSCRIBE: SUCCESS: " + idOrTag);
        private void OnUnsubscribeError() => ConsoleUI.Instance.Log("UNSUBSCRIBE: ERROR");
    }
}
