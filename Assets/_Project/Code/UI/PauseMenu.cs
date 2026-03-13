using TRIA.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TRIA.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField]
        private GameObject pauseMenuPanel;

        private void OnEnable()
        {
            // Listen for state changes so the menu reacts automatically
            GameManager.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void HandleGameStateChanged(GameState newState)
        {
            // Show the menu if we are paused, hide it otherwise
            pauseMenuPanel.SetActive(newState == GameState.Pause);
        }

        // Inside PauseMenu.cs
        // Remove the CallbackContext version and use this simple one:
        public void OnPause()
        {
            // This will now work with "Send Messages" IF called correctly
            GameManager.Instance.TogglePause();
        }

        public void Resume()
        {
            GameManager.Instance.TogglePause();
        }

        public void GoToMainMenu()
        {
            // Ensure the game unpauses before switching scenes
            GameManager.Instance.ChangeState(GameState.MainMenu);

            // Use our SceneLoader for a smooth fade
            SceneLoader.Instance.TransitionToScene("MainMenu", "");
        }

        public void QuitGame()
        {
            GameManager.Instance.QuitGame();
        }
    }
}
