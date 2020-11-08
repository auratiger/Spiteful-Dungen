using System;
using DefaultNamespace;
using UnityCore.Data;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour, IDamageable, ISavable
    {
        [Header("Player stats")]
        [SerializeField] protected int health = 100;
        [SerializeField] protected int baseDamage = 5;
        [SerializeField] private float invulnerabilityTime = 0.5f;

        [Space]
        [Tooltip("the force added to the player body upon death")]
        [SerializeField] private Vector2 deathKick  = new Vector2(250f, 250f);

        [Header("Sub Player scripts")] 
        [SerializeField] internal PlayerAttack playerAttack;
        [SerializeField] internal PlayerMovement playerMovement;
        
        private GameSession m_Session;
    
        internal Rigidbody2D myRigidBody;
        internal CapsuleCollider2D bodyCollider;
        internal BoxCollider2D legsCollider;
        internal Animator myAnimator;
        
        internal InputManager inputManager;

        [Serializable]
        private struct SaveData
        {
            public float[] position;
            public int health;
        }

#region Player State

        internal bool IsFacingRigth = true;
        internal bool IsAttacking = false;
        internal bool IsRunning = false;
        internal bool IsRolling = false;
        internal bool IsAlive = true;
        
        internal bool IsInvulnerable = false;
        internal float LastInvulnerable;

        internal bool IsClimbing = false;
        internal bool IsGrounded = false;

        internal float _decelerationTolerance = 35.0f;
        internal Vector2 velocity;
        
        internal float RotationAngle = 0;
        
        // animation state
        private int m_CurrentState;


#endregion

#region Unity Functions 

        void Awake()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            myAnimator = GetComponent<Animator>();
            bodyCollider = GetComponent<CapsuleCollider2D>();
            legsCollider = GetComponent<BoxCollider2D>();

            m_Session = FindObjectOfType<GameSession>();
            
            inputManager = new InputManager();
        }

        private void OnEnable()
        {
            inputManager?.Enable();
        }

        private void OnDisable()
        {
            inputManager?.Disable();
        }

        void Update()
        {
            if (!IsAlive) return;

            IsGrounded = legsCollider.IsTouchingLayers(LayerMask.GetMask(Layers.Ground, Layers.Platforms, Layers.SolidPlatform));
            IsClimbing = legsCollider.IsTouchingLayers(LayerMask.GetMask(Layers.Climbing));
            
            FallDamage();
        
            HandleAnimation();
        
        }
    
        private void OnCollisionEnter2D(Collision2D other)
        {
        
            if (bodyCollider.IsTouchingLayers(LayerMask.GetMask(Layers.Hazards)))
            {
                TriggerDeath();
            }
            else
            {
                HandleEnemyCollision(other);
            }
        
        }
    
#endregion

#region Public Functions
    
        public void FlipSprite()
        {
            // bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
            // if (playerHasHorizontalSpeed)
            // {
            //     transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
            // }
        
            IsFacingRigth = !IsFacingRigth;
        
            transform.Rotate(0, 180f, 0);

        }

        public float GetHealth()
        {
            return health;
        }

        public int GetDamage()
        {
            return baseDamage;
        }
    
        public void TakeDamage(int damage)
        {
            if (Time.time - LastInvulnerable > invulnerabilityTime)
            {
                IsInvulnerable = false;
            }
        
            if (!IsInvulnerable)
            {
                health -= damage;
                m_Session.SetHealth(health);

                IsInvulnerable = true;
                LastInvulnerable = Time.time;
            }

            if (health <= 0)
            {
                TriggerDeath();
            }
        }
    
        public void TriggerDeath()
        {
            IsAlive = false;
            ChangeAnimationState(PlayerAnimations.PLAYER_DIE);
            myRigidBody.velocity = deathKick;
            m_Session.ProcessPlayerDeath();
        }
    
        public object CaptureState()
        {
            return new SaveData()
            {
                position = new [] {transform.position.x, transform.position.y},
                health = health
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData) state;

            transform.position = new Vector2(saveData.position[0], saveData.position[1]);
            health = saveData.health;
            m_Session.SetHealth(health);
        }

#endregion

#region Private Functions

        private void FallDamage()
        {
            if (!(Vector3.Distance(myRigidBody.velocity, velocity) < _decelerationTolerance))
            {
                TakeDamage((int)Mathf.Abs(velocity.y * 0.6f));
            }
            velocity = myRigidBody.velocity;
        }

        private void HandleAnimation()
        {
            bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

            if (IsAttacking)
            {
                if (95 > RotationAngle && RotationAngle > 85)
                {
                    ChangeAnimationState(PlayerAnimations.PLAYER_BOW_UP);
                }
                else if (50 > RotationAngle && RotationAngle > 40)
                {
                    ChangeAnimationState(PlayerAnimations.PLAYER_BOW_UPPER_RIGTH);
                }
                else if (-50 < RotationAngle && RotationAngle < -40)
                {
                    ChangeAnimationState(PlayerAnimations.PLAYER_BOW_LOWER_RIGHT);
                }
                else if (-95 < RotationAngle && RotationAngle < -85)
                {
                    ChangeAnimationState(PlayerAnimations.PLAYER_BOW_DOWN);
                }
                else
                {
                    ChangeAnimationState(PlayerAnimations.PLAYER_BOW_RIGTH);
                }
            
            }
            else if (IsClimbing)
            {
                // TODO make it so the player can stand still on the top of the ladder
            
                bool playerVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
                if(playerVerticalSpeed) ChangeAnimationState(PlayerAnimations.PLAYER_CLIMB);
            
            }
            else
            {
                if (IsRolling)
                {
                    ChangeAnimationState(PlayerAnimations.PLAYER_ROLL);
                }
                else if (playerHasHorizontalSpeed && IsRunning)
                {
                    ChangeAnimationState(PlayerAnimations.PLAYER_RUNN);
                }
                else if (playerHasHorizontalSpeed)
                {
                    ChangeAnimationState(PlayerAnimations.PLAYER_WALK);
                }
                else
                {
                    ChangeAnimationState(PlayerAnimations.PLAYER_IDLE);
                }
            }
        }
        
        private void ChangeAnimationState(int newState)
        {
            // stop the same animation from interrupting itself
            if (m_CurrentState == newState) return;
        
            // play the animation
            myAnimator.Play(newState);

            // reassign the current state
            m_CurrentState = newState;
        }

        private void HandleEnemyCollision(Collision2D other)
        {
            Enemy enemy = other.collider.GetComponent<Enemy>();

            if (enemy != null)
            {
                foreach (ContactPoint2D point in other.contacts)
                {
                    if (point.normal.y >= 0.9f)
                    {
                        Vector2 jumpVelocityToAdd = new Vector2(myRigidBody.velocity.x, 5f);
                        myRigidBody.velocity = jumpVelocityToAdd;

                        enemy.TakeDamage(baseDamage);
                    }
                    else
                    {
                        TakeDamage(enemy.GetDamage());
                    }
                }
            }
        }
    
#endregion
    }
}
