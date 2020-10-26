using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private int points = 5;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        FindObjectOfType<SFXPlayer>().PlayAudioClip(audioClip);
        FindObjectOfType<GameSession>().AddToScore(points);
        
        Destroy(gameObject);
    }
}
