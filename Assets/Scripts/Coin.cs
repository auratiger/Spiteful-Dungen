using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip coinPickUpSFX;
    [SerializeField] private int points = 5;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        AudioSource.PlayClipAtPoint(coinPickUpSFX, Camera.main.transform.position);
        FindObjectOfType<GameSession>().AddToScore(points);
        Destroy(gameObject);
    }
}
