using UnityEngine;
using UnityEngine.SceneManagement;
using GamePush;

namespace GamePush.Initialization
{
    public class Init : MonoBehaviour
    {
        private async void Start()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
            asyncLoad.allowSceneActivation = false;
            await GP_Init.Ready;
            //SceneManager.LoadScene(1);
            asyncLoad.allowSceneActivation = true;
        }
    }
}
