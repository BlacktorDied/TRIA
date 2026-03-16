using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TRIA.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // --- CORE EVENTS ---
        public static event Action OnFadeOutRequested;
        public static event Action OnFadeInRequested;
        public static event Action<GameState> OnGameStateChanged;

        // --- PERSISTENT PLAYER DATA ---
        [Header("Abilities")]
        public bool dashUnlocked = false;

        [Header("Stats")]
        public float playerAttack = 1f;
        public int playerMaxHealth = 3;

        [Header("Fragment Progress")]
        public int attackFragments = 0;
        public int healthFragments = 0;

        [Header("Checkpoint")]
        public Vector2 lastCheckpointPosition;
        public string lastCheckpointScene;

        // HashSet for collectibles
        public HashSet<string> collectedCollectibles = new HashSet<string>();

        // --- STATE MANAGEMENT ---
        public GameState CurrentState { get; private set; }

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

        private void Start()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "MainMenu")
                ChangeState(GameState.MainMenu);
            else
                ChangeState(GameState.Gameplay);
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState)
                return;

            CurrentState = newState;

            switch (CurrentState)
            {
                case GameState.MainMenu:
                case GameState.Gameplay:
                    Time.timeScale = 1f;
                    break;
                case GameState.Pause:
                    Time.timeScale = 0f;
                    break;
            }

            OnGameStateChanged?.Invoke(newState);
        }

        public void TogglePause()
        {
            if (CurrentState == GameState.Gameplay)
                ChangeState(GameState.Pause);
            else if (CurrentState == GameState.Pause)
                ChangeState(GameState.Gameplay);
        }

        public static void TriggerFadeOut() => OnFadeOutRequested?.Invoke();
        public static void TriggerFadeIn() => OnFadeInRequested?.Invoke();

        public void ResetProgress()
        {
            dashUnlocked = false;
            playerAttack = 1f;
            playerMaxHealth = 3;
            attackFragments = 0;
            healthFragments = 0;
            collectedCollectibles.Clear();
            Debug.Log("Game progress reset!");
        }

        public void QuitGame()
        {
            Debug.Log("Quitting Application...");
            Application.Quit();
        }

        // --- CHECKPOINT SYSTEM ---
        public void SetCheckpoint(Vector2 position)
        {
            lastCheckpointPosition = position;
            lastCheckpointScene = SceneManager.GetActiveScene().name;
            Debug.Log($"Checkpoint set at {position} in scene {lastCheckpointScene}");
        }

        public void RespawnPlayer(GameObject player)
        {
            StartCoroutine(RespawnPlayerRoutine(player));
        }

        private IEnumerator RespawnPlayerRoutine(GameObject player)
        {
            string currentScene = SceneManager.GetActiveScene().name;

            // If checkpoint is in another scene, load it
            if (!string.IsNullOrEmpty(lastCheckpointScene) && lastCheckpointScene != currentScene)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(lastCheckpointScene);
                while (!asyncLoad.isDone)
                    yield return null;
            }

            // Wait one frame for player to be fully loaded
            yield return null;

            // Move player to last checkpoint
            player.transform.position = lastCheckpointPosition;
        }
    }
}
