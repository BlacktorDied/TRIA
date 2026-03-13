using System;
using UnityEngine;

namespace TRIA.Core
{
    public class GameManager : MonoBehaviour
    {
        // Inside GameManager.cs
        public static event Action OnFadeOutRequested;
        public static event Action OnFadeInRequested;

        // Add these helper methods to resolve the "never used" warning:
        public static void TriggerFadeOut() => OnFadeOutRequested?.Invoke();

        public static void TriggerFadeIn() => OnFadeInRequested?.Invoke();

        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }

        // Other scripts will listen to this event to know when the game pauses/resumes
        public static event Action<GameState> OnGameStateChanged;

        private void Awake()
        {
            // Standard Singleton setup to ensure only one GameManager exists
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

            // Handle specific logic based on the new state
            switch (CurrentState)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;
                case GameState.Gameplay:
                    Time.timeScale = 1f;
                    break;
                case GameState.Pause:
                    Time.timeScale = 0f; // Freezes physics and time-based movement
                    break;
            }

            // Broadcast the state change to the rest of the game
            OnGameStateChanged?.Invoke(newState);
        }

        // Controls global game flow (pause, quit)
        public void TogglePause()
        {
            if (CurrentState == GameState.Gameplay)
                ChangeState(GameState.Pause);
            else if (CurrentState == GameState.Pause)
                ChangeState(GameState.Gameplay);
        }

        public void QuitGame()
        {
            Debug.Log("Quitting Game...");
            Application.Quit();
        }
    }
}
