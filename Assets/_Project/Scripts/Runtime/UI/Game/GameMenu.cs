using EntangledShift.Runtime.Systems;
using UnityEngine;
using UnityEngine.Audio;

namespace EntangledShift.Runtime.UI.Game
{
    public class GameMenu : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private AudioClip _backgroundMusic;
        [SerializeField] private AudioMixer _mainMixer;

        private const string MasterVolKey = "Master";
        private const string SFXVolKey = "SFX";
        private const string musicVolKey = "Music";

        private void Start()
        {
            float savedMasterVolume = PlayerPrefs.GetFloat(MasterVolKey, 0.5f);
            float masterDb = Mathf.Log10(Mathf.Max(0.0001f, savedMasterVolume)) * 20;

            float savedSFXVolume = PlayerPrefs.GetFloat(SFXVolKey, 0.8f);
            float sfxDb = Mathf.Log10(Mathf.Max(0.0001f, savedSFXVolume)) * 20;

            float savedMusicVolume = PlayerPrefs.GetFloat(musicVolKey, 0.4f);
            float musicDb = Mathf.Log10(Mathf.Max(0.0001f, savedMusicVolume)) * 20;

            _mainMixer.SetFloat(MasterVolKey, masterDb);
            _mainMixer.SetFloat(SFXVolKey, sfxDb);
            _mainMixer.SetFloat(musicVolKey, musicDb);
        }

        private void OnEnable()
        {
            if (_backgroundMusic != null)
            {
                AudioController.Instance.PlayMusic(_backgroundMusic);
            }
        }
    }
}