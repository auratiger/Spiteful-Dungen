using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    [SerializeField] protected Vector2 deathKick  = new Vector2(250f, 250f);
    
    protected Rigidbody2D myRidigBody;
    protected Animator myAnimator;
    protected CapsuleCollider2D bodyCollider;
    protected BoxCollider2D legsCollider;
    protected float gravitySlaceAtStart;

    // player states
    private bool m_IsRunning = false;
    private bool m_IsRolling = false;
    private bool m_IsInvulnerable = false;
    private bool m_IsAlive = true;

    // animation state
    private int m_CurrentState;

#region Unity Functions 

    // Start is called before the first frame update
    void Start()
    {
        myRidigBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        legsCollider = GetComponent<BoxCollider2D>();
        
        gravitySlaceAtStart = myRidigBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_IsAlive) return;

        var isGrounded = legsCollider.IsTouchingLayers(LayerMask.GetMask(Layers.Ground));
        var isClimbing = legsCollider.IsTouchingLayers(LayerMask.GetMask(Layers.Climbing));
        
        Move();
        FlipSprite();
        Jump(isGrounded);
        Roll(isGrounded);
        ClimbLadder(isClimbing);
        
        HandleAnimation(isClimbing);
        
        // TriggerDeath();
    }
    
#endregion

#region PublicFunctions

    public void StopRoll()
    {
        m_IsRolling = false;
        m_IsInvulnerable = false;
    }
    
    public void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRidigBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRidigBody.velocity.x), 1f);
        }
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
        myRidigBody.velocity = deathKick;
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }

#endregion

#region Private Functions 

    private void Move()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis(Controls.HORIZONTAL);
                
        Vector2 playerVelocity = new Vector2(controlThrow * movementSpeed, myRidigBody.velocity.y);
        myRidigBody.velocity = playerVelocity;
        
        
        if (!m_IsRunning)
        {
            m_IsRunning = CrossPlatformInputManager.GetButtonDown(Controls.FIRE3);
        }
        else
        {
            m_IsRunning = !CrossPlatformInputManager.GetButtonUp(Controls.FIRE3);
        }
        
    }

    private void HandleAnimation(bool isClimbing)
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRidigBody.velocity.x) > Mathf.Epsilon;

        if (isClimbing)
        {
            // TODO make it so the player can stand still on the top of the ladder
            
            bool playerVerticalSpeed = Mathf.Abs(myRidigBody.velocity.y) > Mathf.Epsilon;
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

    private void Jump(bool isGrounded)
    {
        if(!isGrounded) return;
        
        if (CrossPlatformInputManager.GetButtonDown(Controls.JUMP))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRidigBody.velocity += jumpVelocityToAdd;
        }
    }

    private void Roll(bool isGrounded)
    {
        if(!isGrounded) return;
        
        if (CrossPlatformInputManager.GetButtonDown(Controls.FIRE2))
        {
            m_IsRolling = true;
            m_IsInvulnerable = true;
        }
    }


    private void ClimbLadder(bool isClimbing)
    {
        if (!isClimbing)
        {
            myRidigBody.gravityScale = gravitySlaceAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis(Controls.VERTICAL);
        Vector2 climbVelocity = new Vector2(myRidigBody.velocity.x, controlThrow * climbSpeed);
        myRidigBody.velocity = climbVelocity;
        myRidigBody.gravityScale = 0;
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        Enemy enemy = other.collider.GetComponent<Enemy>();

        if (enemy != null)
        {
            foreach (ContactPoint2D point in other.contacts)
            {
                Debug.Log(point.normal);

                if (point.normal.y >= 0.9f)
                {
                    Vector2 jumpVelocityToAdd = new Vector2(myRidigBody.velocity.x, jumpSpeed);
                    myRidigBody.velocity = jumpVelocityToAdd;

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
