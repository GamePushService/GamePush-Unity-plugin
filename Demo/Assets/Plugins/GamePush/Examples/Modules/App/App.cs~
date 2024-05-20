using UnityEngine;
using UnityEngine.UI;
using GamePush;

namespace Examples.App
{
    public class App : MonoBehaviour
    {
        [SerializeField] private Image _appImage;

        public void Title() => GP_App.Title();
        public void Description() => GP_App.Description();

        public void Image() => GP_App.GetImage(_appImage);
        public void ImageUrl() => GP_App.ImageUrl();

        public void Url() => GP_App.Url();
    }
}