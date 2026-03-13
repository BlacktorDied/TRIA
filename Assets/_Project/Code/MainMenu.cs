using TRIA.Core;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Use the SceneLoader we built earlier
        SceneLoader.Instance.TransitionToScene("Basement_01", "StartGame");
    }

    public void QuitGame()
    {
        // Using the full path (TRIA.Core.GameManager) solves the CS1061 error
        TRIA.Core.GameManager.Instance.QuitGame();
    }
}
