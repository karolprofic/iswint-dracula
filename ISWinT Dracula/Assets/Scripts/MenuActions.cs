using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    /// <summary>
    /// Starts a new game by loading the first level.
    /// </summary>
    public void StartNewGame()
    {
        SceneManager.LoadScene("Level1");
    }

    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Reloads the currently active level.
    /// </summary>
    public void ReloadCurrentLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    /// <summary>
    /// Exits the game. Stops play mode if running in the editor.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
