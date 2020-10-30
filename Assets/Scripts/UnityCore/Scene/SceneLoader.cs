using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public void LoadStartLevel()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadNextScene()
    {
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene("Level " + level);
    }

    public void LoadStartMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }

    public void LoadSettingsMenu()
    {
        SceneManager.LoadScene("Settings Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
