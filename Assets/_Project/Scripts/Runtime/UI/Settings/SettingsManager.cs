using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace EntangledShift.Runtime.UI.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioMixer mainMixer;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;

        private const string MasterVolKey = "Master";
        private const string SFXVolKey = "SFX";
        private const string musicVolKey = "Music";

        private void OnEnable()
        {
            float savedMasterVolume = PlayerPrefs.GetFloat(MasterVolKey, 0.5f);
            float savedSFXVolume = PlayerPrefs.GetFloat(SFXVolKey, 0.8f);
            float savedMusicVolume = PlayerPrefs.GetFloat(musicVolKey, 0.4f);

            masterVolumeSlider.value = savedMasterVolume;
            sfxVolumeSlider.value = savedSFXVolume;
            musicVolumeSlider.value = savedMusicVolume;

            ApplyVolume(MasterVolKey, savedMasterVolume);
            ApplyVolume(SFXVolKey, savedSFXVolume);
            ApplyVolume(musicVolKey, savedMusicVolume);

            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        private void OnDisable()
        {
            masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);

            PlayerPrefs.Save();
        }

        public void SetMasterVolume(float volume)
        {
            ApplyVolume(MasterVolKey, volume);
            PlayerPrefs.SetFloat(MasterVolKey, volume);
        }

        public void SetSFXVolume(float volume)
        {
            ApplyVolume(SFXVolKey, volume);
            PlayerPrefs.SetFloat(SFXVolKey, volume);
        }

        public void SetMusicVolume(float volume)
        {
            ApplyVolume(musicVolKey, volume);
            PlayerPrefs.SetFloat(musicVolKey, volume);
        }

        private void ApplyVolume(string parameterName, float volume)
        {
            float dbValue = Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20;
            Debug.Log($"Намагаємось встановити {parameterName} на {dbValue} dB (значення слайдера: {volume})");
            mainMixer.SetFloat(parameterName, dbValue);

            bool result = mainMixer.SetFloat(parameterName, dbValue);
            if (!result)
            {
                Debug.LogError($"ПОМИЛКА: Не вдалося знайти параметр {parameterName} у міксері! Перевірте Exposed Parameters.");
            }
        }
    }
}
