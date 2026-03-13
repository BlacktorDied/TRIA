using System.Collections;
using TRIA.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TRIA.UI
{
    public class ScreenFader : MonoBehaviour
    {
        public static ScreenFader Instance { get; private set; }

        [Header("UI References")]
        [SerializeField]
        private Image fadeImage;

        [Header("Settings")]
        [SerializeField]
        private float fadeDuration = 0.5f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the canvas alive between rooms!

            // Ensure the screen is clear when the game first boots
            SetAlpha(0f);
        }

        private void OnEnable()
        {
            GameManager.OnFadeOutRequested += StartFadeOut;
            GameManager.OnFadeInRequested += StartFadeIn;
        }

        private void OnDisable()
        {
            GameManager.OnFadeOutRequested -= StartFadeOut;
            GameManager.OnFadeInRequested -= StartFadeIn;
        }

        private void StartFadeOut() => StartCoroutine(FadeOut());

        private void StartFadeIn() => StartCoroutine(FadeIn());

        // Coroutine to fade to solid black
        public IEnumerator FadeOut()
        {
            // Block raycasts so the player can't accidentally click UI buttons while fading
            fadeImage.raycastTarget = true;
            yield return Fade(1f);
        }

        // Coroutine to fade to transparent
        public IEnumerator FadeIn()
        {
            yield return Fade(0f);
            fadeImage.raycastTarget = false;
        }

        private IEnumerator Fade(float targetAlpha)
        {
            float startAlpha = fadeImage.color.a;
            float time = 0f;

            while (time < fadeDuration)
            {
                // UnscaledDeltaTime ensures the fade runs even when Time.timeScale == 0!
                time += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
                SetAlpha(alpha);

                yield return null; // Wait for the next frame
            }

            SetAlpha(targetAlpha);
        }

        private void SetAlpha(float alpha)
        {
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }
}
