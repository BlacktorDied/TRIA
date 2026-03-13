using System.Collections;
using UnityEngine;

namespace TRIA.Core
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField]
        private AudioSource sourceA;

        [SerializeField]
        private AudioSource sourceB;

        [Header("Settings")]
        [SerializeField]
        private float transitionDuration = 1.5f;

        private AudioSource activeSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Create the sources via code so the Inspector stays clean
            sourceA = gameObject.AddComponent<AudioSource>();
            sourceB = gameObject.AddComponent<AudioSource>();

            // Setup defaults
            ConfigureSource(sourceA);
            ConfigureSource(sourceB);

            activeSource = sourceA;
        }

        private void ConfigureSource(AudioSource source)
        {
            source.playOnAwake = false;
            source.loop = true;
            source.ignoreListenerPause = true; // Music keeps playing while game is paused!
        }

        public void PlayMusic(AudioClip newClip)
        {
            if (activeSource.clip == newClip)
                return;

            AudioSource inactiveSource = (activeSource == sourceA) ? sourceB : sourceA;
            StartCoroutine(FadeTrack(newClip, inactiveSource));
        }

        private IEnumerator FadeTrack(AudioClip newClip, AudioSource nextSource)
        {
            nextSource.clip = newClip;
            nextSource.volume = 0;
            nextSource.Play();

            float timer = 0;
            float startVolume = activeSource.volume;

            while (timer < transitionDuration)
            {
                timer += Time.unscaledDeltaTime;
                float progress = timer / transitionDuration;

                nextSource.volume = Mathf.Lerp(0, 1, progress);
                activeSource.volume = Mathf.Lerp(startVolume, 0, progress);
                yield return null;
            }

            activeSource.Stop();
            activeSource = nextSource;
        }
    }
}
