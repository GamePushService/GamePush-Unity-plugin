using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Examples.Console;
using UnityEngine.UI;
using TMPro;
using GamePush;

namespace Examples.Images
{
    public class Images : MonoBehaviour
    {
        [SerializeField] private Button _fetchButton;
        [SerializeField] private Button _checkButton;

        private void OnEnable()
        {
            _fetchButton.onClick.AddListener(Fetch);
        }

        private void OnDisable()
        {
            _fetchButton.onClick.RemoveListener(Fetch);
        }

        public void Fetch() => GP_Images.Fetch();
    }
}
