using DefaultNamespace;
using UnityCore.Scene;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Interactables
{
    public class GateController : MonoBehaviour
    {
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
            if (m_Rigidbody2D.IsTouchingLayers(LayerMask.GetMask(Layers.Player)))
            {
                if (!isOpen && CrossPlatformInputManager.GetButtonDown(Controls.INTERACT))
                {
                    OpenGate();
                }

                if (isOpen && CrossPlatformInputManager.GetButtonDown(Controls.VERTICAL))
                {
                    FindObjectOfType<SceneLoader>().LoadNextScene();
                }
            }
        }

#endregion

#region Public Functions

        public void OpenGate()
        {
            myAnimator.SetBool(Rise, true);
        }

        public void CloseGate()
        {
            myAnimator.SetBool(Rise, false);
        }


#endregion

    }
}
