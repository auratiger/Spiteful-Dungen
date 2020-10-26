using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// this child class is designed for giving the character random movements in the world
public class MainScreenPlayerMovements : Player
{
    [SerializeField] private float walkTime = 5f;
    [SerializeField] private float waitTime = 5f;

    private bool isWalking;
    private bool exit;
    private float walkCounter;
    private float waitCounter;
    private int WalkDirection;
    
    private void Start()
    {
        myRidigBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();

        waitCounter = waitTime;
        ChoseDirection();
    }

    // Update is called once per frame
    void Update()
    {
        if (!exit)
        {
            RandomCharacterMovement();
        }
        else
        {
            RunRigth();
        }
    }

    private void RandomCharacterMovement()
    {
        if (isWalking)
        {
            // start animation
            // myAnimator.SetBool(IsRunning, true);

            walkCounter -= Time.deltaTime;
            
            if (walkCounter < 0)
            {
                isWalking = false;
                waitCounter = waitTime;
            }


            switch (WalkDirection)
            {
                case 0:
                    myRidigBody.velocity = new Vector2(-movementSpeed, myRidigBody.velocity.y);
                    break;                    
                case 1:
                    myRidigBody.velocity = new Vector2(movementSpeed, myRidigBody.velocity.y);
                    break;
            }

            FlipSprite();

        }
        else
        {
            // stop animation
            // myAnimator.SetBool(IsRunning, false);

            waitCounter -= Time.deltaTime;
            
            if (waitCounter < 0)
            {
                myRidigBody.velocity = Vector2.zero;
                
                ChoseDirection();
            }
        }
    }

    private void RunRigth()
    {
        myRidigBody.velocity = new Vector2(movementSpeed, myRidigBody.velocity.y);
        
        FlipSprite();

        // myAnimator.SetBool(IsRunning, true);
    }

    public void TriggerExit()
    {
        exit = true;
    }

    private void ChoseDirection()
    {
        WalkDirection = Random.Range(0, 2);
        isWalking = true;
        walkCounter = walkTime;
    }
    
}
