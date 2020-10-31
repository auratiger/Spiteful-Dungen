using System;
using UnityCore.Audio;
using UnityCore.Data;
using UnityEngine;
using AudioType = UnityCore.Audio.AudioType;

namespace Interactables
{
    public class Coin : MonoBehaviour, ISavable
    {
        [SerializeField] private int points = 5;
    
        [Serializable]
        private struct SaveData
        {
            public float[] position;
        }
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            AudioController.instance.PlayAudio(AudioType.SFX_CoinPickup);
            FindObjectOfType<GameSession>().AddToScore(points);
        
            Destroy(gameObject);
        }

        public object CaptureState()
        {
            var position = transform.position;
            return new SaveData()
            {
                position = new[] {position.x, position.y}
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData) state;
        
            transform.position = new Vector2(saveData.position[0], saveData.position[1]);
        }
    }
}
