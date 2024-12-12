using UnityEngine;

namespace GamePush.UI
{
    public class Divider : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text _nearestPlayers;

        void Start()
        {
            _nearestPlayers.text = CoreSDK.language.localization.leaderboard.nearestPlayers;
        }
    }
}
