using TRIA.Core;
using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioClip sceneMusic;

    private void Start()
    {
        if (sceneMusic != null)
        {
            MusicManager.Instance.PlayMusic(sceneMusic);
        }
    }
}
