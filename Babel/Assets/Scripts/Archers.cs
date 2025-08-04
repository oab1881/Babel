using UnityEngine;
using System.Collections.Generic;

public class Archers : MonoBehaviour
{
    // The particle prefab
    [SerializeField]
    GameObject GO_Particles;

    // Radius of the detection circle in world units
    private float detectionRadius = 0;

    // Which layers this archer should detect (e.g., only "Angel" layer)
    public LayerMask detectionLayer;

    // Public property to get/set detection radius
    // Value set in Archers.cs
    public float DetectionRadius
    {
        set { detectionRadius = value; }
        get { return detectionRadius; }
    }

    // Hardcoded damage value per tick for now
    private float damageASecond = 1.7f;

    // Cooldowns between damage and visual arrow firing
    private float attackCooldown = 1.3f;
    private float arrowEffectCooldown = 0.9f;

    // Internal timers
    private float attackTimer = 0f;
    private float arrowTimer = 0f;

    private bool canAttack = false;

    public bool CanAttack { set { canAttack = value; } get { return canAttack; } }

    public float DamageASecond { set { damageASecond = value; } }

    private void Update()
    {
        // Check for all colliders within detection radius that match the layer mask
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, detectionLayer);

        // Prepare a list of valid targets on the correct side
        List<Collider2D> validTargets = new List<Collider2D>();
        float archerX = transform.position.x;
        bool isLeftSideArcher = archerX < 0f;
        
        foreach (var hit in hits)
        {
            if (canAttack)
            {
                if (hit != null && hit.CompareTag("Angel"))
                {
                    float angelX = hit.transform.position.x;

                    // Only include angels on the correct side
                    if ((isLeftSideArcher && angelX < archerX) || (!isLeftSideArcher && angelX > archerX))
                    {
                        validTargets.Add(hit);
                    }
                }
            }
        }

        // No valid targets — reset timers and clean up
        if (validTargets.Count == 0)
        {
            attackTimer = 0f;
            arrowTimer = 0f;

            // Goes through all the children of the parent shooters and deletes their children if they're done playing
            List<GameObject> children = Utilities.GetAllChildren(gameObject);
            for (int i = 0; i < children.Count; i++)
            {
                if (!children[i].GetComponent<ParticleSystem>().isPlaying)
                {
                    Destroy(children[i]);
                    i++;
                }
            }

            return;
        }

        // Update countdown timers
        attackTimer -= Time.deltaTime;
        arrowTimer -= Time.deltaTime;

        // Archer particle system logic — arrows fire only on the side with an angel
        if (arrowTimer <= 0f)
        {
            FireArrows(validTargets);
            arrowTimer = arrowEffectCooldown;
        }

        // Deals damage to angels at a timed interval
        if (attackTimer <= 0f)
        {
            DamageAngels(validTargets);
            attackTimer = attackCooldown;
        }
    }


    private void FireArrows(List<Collider2D> hits)
    {
        foreach (var hit in hits)
        {
            if (hit != null && hit.CompareTag("Angel"))
            {
                // Spawn the arrow in world space at the archer's position
                GameObject particle = Instantiate(GO_Particles, transform.position, Quaternion.identity);

                // Get 3D direction from archer to angel
                Vector3 targetPosition = hit.transform.position;
                Vector3 direction = (targetPosition - transform.position).normalized;

                // Map 2D to 3D — use X and Y from 2D, but Z instead of Y for vertical aiming
                Vector3 lookDirection = new Vector3(direction.x, direction.y, 0f);

                if (lookDirection != Vector3.zero)
                {
                    // Rotate the particle to face the angel in 3D space (handles both yaw and pitch)
                    particle.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.back);
                    // Use Vector3.back as "up" to make Z-forward arrows pitch up/down
                }

                // Play arrow visual + sound
                particle.GetComponent<ParticleSystem>().Play();
                AudioManager.PlaySoundEffect("arrows", 13);
            }
        }
    }




    private void DamageAngels(List<Collider2D> hits)
    {
        foreach (var hit in hits)
        {
            if (hit != null && hit.CompareTag("Angel"))
            {
                Debug.Log("Damaging angel: " + hit.name);

                var angelMovement = hit.GetComponent<AngleMovement>();
                if (angelMovement != null)
                {
                    // Damage angel
                    angelMovement.DecreaseAngleHealth(damageASecond);

                    // Trigger the angel's animator
                    var animator = hit.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetTrigger("AngelHit");
                    }
                }
            }
        }
    }

    // Draw detection radius in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
