using UnityEngine;

public class Archers : MonoBehaviour
{
    public Vector2 boxSize = new Vector2(1f, 1f); // Adjust to match your collider
    public LayerMask detectionLayer; // Set this to the layer your "Angel" is on

    private void Update()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f, detectionLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Angel"))
            {
                Debug.Log("Multiple angels detected!");
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}

