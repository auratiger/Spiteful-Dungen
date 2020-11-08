using System;
using DefaultNamespace;
using UnityEngine;

namespace Interactables
{
    public class VerticalPlatform : MonoBehaviour
    {
        private PlatformEffector2D effector;
        private Rigidbody2D m_Rigidbody2D;

        private InputManager inputManager;

#region Unity Functions

        private void Awake()
        {
            inputManager = new InputManager();
        }

        void Start()
        {
            effector = GetComponent<PlatformEffector2D>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();

            
            inputManager.Platform.PressDown.performed += 
                ctx => RotatePlatformOffset(ctx.ReadValueAsButton());
        }
        
        private void OnEnable()
        {
            inputManager?.Enable();
        }

        private void OnDisable()
        {
            inputManager?.Disable();
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

#region Private Functions

        private void RotatePlatformOffset(bool isPressed)
        {
            if (isPressed)
            {
                effector.rotationalOffset = 180f;
            }
            else
            {
                effector.rotationalOffset = 0f;
            }
        }


#endregion

    }
}
