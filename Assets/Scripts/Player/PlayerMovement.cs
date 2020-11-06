using DefaultNamespace;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] internal float movementSpeed = 5f;
        [SerializeField] internal float jumpSpeed = 5f;
        [SerializeField] internal float climbSpeed = 5f;
        
        [Header("Particles")] 
        [SerializeField] private ParticleSystem footsteps;

        [Header("Player script")]
        [SerializeField] private Player player;
        
        private ParticleSystem.EmissionModule m_FootEmission;
        private float m_EmissionBaseValue;
        private float m_GravitySlaceAtStart;
      

#region Unity Functions

        private void Start()
        {
            m_FootEmission = footsteps.emission;
            m_EmissionBaseValue = m_FootEmission.rateOverTime.constant;
            m_GravitySlaceAtStart = player.myRigidBody.gravityScale;
        }

        private void Update()
        {
            HandleMovement();
        }

#endregion

#region Public Functions

        public void StopRoll()
        {
            player.IsRolling = false;
            player.IsInvulnerable = false;
                
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer(Layers.Enemy), 
                LayerMask.NameToLayer(Layers.Player),
                false);
        }
        
        public void SetMovementSpeed(int speed)
        {
            movementSpeed = speed;
        }

#endregion

#region Private Functions

        private void HandleMovement()
        {
            Move();
            Jump();
            Roll();
            ClimbLadder();
        }
        
        private void Move()
        {
            float controlThrow = CrossPlatformInputManager.GetAxis(Controls.HORIZONTAL);
                
            Vector2 playerVelocity = new Vector2(controlThrow * movementSpeed, player.myRigidBody.velocity.y);
            player.myRigidBody.velocity = playerVelocity;
        
        
            if (!player.IsRunning)
            {
                player.IsRunning = CrossPlatformInputManager.GetButtonDown(Controls.FIRE3);
            }
            else
            {
                player.IsRunning = !CrossPlatformInputManager.GetButtonUp(Controls.FIRE3);
            }

            FootstepsParticles(controlThrow);

            // flips the player
            var moveX = player.myRigidBody.velocity.x;

            if (moveX > 0 && !player.IsFacingRigth)
            {
                player.FlipSprite();
            }
            else if (moveX < 0 && player.IsFacingRigth)
            {
                player.FlipSprite();
            }
        }
        
        private void FootstepsParticles(float controlThrow)
        {
            // foot particles
            if (controlThrow != 0 && player.IsGrounded)
            {
                m_FootEmission.rateOverTime = player.IsRunning ? m_EmissionBaseValue * 1.5f : m_EmissionBaseValue;
            }
            else
            {
                m_FootEmission.rateOverTime = 0;
            }
        }
        
        private void Jump()
        {
            // if(!m_IsGrounded) return;

            if (CrossPlatformInputManager.GetButtonDown(Controls.JUMP) && player.IsGrounded)
            {
                Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
                player.myRigidBody.velocity += jumpVelocityToAdd;
            }

            if (CrossPlatformInputManager.GetButtonUp(Controls.JUMP) && player.myRigidBody.velocity.y > 0)
            {
                player.myRigidBody.velocity = new Vector2(player.myRigidBody.velocity.x, player.myRigidBody.velocity.y * 0.5f);
            }
        }
        
        private void Roll()
        {
            if(!player.IsGrounded) return;
        
            if (CrossPlatformInputManager.GetButtonDown(Controls.FIRE2))
            {
                StartRoll();
                player.LastInvulnerable = Time.time;
            }
        }
        
        private void StartRoll()
        {
            player.IsRolling = true;
            player.IsInvulnerable = true;
        
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer(Layers.Enemy), 
                LayerMask.NameToLayer(Layers.Player),
                true);
        }
        
        private void ClimbLadder()
        {
            if (!player.IsClimbing)
            {
                player.myRigidBody.gravityScale = m_GravitySlaceAtStart;
                return;
            }

            float controlThrow = CrossPlatformInputManager.GetAxis(Controls.VERTICAL);
            Vector2 climbVelocity = new Vector2(player.myRigidBody.velocity.x, controlThrow * climbSpeed);
            player.myRigidBody.velocity = climbVelocity;
            player.myRigidBody.gravityScale = 0;
        }

#endregion
        
    }
}
