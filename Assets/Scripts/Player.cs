using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private Vector2 deathKick  = new Vector2(250f, 250f);
    
    private Rigidbody2D myRidigBody;
    private Animator myAnimator;
    private CapsuleCollider2D bodyCollider;
    private BoxCollider2D legsCollider;
    private float gravitySlaceAtStart;

    private bool isAlive = true;
    
    // Input Controls
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const string FIRE3 = "Fire3";
    private const string JUMP = "Jump";

    // Layer names
    private const string Ground = "Ground";
    private const string Climbing = "Climbing";
    private const string Enemy = "Enemy";
    private const string Hazards = "Hazards";


    // Animator bool hashes
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsClimbing = Animator.StringToHash("isClimbing");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int isRoll = Animator.StringToHash("Roll");


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
        if (!isAlive) return;
        
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

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRidigBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRidigBody.velocity.x), 1f);
        }
        
    }

    private void TriggerDeath()
    {
        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask(Enemy, Hazards)))
        {
            isAlive = false;
            myAnimator.SetTrigger(Die);
            myRidigBody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    public void SetMovementSpeed(int speed)
    {
        runSpeed = speed;
    }

}
