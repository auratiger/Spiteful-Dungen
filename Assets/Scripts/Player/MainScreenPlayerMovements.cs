using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

// this child class is designed for giving the character random movements in the world
namespace Player
{
    public class MainScreenPlayerMovements : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 5f;
    
        [SerializeField] private float walkTime = 5f;
        [SerializeField] private float waitTime = 5f;

        private Rigidbody2D myRigidBody;
        private Animator myAnimator;
    
        private bool isWalking;
        private bool exit;
        private float walkCounter;
        private float waitCounter;
        private int WalkDirection;
    
        private void Start()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
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
                myAnimator.Play(PlayerAnimations.PLAYER_WALK);

                walkCounter -= Time.deltaTime;
            
                if (walkCounter < 0)
                {
                    isWalking = false;
                    waitCounter = waitTime;
                }


                switch (WalkDirection)
                {
                    case 0:
                        myRigidBody.velocity = new Vector2(-movementSpeed, myRigidBody.velocity.y);
                        break;                    
                    case 1:
                        myRigidBody.velocity = new Vector2(movementSpeed, myRigidBody.velocity.y);
                        break;
                }

                FlipSprite();

            }
            else
            {
                // stop animation
                myAnimator.Play(PlayerAnimations.PLAYER_IDLE);
            
                waitCounter -= Time.deltaTime;
            
                if (waitCounter < 0)
                {
                    myRigidBody.velocity = Vector2.zero;
                
                    ChoseDirection();
                }
            }
        }

        private void RunRigth()
        {
            myRigidBody.velocity = new Vector2(movementSpeed, myRigidBody.velocity.y);
        
            FlipSprite();

            myAnimator.Play(PlayerAnimations.PLAYER_WALK);
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
    
        public void FlipSprite()
        {
            bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
            if (playerHasHorizontalSpeed)
            {
                transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
            }
        }

        public void SetMovementSpeed(float speed)
        {
            movementSpeed = speed;
        }
    
    }
}
