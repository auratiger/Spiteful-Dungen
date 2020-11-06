using DefaultNamespace;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Attack")]
        [SerializeField] private GameObject shooter;
        [SerializeField] private GameObject projectile;
        [SerializeField] private float shootDelay = 0.5f;
        
        [Header("Player script")]
        [SerializeField] private Player player;

        private Camera camera;
        private float lastShot;

#region Unity Functions

        private void Start()
        {
            camera = Camera.main;
        }

        private void Update()
        {
            Shoot();
        }

#endregion

#region Public Functions

        public void StopAttack()
        {
            player.IsAttacking = false;
        }

#endregion

#region Private Functions

        private void Shoot()
        {
            if (CrossPlatformInputManager.GetButtonDown(Controls.FIRE1))
            {
                if (!player.IsClimbing && !player.IsRolling)
                {
                    if (Time.time - lastShot < shootDelay) return;
                        
                    Ray mousePosition = camera.ScreenPointToRay(Input.mousePosition);
                    Vector2 playerToMouseVector = mousePosition.origin - transform.position;

                    CalculateRotationAngle(playerToMouseVector);

                    // Rotate the player to where the mouse is pointing
                    if (playerToMouseVector.x < 0 && player.IsFacingRigth)
                    {
                        player.FlipSprite();
                    }
                    else if (playerToMouseVector.x > 0 && !player.IsFacingRigth)
                    {
                        player.FlipSprite();
                    }
                        
                    shooter.transform.Rotate(0f, 0f, player.RotationAngle);
                    player.IsAttacking = true;
                    lastShot = Time.time;
                        
                    GameObject arrow = Instantiate(projectile, shooter.transform.position, shooter.transform.rotation);
                        
                    shooter.transform.Rotate(0f, 0f, -player.RotationAngle);
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
                player.RotationAngle = Mathf.Sign(playerToMouseVector.y) * 90f;
            }
            else if (angle > 26.5650f)
            {
                player.RotationAngle = Mathf.Sign(playerToMouseVector.y) * 45f;
            }
            else
            {
                player.RotationAngle = 0;
            }
        }

#endregion

    }
}
