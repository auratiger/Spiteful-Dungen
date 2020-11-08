using System;
using DefaultNamespace;
using UnityCore.Scene;
using UnityEngine;

namespace Interactables
{
    public class GateController : MonoBehaviour
    {
        [SerializeField] private bool passable;
        
        private Animator myAnimator;
        private Rigidbody2D m_Rigidbody2D;
    
        private static readonly int Rise = Animator.StringToHash("Rise");

        private bool isOpen = false;

        private InputManager inputmanager;

#region Unity Functions

        private void Awake()
        {
            myAnimator = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            
            inputmanager = new InputManager();
        }

        private void OnEnable()
        {
            inputmanager?.Enable();
        }

        private void OnDisable()
        {
            inputmanager?.Disable();
        }

        private void Update()
        {
            if (passable)
            {
                PlayerEnter();
            }
        }

#endregion

#region Public Functions

        public void OpenGate()
        {
            myAnimator.SetBool(Rise, true);
            isOpen = true;
        }

        public void CloseGate()
        {
            myAnimator.SetBool(Rise, false);
        }

        public bool isPassable()
        {
            return passable;
        }
        
#endregion

#region Private Functions

        private void PlayerEnter()
        {
            if (m_Rigidbody2D.IsTouchingLayers(LayerMask.GetMask(Layers.Player)))
            {
                if (isOpen && inputmanager.Player.Interact.triggered)
                {
                    FindObjectOfType<SceneLoader>().LoadNextScene();
                }
            }
        }

#endregion

    }
}
