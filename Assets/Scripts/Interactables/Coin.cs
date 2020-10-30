using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityCore.Audio;
using UnityEngine;
using AudioType = UnityCore.Audio.AudioType;

public class Coin : MonoBehaviour
{
    [SerializeField] private int points = 5;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        AudioController.instance.PlayAudio(AudioType.SFX_CoinPickup);
        FindObjectOfType<GameSession>().AddToScore(points);
        
        Destroy(gameObject);
    }
}
