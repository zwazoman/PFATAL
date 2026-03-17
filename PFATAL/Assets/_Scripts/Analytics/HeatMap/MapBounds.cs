using UnityEngine;

public class MapBounds : MonoBehaviour
{
    public Bounds m_Bounds;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(m_Bounds.center, m_Bounds.size);
    }
}
