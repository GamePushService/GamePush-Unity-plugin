using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace GamePush.Overlays.Widgets
{
    /// <summary>
    /// Loads avatars and icons off the network into an Image, with a process-wide texture cache
    /// so scrolling a list does not re-download the same picture.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public sealed class GP_RemoteImage : MonoBehaviour
    {
        const int CacheLimit = 128;
        const int MinDecodedSize = 16;
        static readonly Color EmptyFill = new Color(1f, 1f, 1f, 0.12f);
        const string BrowserUserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";

        static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();
        static readonly List<string> CacheOrder = new List<string>();

        public Sprite placeholder;

        Image _image;
        string _url;
        string _fallbackUrl;
        string _upgradeUrl;
        Coroutine _routine;
        Coroutine _upgradeRoutine;

        Image Target => _image != null ? _image : _image = GetComponent<Image>();

        void Awake()
        {
            LockSquare();
        }

        void OnEnable()
        {
            LockSquare();
        }

        void LockSquare()
        {
            var layout = GetComponent<LayoutElement>();
            if (layout != null)
            {
                var size = Mathf.Max(layout.minWidth, layout.minHeight, layout.preferredWidth, layout.preferredHeight);
                if (size > 0f)
                {
                    layout.minWidth = size;
                    layout.minHeight = size;
                    layout.preferredWidth = size;
                    layout.preferredHeight = size;
                    layout.flexibleWidth = 0f;
                    layout.flexibleHeight = 0f;
                    GP_LayoutSquare.Lock(this, size);
                }
            }

            if (Target != null && Target.sprite != null)
                Target.preserveAspect = true;
        }

        /// <summary>
        /// Shows the generator avatar immediately, then replaces it if the explicit URL downloads.
        /// </summary>
        public void LoadPlayer(string avatar, int playerId, Sprite fallback = null)
        {
            if (fallback != null)
                placeholder = fallback;

            StopUpgrade();
            var generated = playerId > 0 ? Normalize(GP_AvatarGenerator.Generate(playerId)) : "";
            var explicitUrl = Normalize(avatar);

            if (!string.IsNullOrEmpty(generated))
                Begin(generated, showPlaceholder: false);
            else
                Begin(explicitUrl, showPlaceholder: true);

            if (!string.IsNullOrEmpty(explicitUrl) && explicitUrl != generated)
                BeginUpgrade(explicitUrl);
        }

        public void Load(string url, Sprite fallback = null, string fallbackUrl = null)
        {
            if (fallback != null)
                placeholder = fallback;

            StopUpgrade();
            url = Normalize(url);
            _fallbackUrl = Normalize(fallbackUrl);
            if (_fallbackUrl == url)
                _fallbackUrl = "";
            Begin(url, showPlaceholder: true);
        }

        void Begin(string url, bool showPlaceholder)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            _url = url;
            if (string.IsNullOrEmpty(url))
            {
                Apply(placeholder);
                return;
            }
            if (Cache.TryGetValue(url, out var cached) && cached != null)
            {
                Apply(cached);
                return;
            }

            if (showPlaceholder)
                Apply(placeholder);
            else
                HideCircle();

            if (isActiveAndEnabled)
                _routine = StartCoroutine(Download(url, upgrade: false));
        }

        void BeginUpgrade(string url)
        {
            _upgradeUrl = url;
            if (Cache.TryGetValue(url, out var cached) && cached != null)
            {
                _url = url;
                Apply(cached);
                return;
            }
            if (isActiveAndEnabled)
                _upgradeRoutine = StartCoroutine(Download(url, upgrade: true));
        }

        void StopUpgrade()
        {
            if (_upgradeRoutine != null)
            {
                StopCoroutine(_upgradeRoutine);
                _upgradeRoutine = null;
            }
            _upgradeUrl = "";
        }

        void OnDisable()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }
            StopUpgrade();
        }

        IEnumerator Download(string url, bool upgrade)
        {
            using var request = new UnityWebRequest(EncodeQuery(url), UnityWebRequest.kHttpVerbGET);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = 15;
            request.redirectLimit = 16;
            request.SetRequestHeader("Accept", "image/png,image/jpeg,image/jpg,*/*;q=0.1");
            try
            {
                request.SetRequestHeader("User-Agent", BrowserUserAgent);
            }
            catch (InvalidOperationException)
            {
            }

            yield return request.SendWebRequest();
            if (upgrade)
                _upgradeRoutine = null;
            else
                _routine = null;

            if (upgrade)
            {
                if (_upgradeUrl != url)
                    yield break;
            }
            else if (_url != url)
            {
                yield break;
            }

            Texture2D texture = null;
            try
            {
                if (request.result == UnityWebRequest.Result.Success)
                    texture = Decode(request.downloadHandler != null ? request.downloadHandler.data : null);
            }
            catch (Exception)
            {
                texture = null;
            }

            if (texture == null)
            {
                if (!upgrade && !TryFallback())
                    Apply(placeholder);
                yield break;
            }

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
            Store(url, sprite);
            _url = url;
            Apply(sprite);
        }

        void HideCircle()
        {
            if (Target == null)
                return;
            Target.enabled = true;
            Target.sprite = null;
            Target.color = EmptyFill;
            Target.preserveAspect = false;
            Target.type = Image.Type.Simple;
        }

        void Apply(Sprite sprite)
        {
            if (Target == null)
                return;
            var empty = sprite == null && placeholder == null;
            Target.enabled = true;
            Target.sprite = sprite != null ? sprite : placeholder;
            Target.color = Target.sprite != null ? Color.white : EmptyFill;
            Target.preserveAspect = Target.sprite != null;
            if (empty)
                Target.type = Image.Type.Simple;
        }

        bool TryFallback()
        {
            var next = _fallbackUrl;
            _fallbackUrl = "";
            if (string.IsNullOrEmpty(next) || next == _url)
                return false;
            Begin(next, showPlaceholder: false);
            return true;
        }

        static string Normalize(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";
            url = url.Trim();
            if (url.StartsWith("//"))
                url = "https:" + url;
            return GP_Images.FormatToPng(url);
        }

        static string EncodeQuery(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;
            var query = url.IndexOf('?');
            if (query < 0)
                return url;
            return url.Substring(0, query + 1) + url.Substring(query + 1).Replace(",", "%2C").Replace(" ", "%20");
        }

        static Texture2D Decode(byte[] data)
        {
            if (data == null || data.Length < 24 || IsWebP(data))
                return null;
            var texture = new Texture2D(2, 2);
            if (!texture.LoadImage(data) || texture.width < MinDecodedSize || texture.height < MinDecodedSize)
            {
                UnityEngine.Object.Destroy(texture);
                return null;
            }
            return texture;
        }

        static bool IsWebP(byte[] data)
        {
            return data.Length >= 12
                   && data[0] == (byte)'R' && data[1] == (byte)'I' && data[2] == (byte)'F' && data[3] == (byte)'F'
                   && data[8] == (byte)'W' && data[9] == (byte)'E' && data[10] == (byte)'B' && data[11] == (byte)'P';
        }

        static void Store(string url, Sprite sprite)
        {
            if (Cache.ContainsKey(url))
                return;
            Cache[url] = sprite;
            CacheOrder.Add(url);
            while (CacheOrder.Count > CacheLimit)
            {
                var oldest = CacheOrder[0];
                CacheOrder.RemoveAt(0);
                Cache.Remove(oldest);
            }
        }
    }
}
