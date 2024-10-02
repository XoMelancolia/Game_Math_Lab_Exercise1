using UnityEngine;

namespace GameMath.Gizmos
{
    [ExecuteAlways]
    public class PositionGizmo : MonoBehaviour
    {
        [SerializeField] private float radius = 0.1f;
        [SerializeField] private Color color = Color.red;
    
        private void OnDrawGizmos()
        {
            UnityEngine.Gizmos.color = color;
            UnityEngine.Gizmos.DrawSphere(transform.position, radius);
        }
    }
}