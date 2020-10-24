using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public void StartGame()
    {
        Debug.Log("Start Game");

    }

    public void LoadStartLevel()
    {
        SceneManager.LoadScene("Level 1");
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
