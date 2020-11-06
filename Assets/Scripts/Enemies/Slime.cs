using System;
using DefaultNamespace;
using UnityEngine;

namespace Enemies
{
    public class Slime : Enemy
    {
        
        private Rigidbody2D myRigidBody;
        private PolygonCollider2D _bodyCollider;
        
        [Serializable]
        private struct SaveData
        {
            public float[] position;
            public int health;
        }

#region Unity Functions

        // Start is called before the first frame update
        private void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            _bodyCollider = GetComponent<PolygonCollider2D>();
        }

        // Update is called once per frame
        private void Update()
        {
            Move();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f);
        }

#endregion

#region Public Functions

        public override void TakeDamage(int _damage)
        {
            health -= _damage;

            if (health <= 0)
            {
                TriggerDeath();
            }
        }

        public override void TriggerDeath()
        {
            Debug.Log("Slime is dead");
            Destroy(gameObject);
        }

        public override object CaptureState()
        {
            var position = transform.position;
            return new SaveData()
            {
                position = new[] {position.x, position.y},
                health = health,
            };
        }

        public override void RestoreState(object state)
        {
            var saveData = (SaveData) state;
            
            transform.position = new Vector2(saveData.position[0], saveData.position[1]);
            health = saveData.health;
        }


#endregion

#region Private Functions

        private void PlayerCollision()
        {
            if (_bodyCollider.IsTouchingLayers(LayerMask.GetMask(Layers.Player)))
            {
                FindObjectOfType<Player.Player>().TakeDamage(damage);
            }
        }
        
        protected override void Move()
        {
            if (IsFacingRigth())
            {
                myRigidBody.velocity = new Vector2(moveSpeed, 0f);
            }
            else
            {
                myRigidBody.velocity = new Vector2(-moveSpeed, 0f);
            }
        }


#endregion
    }
}