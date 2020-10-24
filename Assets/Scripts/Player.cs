using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [SerializeField] protected float runSpeed = 5f;
    [SerializeField] protected float jumpSpeed = 5f;
    [SerializeField] protected float climbSpeed = 5f;
    [SerializeField] protected Vector2 deathKick  = new Vector2(250f, 250f);
    
    protected Rigidbody2D myRidigBody;
    protected Animator myAnimator;
    protected CapsuleCollider2D bodyCollider;
    protected BoxCollider2D legsCollider;
    protected float gravitySlaceAtStart;

    protected bool _isAlive = true;
    protected bool _isInvulnerable = false;
    
    // Input Controls
    protected const string HORIZONTAL = "Horizontal";
    protected const string VERTICAL = "Vertical";
    protected const string FIRE3 = "Fire3";
    protected const string JUMP = "Jump";

    // Layer names
    protected const string Ground = "Ground";
    protected const string Climbing = "Climbing";
    protected const string Enemy = "Enemy";
    protected const string Hazards = "Hazards";


    // Animator bool hashes
    protected static readonly int IsRunning = Animator.StringToHash("isRunning");
    protected static readonly int IsClimbing = Animator.StringToHash("isClimbing");
    protected static readonly int Die = Animator.StringToHash("Die");
    protected static readonly int isRoll = Animator.StringToHash("Roll");


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
        if (!_isAlive) return;
        
        Run();
        Roll();
        ClimbLadder();
        Jump();
        FlipSprite();
        TriggerDeath();
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis(HORIZONTAL);
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRidigBody.velocity.y);
        myRidigBody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRidigBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool(IsRunning, playerHasHorizontalSpeed);
    }

    private void Jump()
    {
        if (!legsCollider.IsTouchingLayers(LayerMask.GetMask(Ground))) return;
        
        if (CrossPlatformInputManager.GetButtonDown(JUMP))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRidigBody.velocity += jumpVelocityToAdd;
        }
    }

    private void Roll()
    {
        if (!legsCollider.IsTouchingLayers(LayerMask.GetMask(Ground))) return;

        if (CrossPlatformInputManager.GetButtonDown(FIRE3))
        {
            myAnimator.SetTrigger(isRoll);
        }
    }

    private void ClimbLadder()
    {
        if (!legsCollider.IsTouchingLayers(LayerMask.GetMask(Climbing)))
        {
            myAnimator.SetBool(IsClimbing, false);
            myRidigBody.gravityScale = gravitySlaceAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis(VERTICAL);
        Vector2 climbVelocity = new Vector2(myRidigBody.velocity.x, controlThrow * climbSpeed);
        myRidigBody.velocity = climbVelocity;
        myRidigBody.gravityScale = 0;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRidigBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool(IsClimbing, playerHasHorizontalSpeed);
    }

    protected void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRidigBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRidigBody.velocity.x), 1f);
        }
    }

    protected void TriggerDeath()
    {
        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask(Enemy, Hazards)))
        {
            if (_isInvulnerable) return;
            
            _isAlive = false;
            myAnimator.SetTrigger(Die);
            myRidigBody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    public void SetMovementSpeed(int speed)
    {
        runSpeed = speed;
    }

    public void BecomeInvulnerable()
    {
        _isInvulnerable = true;
    }

    public void DisableInvulnerable()
    {
        _isInvulnerable = false;
    }

}
