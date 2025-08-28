using UnityEngine;
using UnityEngine.UI;
using GamePush;

using Examples.Console;

namespace Examples.Sounds
{
    public class Sounds : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _isMuted;
        [SerializeField] private Button _isSFXMuted;
        [SerializeField] private Button _isMusicMuted;
        [SerializeField] private Button _muteAll;
        [SerializeField] private Button _muteSFX;
        [SerializeField] private Button _muteMusic;
        [SerializeField] private Button _unmuteAll;
        [SerializeField] private Button _unmuteSFX;
        [SerializeField] private Button _unmuteMusic;
        
        [Header("Audio sources")]
        [SerializeField] private AudioSource MusicSource;
        [SerializeField] private AudioSource SFXSource;
        
        private void OnEnable()
        {
            GP_Sounds.OnMute += () => OnMute(SoundType.All);
            GP_Sounds.OnMuteSFX += () => OnMute(SoundType.SFX);
            GP_Sounds.OnMuteMusic += () => OnMute(SoundType.Music);
            
            GP_Sounds.OnUnmute += () => OnUnmute(SoundType.All);
            GP_Sounds.OnUnmuteMusic += () => OnUnmute(SoundType.Music);
            GP_Sounds.OnUnmuteSFX += () => OnUnmute(SoundType.SFX);

            _isMuted.onClick.AddListener(() => IsMuted(SoundType.All));
            _isSFXMuted.onClick.AddListener(() => IsMuted(SoundType.SFX));
            _isMusicMuted.onClick.AddListener(() => IsMuted(SoundType.Music));
            _muteAll.onClick.AddListener(() => Mute(SoundType.All));
            _muteSFX.onClick.AddListener(() => Mute(SoundType.SFX));
            _muteMusic.onClick.AddListener(() => Mute(SoundType.Music));
            _unmuteAll.onClick.AddListener(() => Unmute(SoundType.All));
            _unmuteSFX.onClick.AddListener(() => Unmute(SoundType.SFX));
            _unmuteMusic.onClick.AddListener(() => Unmute(SoundType.Music));
        }

        private void OnDisable()
        {
            _isMuted.onClick.RemoveAllListeners();
            _isSFXMuted.onClick.RemoveAllListeners();
            _isMusicMuted.onClick.RemoveAllListeners();
            _muteAll.onClick.RemoveAllListeners();
            _muteSFX.onClick.RemoveAllListeners();
            _muteMusic.onClick.RemoveAllListeners();
            _unmuteAll.onClick.RemoveAllListeners();
            _unmuteSFX.onClick.RemoveAllListeners();
            _unmuteMusic.onClick.RemoveAllListeners();
        }

        public void OnMute(SoundType type)
        {
            ConsoleUI.Instance.Log($"{type} should be muted");
            switch (type)
            {
                case SoundType.All:
                    AudioListener.pause = true;
                    return;
                case SoundType.SFX:
                    SFXSource.mute = true;
                    return;
                case SoundType.Music:
                    MusicSource.mute = true;
                    return;
            }
        }
        
        public void OnUnmute(SoundType type)
        {
            ConsoleUI.Instance.Log($"{type} should be unmuted");
            switch (type)
            {
                case SoundType.All:
                    AudioListener.pause = false;
                    return;
                case SoundType.SFX:
                    SFXSource.mute = false;
                    return;
                case SoundType.Music:
                    MusicSource.mute = false;
                    return;
            }
        }

        public void IsMuted(SoundType type)
        {
            bool isMuted = GP_Sounds.IsMuted(type);
            ConsoleUI.Instance.Log($"{type} muted: {isMuted}");
        }
        
        public void Mute(SoundType type)
        {
            ConsoleUI.Instance.Log($"{type} is muted");
            GP_Sounds.Mute(type);
        }
        
        public void Unmute(SoundType type)
        {
            ConsoleUI.Instance.Log($"{type} is unmuted");
            GP_Sounds.Unmute(type);
        }
    }
}