using System.Collections;
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

    private bool isDamaging = false;
    private Collider2D[] hits = new Collider2D[0];

    private int damageASecond = 1; //HardCoded for now

    private void Update()
    {
        // Check for all colliders within detection radius that match the layer mask
        hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, detectionLayer);

        // If there are angels in range and we're not already damaging, start the coroutine
        if (hits.Length > 0 && !isDamaging)
        {
            StartCoroutine(DamageAngelsLoop());
        }
    }

    // Draw detection radius in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    // Continuously damage Angels in range once per second
    private IEnumerator DamageAngelsLoop()
    {
        isDamaging = true;

        while (true)
        {
            // Refresh hit list every loop
            hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, detectionLayer);

            bool hasAngels = false;

            foreach (var hit in hits)
            {
                if (hit != null && hit.CompareTag("Angel"))
                {
                    hasAngels = true;
                    Debug.Log("Damaging angel: " + hit.name);
                    hit.GetComponent<AngleMovement>().DecreaseAngleHealth(damageASecond);
                }
            }

            if (!hasAngels)
            {
                isDamaging = false;
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

}
