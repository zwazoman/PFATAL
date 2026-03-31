using UnityEngine;

public class MapBounds : MonoBehaviour
{
    public Bounds m_Bounds;

    public static MapBounds instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(m_Bounds.center, m_Bounds.size);
    }
}
