using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Data;
using GamePush.Tools;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GamePush.UI
{
    public class LeaderboardCell : MonoBehaviour
    {
        [SerializeField]
        private Image avatarImage;
        [SerializeField]
        TMP_Text playerName, playerFields;
        [SerializeField]
        GameObject firstPlace, secondPlace, thirdPlace, defaultPlace;
        [SerializeField]
        TMP_Text placeNum;

        public async Task Init(PlayerRatingState playerState)
        {
            playerName.text = playerState.name;
            playerFields.text = playerState.fields;

            SetPlace(playerState.position);

            bool hasAvatar = await DownloadImageAsync(playerState.avatar, avatarImage);
            if (!hasAvatar)
                avatarImage.enabled = false;
        }

        private void DeactivePlaces()
        {
            firstPlace.SetActive(false);
            secondPlace.SetActive(false);
            thirdPlace.SetActive(false);
            defaultPlace.SetActive(false);
        }

        private void SetPlace(int position)
        {
            DeactivePlaces();

            //Debug.Log("Set position " + position.ToString());

            switch (position)
            {
                case 1:
                    firstPlace.SetActive(true);
                    return;
                case 2:
                    secondPlace.SetActive(true);
                    return;
                case 3:
                    thirdPlace.SetActive(true);
                    return;
                default:
                    defaultPlace.SetActive(true);
                    placeNum.text = position.ToString();
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
