using UnityEngine;

public class Archers : MonoBehaviour
{
    // Radius of the detection circle in world units
    private float detectionRadius = 0;

    // Which layers this archer should detect (e.g., only "Angel" layer)
    public LayerMask detectionLayer;

    // Public property to get/set detection radius
    public float DetectionRadius
    {
        set { detectionRadius = value; }
        get { return detectionRadius; }
    }

    private void Update()
    {
        // Check for all colliders within detection radius that match the layer mask
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, detectionLayer);

        // Loop through detected objects
        foreach (var hit in hits)
        {
            // Only respond to objects tagged as "Angel"
            if (hit.CompareTag("Angel"))
            {
                Debug.Log("Multiple angels in range: " + hit.name);
            }
        }
    }

    // Visual aid in the editor to show the detection radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

}
