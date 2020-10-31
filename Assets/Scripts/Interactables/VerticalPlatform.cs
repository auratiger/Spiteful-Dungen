using DefaultNamespace;
using UnityEngine;

namespace Interactables
{
    public class VerticalPlatform : MonoBehaviour
    {
        private PlatformEffector2D effector;
        private Rigidbody2D m_Rigidbody2D;

#region Unity Functions

        // Start is called before the first frame update
        void Start()
        {
            effector = GetComponent<PlatformEffector2D>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                effector.rotationalOffset = 180f;
            }

            if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
            {
                effector.rotationalOffset = 0f;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (m_Rigidbody2D.IsTouchingLayers(LayerMask.GetMask(Layers.Player))) return;

            var body = other.collider.gameObject.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.gravityScale = 0;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            var body = other.collider.gameObject.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.gravityScale = 1;
            }
        }
        
#endregion

    }
}
