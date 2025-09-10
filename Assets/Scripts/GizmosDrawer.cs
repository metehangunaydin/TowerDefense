using UnityEngine;

public class GizmosDrawer : MonoBehaviour
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private Color color = Color.cyan;

    // Sahne görünümünde çizim yapar
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.DrawWireSphere(transform.position, radius + 0.1f);
        
        
    }
}
