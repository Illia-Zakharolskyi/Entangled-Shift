using UnityEngine;

namespace EntangledShift.Runtime.Systems
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private float _minSFXPitch = 0.9f;
        [SerializeField] private float _maxSFXPitch = 1.1f;

        [Header("Outputs")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource uiSource;

        public static AudioController Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource.clip == clip && musicSource.isPlaying) return;

            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource.isPlaying) musicSource.Stop();
        }

        public void PlayOneShotSFXSound(AudioClip clip)
        {
            sfxSource.pitch = Random.Range(_minSFXPitch, _maxSFXPitch);
            sfxSource.PlayOneShot(clip);
        }

        public void PlayOneShotUISound(AudioClip clip)
        {
            uiSource.PlayOneShot(clip);
        }
    }
}