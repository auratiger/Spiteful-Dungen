using System;
using DefaultNamespace;
using UnityCore.Audio;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.UI;
using AudioType = UnityCore.Audio.AudioType;

public class GameSession : MonoBehaviour
{
    [SerializeField] private int score = 0;

    [SerializeField] private Slider healthBar;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject gameOverPrefab;
        
    private Player.Player player;

    public static bool isMenuOpen { get; private set; } = false;

    private InputManager inputManager;
    
#region Unity Functions

    void Awake()
    {
        if (FindObjectsOfType<GameSession>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        player = FindObjectOfType<Player.Player>();
        inputManager = new InputManager();

        AudioController.instance.PlayAudio(AudioType.Overworld_2, true, 1f);
    }

    private void Start()
    {
        healthBar.value = player.GetHealth();
        scoreText.text = score.ToString();

        inputManager.Game.Exit.performed += _ => ToggleGameMenu();
    }

    private void OnEnable()
    {
        inputManager?.Enable();
    }

    private void OnDisable()
    {
        inputManager?.Disable();
    }

#endregion

#region Public Functuins

    public void AddToScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }

    public void SetHealth(int health)
    {
        healthBar.value = health;
    }

    public void ProcessPlayerDeath()
    {
        ToggleGameOver();
        ResetGameSession();
    }

    public void ToggleGameMenu()
    {
        if (!isMenuOpen)
        {
            gameMenu.SetActive(true);
            PauseGame();
            isMenuOpen = true;
        }
        else
        {
            gameMenu.SetActive(false);
            ResumeGame();
            isMenuOpen = false;
        }
        
    }

    public void RestartLevel()
    {
        FindObjectOfType<SceneLoader>().LoadStartLevel();
        ResetGameSession();
    }
    
    public void BackToMenu()
    {
        ResumeGame();
        FindObjectOfType<SceneLoader>().LoadStartMenu();
        ResetGameSession();
    }

    
#endregion

#region Private Functions


    private void ToggleGameOver()
    {
        Instantiate(gameOverPrefab, Vector3.zero, Quaternion.identity);
    }

    private void ResetGameSession()
    {
        AudioController.instance.StopAudio(AudioType.Overworld_2);
        Destroy(gameObject);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
    }

#endregion

}
