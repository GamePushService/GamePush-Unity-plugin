using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GamePush;
using Examples.Console;

namespace Examples.Socials
{
    public class Socials : MonoBehaviour
    {
        private readonly string SHARE_TEXT = "Мне удалось нажать 73 раза по квадрату за 5 секунд! Сможешь меня обогнать?";
        private string URL => GP_App.Url();
        private readonly string IMAGE = "https://gamepush.com/img/ogimage.png";

        [SerializeField] private Button _shareButton;
        [SerializeField] private Button _postButton;
        [SerializeField] private Button _inviteButton;
        [SerializeField] private Button _joinCommunityButton;
        [SerializeField] private Button _canJoinCommunityButton;
        [SerializeField] private Button _communityLinkButton;
        [SerializeField] private Button _isButton;
        [SerializeField] private Button _makeLinkButton;
        [SerializeField] private Button _shareParamsButton;
        [Space]
        [SerializeField] private TMP_InputField _shareLinkContent;
        [SerializeField] private TMP_InputField _shareLinkField;


        private void OnEnable()
        {
            _shareButton.onClick.AddListener(Share);
            _postButton.onClick.AddListener(Post);
            _inviteButton.onClick.AddListener(Invite);
            _joinCommunityButton.onClick.AddListener(JoinCommunity);
            _canJoinCommunityButton.onClick.AddListener(CanJoinCommunity);
            _communityLinkButton.onClick.AddListener(CommunityLink);
            _isButton.onClick.AddListener(Is);
            _makeLinkButton.onClick.AddListener(MakeShareLink);
            _shareParamsButton.onClick.AddListener(GetShareParams);

            GP_Socials.OnShare += OnShare;
            GP_Socials.OnPost += OnPost;
            GP_Socials.OnInvite += OnInvite;
            GP_Socials.OnJoinCommunity += OnJoinCommunity;
        }

        private void OnDisable()
        {
            _shareButton.onClick.RemoveListener(Share);
            _postButton.onClick.RemoveListener(Post);
            _inviteButton.onClick.RemoveListener(Invite);
            _joinCommunityButton.onClick.RemoveListener(JoinCommunity);
            _canJoinCommunityButton.onClick.RemoveListener(CanJoinCommunity);
            _communityLinkButton.onClick.RemoveListener(CommunityLink);
            _isButton.onClick.RemoveListener(Is);
            _makeLinkButton.onClick.RemoveAllListeners();
            _shareButton.onClick.RemoveAllListeners();

            GP_Socials.OnShare -= OnShare;
            GP_Socials.OnPost -= OnPost;
            GP_Socials.OnInvite -= OnInvite;
            GP_Socials.OnJoinCommunity -= OnJoinCommunity;
        }


        public void Share() => GP_Socials.Share(SHARE_TEXT, URL, IMAGE);
        public void Post() => GP_Socials.Post(SHARE_TEXT);
        public void Invite() => GP_Socials.Invite();
        public void JoinCommunity() => GP_Socials.JoinCommunity();
        public void CommunityLink() => ConsoleUI.Instance.Log("COMMUNITY LINK: " + GP_Socials.CommunityLink());
        public void CanJoinCommunity() => ConsoleUI.Instance.Log("CAN JOIN COMMUNITY: " + GP_Socials.CanJoinCommunity());
        public void Is()
        {
            ConsoleUI.Instance.Log("IS SUPPORTS SHARE: " + GP_Socials.IsSupportsShare());
            ConsoleUI.Instance.Log("IS SUPPORTS NATIVE COMMUNITY JOIN: " + GP_Socials.IsSupportsNativeCommunityJoin());
            ConsoleUI.Instance.Log("IS SUPPORTS NATIVE INVITE: " + GP_Socials.IsSupportsNativeInvite());
            ConsoleUI.Instance.Log("IS SUPPORTS NATIVE POSTS: " + GP_Socials.IsSupportsNativePosts());
            ConsoleUI.Instance.Log("IS SUPPORTS NATIVE SHARE: " + GP_Socials.IsSupportsNativeShare());
        }

        public void MakeShareLink()
        {
            string content = _shareLinkContent.text;
            ConsoleUI.Instance.Log("CREATE LINK WITH CONTENT: " + content);
            string link = GP_Socials.MakeShareLink(content);
            _shareLinkField.text = link;
            ConsoleUI.Instance.Log("SHARE LINK: \n" + link);
        }
        public void GetShareParams()
        {
            ConsoleUI.Instance.Log("From Player: " + GP_Socials.GetSharePlayerID());
            ConsoleUI.Instance.Log("Content: " + GP_Socials.GetShareContent());
        }

        private void OnShare(bool success) => ConsoleUI.Instance.Log("SOCIALS: SHARE: " + success);
        private void OnPost(bool success) => ConsoleUI.Instance.Log("SOCIALS: ON POST: " + success);
        private void OnInvite(bool success) => ConsoleUI.Instance.Log("SOCIALS: ON INVITE: " + success);
        private void OnJoinCommunity(bool success) => ConsoleUI.Instance.Log("SOCIALS: ON JOIN COMMUNITY: " + success);
    }
}