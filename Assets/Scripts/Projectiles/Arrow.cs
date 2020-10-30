using System;
using DefaultNamespace;
using UnityCore.Audio;
using UnityEngine;
using AudioType = UnityCore.Audio.AudioType;

namespace Projectiles
{
    public class Arrow : Projectile
    {
        private Rigidbody2D m_Rigidbody2D;
        

#region Unity Functions

        private void Awake()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            m_Rigidbody2D.velocity = transform.right * xVelocity;
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            HandleImpact();
            ImpactSound();
        }


#endregion

#region Protected Functions

        protected override void ImpactSound()
        {
            AudioController.instance.PlayAudio(AudioType.SFX_ArrowHit);
        }

        protected override void HandleImpact()
        {

            Destroy(gameObject);
            
        }

#endregion

    }
}