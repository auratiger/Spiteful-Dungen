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

            ConfigureInput();
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
            ClimbLadder();
        }

        private void ConfigureInput()
        {
            player.inputManager.Player.Run.performed += ctx => player.IsRunning = ctx.ReadValueAsButton();
            player.inputManager.Player.Jump.performed += ctx => Jump(ctx.ReadValueAsButton());
            player.inputManager.Player.Roll.performed += _ => StartRoll();
            
        }

        private void Move()
        {
            float controlThrow = player.inputManager.Player.MovementHorizontal.ReadValue<float>(); 
                
            Vector2 playerVelocity = new Vector2(controlThrow * movementSpeed, player.myRigidBody.velocity.y);
            player.myRigidBody.velocity = playerVelocity;

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
        
        private void Jump(bool isPressed)
        {
            if (isPressed && player.IsGrounded)
            {
                Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
                player.myRigidBody.velocity += jumpVelocityToAdd;
            }

            if (!isPressed && player.myRigidBody.velocity.y > 0)
            {
                player.myRigidBody.velocity = new Vector2(player.myRigidBody.velocity.x, player.myRigidBody.velocity.y * 0.5f);
            }
        }

        private void StartRoll()
        {
            if(!player.IsGrounded) return;

            player.IsRolling = true;
            
            player.LastInvulnerable = Time.time;
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

            float controlThrow = player.inputManager.Player.MovementVertical.ReadValue<float>(); 
            Vector2 climbVelocity = new Vector2(player.myRigidBody.velocity.x, controlThrow * climbSpeed);
            player.myRigidBody.velocity = climbVelocity;
            player.myRigidBody.gravityScale = 0;
        }

#endregion
        
    }
}
