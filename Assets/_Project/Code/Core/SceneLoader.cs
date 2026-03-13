using System.Collections;
using TRIA.Core.Constants;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TRIA.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }
        public string TargetSpawnPointId { get; private set; }

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

        public void TransitionToScene(string sceneName, string spawnPointId)
        {
            if (GameManager.Instance.CurrentState == GameState.Pause)
                return;
            TargetSpawnPointId = spawnPointId;
            StartCoroutine(TransitionRoutine(sceneName));
        }

        private IEnumerator TransitionRoutine(string sceneName)
        {
            GameManager.Instance.ChangeState(GameState.Pause);

            // FIX: Call the event via the GameManager class name
            GameManager.TriggerFadeOut();

            // Wait for the fade duration (0.5s is standard for our ScreenFader)
            yield return new WaitForSecondsRealtime(0.5f);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
                yield return null;

            RepositionPlayer();

            // FIX: Call the event via the GameManager class name
            GameManager.TriggerFadeIn();

            GameManager.Instance.ChangeState(GameState.Gameplay);
        }

        private void RepositionPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag(Tags.Player);
            if (player != null)
            {
                SpawnPoint[] spawnPoints = Object.FindObjectsByType<SpawnPoint>(
                    FindObjectsSortMode.None
                );
                foreach (SpawnPoint sp in spawnPoints)
                {
                    if (sp.spawnId == TargetSpawnPointId)
                    {
                        player.transform.position = sp.transform.position;
                        return;
                    }
                }
            }
        }
    }
}
