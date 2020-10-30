using UnityEngine;

namespace Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] protected float xVelocity;
        [SerializeField] protected float yVelocity;

        protected abstract void ImpactSound();
        protected abstract void HandleImpact(Collision2D other);

    }
}