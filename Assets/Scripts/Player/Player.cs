using System;
using DefaultNamespace;
using UnityCore.Data;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour, IDamageable, ISavable
{
    [Header("Player stats")]
    [SerializeField] protected int health = 100;
    [SerializeField] protected int baseDamage = 5;
    [SerializeField] private float invulnerabilityTime = 0.5f;
    
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
    [SerializeField] private float shootDelay = 0.5f;

    [Header("Particles")] 
    [SerializeField] private ParticleSystem footsteps;

    private GameSession m_Session;
    
    private Rigidbody2D myRigidBody;
    private Animator myAnimator;
    private CapsuleCollider2D bodyCollider;
    private BoxCollider2D legsCollider;
    private float gravitySlaceAtStart;
    private Camera camera;
    
    [Serializable]
    private struct SaveData
    {
        public float[] position;
        public int health;
    }

#region States

    // player states
    private bool m_isAttacking = false;
    private bool m_IsRunning = false;
    private bool m_IsRolling = false;
    private bool m_IsInvulnerable = false;
    private bool m_IsAlive = true;

    private bool m_IsClimbing = false;
    private bool m_IsGrounded = false;

    private float lastShot;
    private float lastInvulnerable;

    private bool m_IsFacingRigth = true;
    private float m_RotationAngle = 0;
    
    // animation state
    private int m_CurrentState;


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

        m_Session = FindObjectOfType<GameSession>();

        camera = Camera.main;

        footEmission = footsteps.emission;
        emissionBaseValue = footEmission.rateOverTime.constant;
        
        gravitySlaceAtStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_IsAlive) return;

        m_IsGrounded = legsCollider.IsTouchingLayers(LayerMask.GetMask(Layers.Ground, Layers.Platforms));
        m_IsClimbing = legsCollider.IsTouchingLayers(LayerMask.GetMask(Layers.Climbing));

        Shoot();
        Move();
        Jump();
        Roll();
        ClimbLadder();
        
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

    public void StartRoll()
    {
        m_IsRolling = true;
        m_IsInvulnerable = true;
        
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer(Layers.Enemy), 
            LayerMask.NameToLayer(Layers.Player),
            true);
    }
    
    public void StopRoll()
    {
        m_IsRolling = false;
        m_IsInvulnerable = false;
        
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer(Layers.Enemy), 
            LayerMask.NameToLayer(Layers.Player),
            false);
    }

    public void StopAttack()
    {
        m_isAttacking = false;
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
        if (Time.time - lastInvulnerable > invulnerabilityTime)
        {
            m_IsInvulnerable = false;
        }
        
        if (!m_IsInvulnerable)
        {
            health -= damage;
            m_Session.SetHealth(health);

            m_IsInvulnerable = true;
            lastInvulnerable = Time.time;
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

    private void Shoot()
    {
        if (CrossPlatformInputManager.GetButtonDown(Controls.FIRE1))
        {
            if (!m_IsClimbing && !m_IsRolling)
            {
                if (Time.time - lastShot < shootDelay) return;
                
                Ray mousePosition = camera.ScreenPointToRay(Input.mousePosition);
                Vector2 playerToMouseVector = mousePosition.origin - transform.position;

                CalculateRotationAngle(playerToMouseVector);

                // Rotate the player to where the mouse is pointing
                if (playerToMouseVector.x < 0 && m_IsFacingRigth)
                {
                    FlipSprite();
                }
                else if (playerToMouseVector.x > 0 && !m_IsFacingRigth)
                {
                    FlipSprite();
                }
                
                shooter.transform.Rotate(0f, 0f, m_RotationAngle);
                m_isAttacking = true;
                lastShot = Time.time;
                
                GameObject arrow = Instantiate(projectile, shooter.transform.position, shooter.transform.rotation);
                
                shooter.transform.Rotate(0f, 0f, -m_RotationAngle);
            }
        }
    }

    private void CalculateRotationAngle(Vector2 playerToMouseVector)
    {
        // Debug.Log(transform.position);
        // Debug.Log(mousePosFromPlayer);
        // Debug.Log(angle);
        
        var angle = Vector2.Angle(new Vector2(playerToMouseVector.x, 0), playerToMouseVector);

        // this restricts the shooting to specific angles,
        // instead of being able to shoot everywhere  
        if (angle > 63.4349f)
        {
            m_RotationAngle = Mathf.Sign(playerToMouseVector.y) * 90f;
        }
        else if (angle > 26.5650f)
        {
            m_RotationAngle = Mathf.Sign(playerToMouseVector.y) * 45f;
        }
        else
        {
            m_RotationAngle = 0;
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

        if (m_isAttacking)
        {
            if (95 > m_RotationAngle && m_RotationAngle > 85)
            {
                ChangeAnimationState(PlayerAnimations.PLAYER_BOW_UP);
            }
            else if (50 > m_RotationAngle && m_RotationAngle > 40)
            {
                ChangeAnimationState(PlayerAnimations.PLAYER_BOW_UPPER_RIGTH);
            }
            else if (-50 < m_RotationAngle && m_RotationAngle < -40)
            {
                ChangeAnimationState(PlayerAnimations.PLAYER_BOW_LOWER_RIGHT);
            }
            else if (-95 < m_RotationAngle && m_RotationAngle < -85)
            {
                ChangeAnimationState(PlayerAnimations.PLAYER_BOW_DOWN);
            }
            else
            {
                ChangeAnimationState(PlayerAnimations.PLAYER_BOW_RIGTH);
            }
            
        }
        else if (m_IsClimbing)
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
            StartRoll();
            lastInvulnerable = Time.time;
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
