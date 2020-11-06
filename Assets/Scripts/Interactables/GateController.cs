using DefaultNamespace;
using UnityCore.Scene;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Interactables
{
    public class GateController : MonoBehaviour
    {
        [SerializeField] private bool passable;
        
        private Animator myAnimator;
        private Rigidbody2D m_Rigidbody2D;
    
        private static readonly int Rise = Animator.StringToHash("Rise");

        private bool isOpen = false;

#region Unity Functions

        private void Awake()
        {
            myAnimator = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
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
                if (isOpen && CrossPlatformInputManager.GetAxis(Controls.VERTICAL) > 0.1)
                {
                    FindObjectOfType<SceneLoader>().LoadNextScene();
                }
            }
        }

#endregion

    }
}
