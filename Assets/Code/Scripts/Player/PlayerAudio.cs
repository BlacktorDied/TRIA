using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource footstepsSource;

    [Header("Clips")]
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip jumpDashClip;
    [SerializeField] private AudioClip attackClip;

    [Header("Footsteps Pitch")]
    [SerializeField] private bool randomizeFootstepPitch = true;
    [SerializeField] private float footstepPitchMin = 0.95f;
    [SerializeField] private float footstepPitchMax = 1.05f;

    [Header("SFX Pitch")]
    [SerializeField] private bool randomizeSfxPitch = true;
    [SerializeField] private float sfxPitchMin = 0.97f;
    [SerializeField] private float sfxPitchMax = 1.03f;

    private void Awake()
    {
        if (sfxSource == null) Debug.LogError("PlayerAudio: SFX AudioSource is missing.");
        if (footstepsSource == null) Debug.LogError("PlayerAudio: Footsteps AudioSource is missing.");

        if (footstepsSource != null)
        {
            footstepsSource.loop = true;
            footstepsSource.clip = walkClip;
            footstepsSource.playOnAwake = false;
        }
    }

    public void PlayJump() => PlayOneShot(jumpDashClip);
    public void PlayAttack() => PlayOneShot(attackClip);
    public void PlayDash() => PlayOneShot(jumpDashClip);

    private void PlayOneShot(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.pitch = randomizeSfxPitch
            ? Random.Range(sfxPitchMin, sfxPitchMax)
            : 1f;

        sfxSource.PlayOneShot(clip);
    }

    public void StartFootsteps()
    {
        if (footstepsSource == null) return;
        if (footstepsSource.isPlaying) return;
        if (walkClip == null) return;

        footstepsSource.pitch = randomizeFootstepPitch
            ? Random.Range(footstepPitchMin, footstepPitchMax)
            : 1f;

        footstepsSource.Play();
    }

    public void StopFootsteps()
    {
        if (footstepsSource == null) return;
        if (!footstepsSource.isPlaying) return;

        footstepsSource.Stop();
    }
}
