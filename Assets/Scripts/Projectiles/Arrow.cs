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
        private int baseDamage = 5;
        private int damage;

        
#region Unity Functions

        private void Awake()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            damage = FindObjectOfType<Player>().GetDamage() + baseDamage;
        }

        private void Start()
        {
            // m_Rigidbody2D.velocity = transform.right * xVelocity;
            m_Rigidbody2D.AddForce(transform.right * xVelocity);
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            HandleImpact(other);
            ImpactSound();
        }


#endregion

#region Protected Functions

        protected override void ImpactSound()
        {
            AudioController.instance.PlayAudio(AudioType.SFX_ArrowHit);
        }

        protected override void HandleImpact(Collision2D other)
        {
            if (m_Rigidbody2D.IsTouchingLayers(LayerMask.GetMask(Layers.Enemy)))
            {
                Enemy enemy = other.collider.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
            
            Destroy(gameObject);
            
        }

#endregion

    }
}