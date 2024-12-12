using UnityEngine;

namespace GamePush.UI
{
    public class BottomBar : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text _inviteDivider, _inviteFriends;

        void Start()
        {
            _inviteDivider.text = CoreSDK.language.localization.leaderboard.inviteDivider;
            _inviteFriends.text = CoreSDK.language.localization.leaderboard.inviteFriends;
        }
    }
}
