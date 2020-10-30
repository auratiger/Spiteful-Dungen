using DefaultNamespace;
using UnityEngine;

namespace Enemies
{
    public class Slime : Enemy
    {
        
        private Rigidbody2D myRigidBody;
        private PolygonCollider2D _bodyCollider;
    
        // Start is called before the first frame update
        void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            _bodyCollider = GetComponent<PolygonCollider2D>();
        }

        // Update is called once per frame
        void Update()
        {
            Move();
        }

        void PlayerCollision()
        {
            if (_bodyCollider.IsTouchingLayers(LayerMask.GetMask(Layers.Player)))
            {
                FindObjectOfType<Player>().TakeDamage(damage);
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

        protected void OnTriggerEnter2D(Collider2D other)
        {
            transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f);
        }
    }
}