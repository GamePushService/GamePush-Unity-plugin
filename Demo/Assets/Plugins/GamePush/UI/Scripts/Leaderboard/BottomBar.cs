using System.Text.RegularExpressions;
using UnityEngine;

namespace GamePush.UI
{
    public class BottomBar : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text _inviteDivider, _shareRecord;
        [SerializeField]
        private UnityEngine.UI.Button _shareButton;

        private string _playerPosition;

        void Start()
        {
            _inviteDivider.text = CoreSDK.language.localization.leaderboard.inviteDivider;
            _shareRecord.text = CoreSDK.language.localization.leaderboard.shareRecord;

            _shareButton.onClick.AddListener(() => ShareRecord());
        }

        public void SetPlayerPosition(int position) => _playerPosition = position.ToString();
        public void SetPlayerPosition(string position) => _playerPosition = position;

        public void SetButtonText(bool playerInBoard)
        {
            _shareRecord.text = playerInBoard ?
                CoreSDK.language.localization.leaderboard.shareRecord :
                CoreSDK.language.localization.leaderboard.inviteFriends;
        }

        private void ShareRecord()
        {
            string shareRecord = _shareRecord.text;
            string shareRecordText = CoreSDK.language.localization.leaderboard.shareRecordText;

            string gameTitle = CoreSDK.app.Title();

            shareRecordText = Regex.Replace(shareRecordText, @"\{\{player\.position\}\}", _playerPosition);
            shareRecordText = Regex.Replace(shareRecordText, @"\{\{game\.title\}\}", gameTitle);

            //print(shareRecordText);

            CoreSDK.socials.OpenPanel(shareRecord, shareRecordText);
        }
    }
}
