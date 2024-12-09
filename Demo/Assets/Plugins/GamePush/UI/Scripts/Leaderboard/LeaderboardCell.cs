using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GamePush.UI
{
    public class LeaderboardCell : MonoBehaviour
    {
        [Header("Cell custom elements")]
        [Space(10)]
        [SerializeField] private Image _cellBack;
        [SerializeField] private Color _defaultBack;
        [SerializeField] private Color _playerBack;

        [Space(10)]
        [SerializeField] private Image _numBack;
        [SerializeField] private Color _defaultNum;
        [SerializeField] private Color _playerNum;

        [Space]
        [Header("Cell info elements")]
        [Space(10)]
        [SerializeField] private GameObject _avatarHolder;
        [SerializeField] private Image _avatarImage;
        [SerializeField] private GameObject _textHolder;
        [SerializeField] private TMP_Text _playerName, _playerFields;
        [SerializeField] private TMP_Text _placeNum;

        [Space]
        [Header("Place tags objects")]
        [Space(10)]
        [SerializeField] private GameObject _defaultPlace;
        [SerializeField] private GameObject _firstPlace;
        [SerializeField] private GameObject _secondPlace;
        [SerializeField] private GameObject _thirdPlace;

        private LeaderboardUI _leaderboard;
        private bool _isPlayerCell;
        public bool IsPlayerCell() => _isPlayerCell;

        public async Task Init(LeaderboardUI leaderboard, PlayerRatingState playerState, bool isMyPlayer)
        {
            _leaderboard = leaderboard;

            _playerName.text = playerState.name;
            _playerFields.text = playerState.fields;

            SetCustomization(isMyPlayer);
            SetPlace(playerState.position);

            bool hasAvatar = await DownloadImageAsync(playerState.avatar, _avatarImage);
            if (!hasAvatar)
                _avatarImage.enabled = false;
        }

        public void InitEmpty()
        {
            _avatarHolder.SetActive(false);
            _textHolder.SetActive(false);
            _defaultPlace.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            print(collision.name);
        }


        private void SetCustomization(bool isPlayer)
        {
            _cellBack.color = isPlayer? _playerBack : _defaultBack;
            _numBack.color = isPlayer ? _playerNum : _defaultNum;

            //print("My player: " + isPlayer);
            _isPlayerCell = isPlayer;
        }

        private void DeactivePlaces()
        {
            _firstPlace.SetActive(false);
            _secondPlace.SetActive(false);
            _thirdPlace.SetActive(false);
            _defaultPlace.SetActive(false);
        }

        private void SetPlace(int position)
        {
            DeactivePlaces();

            //Debug.Log("Set position " + position.ToString());

            switch (position)
            {
                case 1:
                    _firstPlace.SetActive(true);
                    return;
                case 2:
                    _secondPlace.SetActive(true);
                    return;
                case 3:
                    _thirdPlace.SetActive(true);
                    return;
                default:
                    _defaultPlace.SetActive(true);
                    _placeNum.text = position.ToString();
                    return;
            }
        }

        public async Task<bool> DownloadImageAsync(string url, Image image)
        {
            if (url == "" || url == null)
                return false;

            var request = UnityWebRequestTexture.GetTexture(url);

            AsyncOperation operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D _texture2D = ((DownloadHandlerTexture)request.downloadHandler).texture;

                if (_texture2D == null)
                    return false;

                Sprite sprite = Sprite.Create(_texture2D, new Rect(0, 0, _texture2D.width, _texture2D.height), new Vector2(0.5f, 0.5f), 20f);

                image.sprite = sprite;
                return true;
            }
            else
            {
                //Debug.Log("Download Image : Failed");
                return false;
            }
        }
    }
}
