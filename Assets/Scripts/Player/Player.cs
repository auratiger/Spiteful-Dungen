using DefaultNamespace;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Player stats")]
    [SerializeField] protected int health = 100;
    [SerializeField] protected int baseDamage = 5;
    
    [Header("Movement")]
    [SerializeField] protected float movementSpeed = 5f;
    [SerializeField] protected float jumpSpeed = 5f;
    [SerializeField] protected float climbSpeed = 5f;
    
    [Space]
    [Tooltip("the force added to the player body upon death")]
    [SerializeField] private Vector2 deathKick  = new Vector2(250f, 250f);

    [Header("Attack")]
    [SerializeField] private GameObject shooter;
    [SerializeField] private GameObject projectile;

    [Header("Particles")] 
    [SerializeField] private ParticleSystem footsteps;
    
    private Rigidbody2D myRigidBody;
    private Animator myAnimator;
    private CapsuleCollider2D bodyCollider;
    private BoxCollider2D legsCollider;
    private float gravitySlaceAtStart;

#region States

    // player states
    private bool m_IsRunning = false;
    private bool m_IsRolling = false;
    private bool m_IsInvulnerable = false;
    private bool m_IsAlive = true;

    private bool m_IsClimbing = false;
    private bool m_IsGrounded = false;

    // animation state
    private int m_CurrentState;

    private bool m_IsFacingRigth = true;

#endregion

#region Particles

    private ParticleSystem.EmissionModule footEmission;
    private float emissionBaseValue;
    
#endregion

#region Unity Functions 

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        legsCollider = GetComponent<BoxCollider2D>();

        footEmission = footsteps.emission;
        emissionBaseValue = footEmission.rateOverTime.constant;
        
        gravitySlaceAtStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_IsAlive) return;

        m_IsGrounded = legsCollider.IsTouchingLayers(LayerMask.GetMask(Layers.Ground));
        m_IsClimbing = legsCollider.IsTouchingLayers(LayerMask.GetMask(Layers.Climbing));

        Shoot();
        Move();
        Jump();
        Roll();
        ClimbLadder();
        
        HandleAnimation();
        
        // TriggerDeath();
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

    public void StopRoll()
    {
        m_IsRolling = false;
        m_IsInvulnerable = false;
    }
    
    public void FlipSprite()
    {
        // bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        // if (playerHasHorizontalSpeed)
        // {
        //     transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        // }
        
        m_IsFacingRigth = !m_IsFacingRigth;
        
        transform.Rotate(0, 180f, 0);

        }
    
    public void SetMovementSpeed(int speed)
    {
        movementSpeed = speed;
    }
    
    public void TakeDame(int damage)
    {
        if (!m_IsInvulnerable)
        {
            health -= damage;
        }

        if (health <= 0)
        {
            TriggerDeath();
        }
    }
    
    public void TriggerDeath()
    {
        m_IsAlive = false;
        ChangeAnimationState(PlayerAnimations.PLAYER_DIE);
        myRigidBody.velocity = deathKick;
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }

#endregion

#region Private Functions

    private void Shoot()
    {
        if (CrossPlatformInputManager.GetButtonDown(Controls.FIRE1))
        {
            if (!m_IsClimbing && !m_IsRolling)
            {
                GameObject arrow = Instantiate(projectile, shooter.transform.position, shooter.transform.rotation);
            }
        }
    }

    private void Move()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis(Controls.HORIZONTAL);
                
        Vector2 playerVelocity = new Vector2(controlThrow * movementSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
        
        
        if (!m_IsRunning)
        {
            m_IsRunning = CrossPlatformInputManager.GetButtonDown(Controls.FIRE3);
        }
        else
        {
            m_IsRunning = !CrossPlatformInputManager.GetButtonUp(Controls.FIRE3);
        }

        FootstepsParticles(controlThrow);

        // flips the player
        var moveX = myRigidBody.velocity.x;

        if (moveX > 0 && !m_IsFacingRigth)
        {
            FlipSprite();
        }
        else if (moveX < 0 && m_IsFacingRigth)
        {
            FlipSprite();
        }
    }

    private void FootstepsParticles(float controlThrow)
    {
        // foot particles
        if (controlThrow != 0 && m_IsGrounded)
        {
            footEmission.rateOverTime = m_IsRunning ? emissionBaseValue * 1.5f : emissionBaseValue;
        }
        else
        {
            footEmission.rateOverTime = 0;
        }
    }

    private void HandleAnimation()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

        if (m_IsClimbing)
        {
            // TODO make it so the player can stand still on the top of the ladder
            
            bool playerVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
            if(playerVerticalSpeed) ChangeAnimationState(PlayerAnimations.PLAYER_CLIMB);
            
        }
        else
        {
            if (m_IsRolling)
            {
                ChangeAnimationState(PlayerAnimations.PLAYER_ROLL);
            }
            else if (playerHasHorizontalSpeed && m_IsRunning)
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

    private void Jump()
    {
        // if(!m_IsGrounded) return;
        
        if (CrossPlatformInputManager.GetButtonDown(Controls.JUMP) && m_IsGrounded)
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocityToAdd;
        }

        if (CrossPlatformInputManager.GetButtonUp(Controls.JUMP) && myRigidBody.velocity.y > 0)
        {
            myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, myRigidBody.velocity.y * 0.5f);
        }
    }

    private void Roll()
    {
        if(!m_IsGrounded) return;
        
        if (CrossPlatformInputManager.GetButtonDown(Controls.FIRE2))
        {
            m_IsRolling = true;
            m_IsInvulnerable = true;
        }
    }


    private void ClimbLadder()
    {
        if (!m_IsClimbing)
        {
            myRigidBody.gravityScale = gravitySlaceAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis(Controls.VERTICAL);
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0;
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
                    Vector2 jumpVelocityToAdd = new Vector2(myRigidBody.velocity.x, jumpSpeed);
                    myRigidBody.velocity = jumpVelocityToAdd;

                    enemy.TakeDame(baseDamage);
                }
                else
                {
                    TakeDame(enemy.GetDamage());
                }
            }
        }
    }
    
#endregion

}
