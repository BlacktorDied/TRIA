using UnityEngine;

namespace TRIA.Player
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField]
        private AudioSource sfxSource;

        [SerializeField]
        private AudioSource footstepsSource;

        [Header("Clips")]
        [SerializeField]
        private AudioClip walkClip;

        [SerializeField]
        private AudioClip jumpDashClip;

        [SerializeField]
        private AudioClip attackClip;

        [Header("Pitch Randomization")]
        [SerializeField]
        private float pitchMin = 0.95f;

        [SerializeField]
        private float pitchMax = 1.05f;

        private void Awake()
        {
            if (footstepsSource != null)
            {
                footstepsSource.loop = true;
                footstepsSource.clip = walkClip;
                footstepsSource.playOnAwake = false;
                footstepsSource.spatialBlend = 0f; // 2D Sound
            }
            if (sfxSource != null)
                sfxSource.spatialBlend = 0f;
        }

        public void PlayJump() => PlayOneShot(jumpDashClip);

        public void PlayDash() => PlayOneShot(jumpDashClip);

        public void PlayAttack() => PlayOneShot(attackClip);

        private void PlayOneShot(AudioClip clip)
        {
            if (clip == null || sfxSource == null)
                return;
            sfxSource.pitch = Random.Range(pitchMin, pitchMax);
            sfxSource.PlayOneShot(clip);
        }

        public void StartFootsteps()
        {
            if (footstepsSource == null || walkClip == null)
                return;
            if (!footstepsSource.isPlaying)
            {
                footstepsSource.pitch = Random.Range(pitchMin, pitchMax);
                footstepsSource.Play();
            }
        }

        public void StopFootsteps()
        {
            if (footstepsSource != null && footstepsSource.isPlaying)
                footstepsSource.Stop();
        }
    }
}
