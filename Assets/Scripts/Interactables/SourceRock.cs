using DefaultNamespace;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Interactables
{
    public class SourceRock : MonoBehaviour
    {

        enum Direction
        {
            UP, DOWN, RIGHT, LEFT
        }

        [Tooltip("The direction the object needs to be hit")]
        [SerializeField] private Direction hitDirection;
    
        [Space]
        [Tooltip("Points though witch the platform passes")]
        [SerializeField] private GameObject[] waypoints;

        [Space]
        [SerializeField] private float speed = 5f;
    
        private PolygonCollider2D m_Collider;
        private Light2D m_Light;
    
        private int m_WaypointIndex = 0;
        private int m_IndexIncrement = 1;
        private bool isActive { get; set; }
    
        #region Unity Functions

        private void Awake()
        {
            m_Collider = GetComponent<PolygonCollider2D>();
            m_Light = GetComponentInChildren<Light2D>();
        }

        private void Update()
        {
            Move();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(Layers.Projectiles))
            {
                foreach (ContactPoint2D point in other.contacts)
                {
                    if ((point.normal.y >= 0.9f && hitDirection == Direction.DOWN) ||
                        (point.normal.y <= -0.9f && hitDirection == Direction.UP) ||
                        (point.normal.x >= 0.9f && hitDirection == Direction.LEFT) ||
                        (point.normal.x <= -0.9f && hitDirection == Direction.RIGHT))
                    {
                        TriggerRock();
                        return;
                    }
                }
            }
        
            other.transform.parent = gameObject.transform;

        }

        private void OnCollisionExit2D(Collision2D other)
        {
            other.transform.parent = null;
        }

        #endregion

        #region Private Functions

        private void TriggerRock()
        {
            if (isActive)
            {
                isActive = false;
                m_Light.color = Color.red;
            }
            else
            {
                isActive = true;
                m_Light.color = Color.green;
                ActivateGate();
            }
        }

        private void ActivateGate()
        {
            var rocks = FindObjectsOfType<SourceRock>();
            bool allActive = true;
        
            foreach (var rock in rocks)
            {
                if (!rock.isActive)
                {
                    allActive = false;
                }    
            }

            if (allActive)
            {
                FindObjectOfType<GateController>().OpenGate();
            }
        }

        private void Move()
        {
            if (m_WaypointIndex >= 0  && m_WaypointIndex <= waypoints.Length - 1)
            {
                var targetPosition = waypoints[m_WaypointIndex].transform.position;
            
                var moveThisFrame = speed * Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveThisFrame);

                if (transform.position == targetPosition)
                {
                    m_WaypointIndex += m_IndexIncrement;
                }
            }
            else
            {
                m_IndexIncrement *= -1;
                m_WaypointIndex += m_IndexIncrement;
            }
        }
    
        #endregion

    }
}
