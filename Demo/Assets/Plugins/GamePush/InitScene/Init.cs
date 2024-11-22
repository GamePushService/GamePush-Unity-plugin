using UnityEngine;
using UnityEngine.SceneManagement;
using GamePush;

namespace GamePush.Initialization
{
    public class Init : MonoBehaviour
    {
        private async void Start()
        {
            await GP_Init.Ready;
            SceneManager.LoadScene(1);
        }
    }
}
