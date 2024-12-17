using System.Text.RegularExpressions;
using UnityEngine;

namespace GamePush.UI
{
    public class BottomBar : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text _inviteDivider, _inviteFriends;
        [SerializeField]
        private UnityEngine.UI.Button _shareButton;

        private string _playerPosition;

        void Start()
        {
            _inviteDivider.text = CoreSDK.language.localization.leaderboard.inviteDivider;
            _inviteFriends.text = CoreSDK.language.localization.leaderboard.inviteFriends;

            _shareButton.onClick.AddListener(() => ShareRecord());
        }

        public void SetPlayerPosition(int position) => _playerPosition = position.ToString();
        public void SetPlayerPosition(string position) => _playerPosition = position;

        private void ShareRecord()
        {
            string shareRecord = CoreSDK.language.localization.leaderboard.shareRecord;
            string shareRecordText = CoreSDK.language.localization.leaderboard.shareRecordText;

            string gameTitle = CoreSDK.app.Title();

            shareRecordText = Regex.Replace(shareRecordText, @"\{\{player\.position\}\}", _playerPosition);
            shareRecordText = Regex.Replace(shareRecordText, @"\{\{game\.title\}\}", gameTitle);

            //print(shareRecordText);

            CoreSDK.socials.OpenPanel(shareRecord, shareRecordText);
        }
    }
}
