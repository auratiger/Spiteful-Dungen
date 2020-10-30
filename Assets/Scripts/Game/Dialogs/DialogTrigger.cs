using System;
using UnityEngine;

namespace Game.Dialogs
{
    public class DialogTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(Application.dataPath);
            Destroy(gameObject);
        }
    }
    
}

