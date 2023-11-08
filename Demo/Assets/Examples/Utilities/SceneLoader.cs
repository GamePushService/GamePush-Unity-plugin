using UnityEngine;
using UnityEngine.SceneManagement;

namespace Examples.Utilities
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);
    }
}

