using UnityEngine;
using GamePush;
using TMPro;

namespace Examples.Channel.Info
{
    public class Info : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerId;

        private void Start() => _playerId.text = GP_Player.GetID().ToString();
    }
}
