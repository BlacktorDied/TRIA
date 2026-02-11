using System;
using System.Collections.Generic; // Required for HashSet
using UnityEngine;

namespace TRIA.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // --- CORE EVENTS ---
        public static event Action OnFadeOutRequested;
        public static event Action OnFadeInRequested;
        public static event Action<GameState> OnGameStateChanged;

        // --- PERSISTENT PLAYER DATA (From GameManager2) ---
        [Header("Abilities")]
        public bool dashUnlocked = false;

        [Header("Stats")]
        public float playerAttack = 1f;
        public int playerMaxHealth = 3;

        [Header("Fragment Progress")]
        public int attackFragments = 0;
        public int healthFragments = 0;

        [Header("World State")]
        public string lastEntryID;

        // HashSet is used because checking "Contains" is much faster than a List
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
            // Set initial state based on scene
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
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

            // Handle Time and Physics
            switch (CurrentState)
            {
                case GameState.MainMenu:
                case GameState.Gameplay:
                    Time.timeScale = 1f;
                    break;
                case GameState.Pause:
                    Time.timeScale = 0f; // Freeze the world
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

        // --- HELPER METHODS ---
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
    }
}
