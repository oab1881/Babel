using System.Collections;
using UnityEngine;

public class Archers : MonoBehaviour
{
    //Get particle system references for arrows
    [SerializeField]
    private ParticleSystem leftArrows; //assigned in inspector
    [SerializeField]
    private ParticleSystem rightArrows; //assigned in inspector

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

    private float damageASecond = 0.5f; //HardCoded for now

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

        //Determine which side to fire from based on the first angel in range
        bool angelOnRight = false;
        bool angelFound = false;

        //Archer particle System Logic - set up so that arrows fire only on the side with an angel
        //Look for angel and determine if the angel is on the right
        foreach (var hit in hits)
        {
            if (hit != null && hit.CompareTag("Angel"))
            {
                angelFound = true;
                angelOnRight = hit.transform.position.x > transform.position.x;
                break;
            }
        }

        //When an angel is found, fire arrows on the correct side
        if (angelFound)
        {
            if (angelOnRight)
            {
                if (rightArrows != null) rightArrows.Play();
                if (leftArrows != null) leftArrows.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            else
            {
                if (leftArrows != null) leftArrows.Play();
                if (rightArrows != null) rightArrows.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

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

                    // Damage angel
                    var angelMovement = hit.GetComponent<AngleMovement>();
                    if (angelMovement != null)
                    {
                        angelMovement.DecreaseAngleHealth(damageASecond);

                        // Trigger the angel's animator
                        var animator = hit.GetComponent<Animator>();
                        if (animator != null)
                        {
                            animator.SetTrigger("AngelHit");
                        }

                        hit.GetComponent<AngleMovement>().DecreaseAngleHealth(damageASecond);
                    }

                }
            }

            if (!hasAngels)
            {
                //Stop and deactivate particle systems
                if (leftArrows != null)
                {
                    leftArrows.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
                if (rightArrows != null)
                {
                    rightArrows.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }

                isDamaging = false;
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

}
