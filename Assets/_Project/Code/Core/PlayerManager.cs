using TRIA.Core.Constants;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene events

namespace TRIA.Core
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }
        public GameObject CurrentPlayer { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Subscribe to the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            // Unsubscribe when destroyed to avoid memory leaks
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            SpawnMainCamera();

            // Initial check in case we started the game directly in a level (skipping menu)
            CheckAndSpawnPlayer(SceneManager.GetActiveScene().name);
        }

        // This runs EVERY time a scene changes
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CheckAndSpawnPlayer(scene.name);
        }

        private void CheckAndSpawnPlayer(string sceneName)
        {
            // 1. Don't spawn if we are in the menu
            if (sceneName == "MainMenu")
                return;

            // 2. Don't spawn if the player already exists
            if (CurrentPlayer != null)
                return;

            SpawnPlayer();
        }

        private void SpawnMainCamera()
        {
            if (GameObject.Find("MainCamera") != null)
                return;

            GameObject cameraPrefab = Resources.Load<GameObject>("MainCamera");
            if (cameraPrefab != null)
            {
                GameObject camInstance = Instantiate(cameraPrefab);
                camInstance.name = "MainCamera";
                DontDestroyOnLoad(camInstance);
            }
        }

        private void SpawnPlayer()
        {
            GameObject playerPrefab = Resources.Load<GameObject>("Player");

            if (playerPrefab != null)
            {
                CurrentPlayer = Instantiate(playerPrefab);
                CurrentPlayer.name = "Player";
                DontDestroyOnLoad(CurrentPlayer);

                // Find the spawn point tagged 'Respawn' for the initial entry
                GameObject defaultSpawn = GameObject.FindGameObjectWithTag(Tags.Respawn);
                if (defaultSpawn != null)
                {
                    CurrentPlayer.transform.position = defaultSpawn.transform.position;
                }

                Debug.Log("PlayerManager: Player spawned successfully.");
            }
            else
            {
                Debug.LogError("PlayerManager: Could not find 'Player' prefab in Resources!");
            }
        }
    }
}
