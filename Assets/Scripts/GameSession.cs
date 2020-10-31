using UnityCore.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioType = UnityCore.Audio.AudioType;

public class GameSession : MonoBehaviour
{
    [SerializeField] private int score = 0;

    [SerializeField] private Slider healthBar;
    [SerializeField] private Text scoreText;

    private Player player;
    
    // Start is called before the first frame update
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

        player = FindObjectOfType<Player>();
        
        AudioController.instance.PlayAudio(AudioType.Overworld_2, true, 1f);
    }

    private void Start()
    {
        healthBar.value = player.GetHealth();
        scoreText.text = score.ToString();
    }

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
        Debug.Log("You died");
        ResetGameSession();
    }

    private void ResetGameSession()
    {
        AudioController.instance.StopAudio(AudioType.Overworld_2);
        FindObjectOfType<SceneLoader>().LoadStartMenu();
        Destroy(gameObject);
    }
}
