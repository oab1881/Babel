using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class Archers : MonoBehaviour
{
    /*This can be overhauled instead of detecting side each Archer position covers a certain point.
    Use a bool which says which side it covers
    Doesn't need a reference to both parents

    Bugs possible line 153
    */


    //Get particle system references for arrows
    [SerializeField]
    private GameObject leftArrowsParent; //assigned in inspector
    [SerializeField]
    private GameObject rightArrowsParent; //assigned in inspector

    //THe particle prefab
    [SerializeField]
    GameObject GO_Particles;

    // Radius of the detection circle in world units
    private float detectionRadius = 0;

    // Which layers this archer should detect (e.g., only "Angel" layer)
    public LayerMask detectionLayer;

    // Public property to get/set detection radius
    //Value set in Archers.cs
    public float DetectionRadius
    {
        set { detectionRadius = value; }
        get { return detectionRadius; }
    }

    private bool isDamaging = false;
    private Collider2D[] hits = new Collider2D[0];

    private float damageASecond = 1.7f; //HardCoded for now

    private void Start()
    {
        leftArrowsParent.SetActive(true);   //used to debug, but it works
        rightArrowsParent.SetActive(true);
    }

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

            //Archer particle System Logic - set up so that arrows fire only on the side with an angel
            foreach (var hit in hits)
            {
                //Determines if that their are angels in the list
                //Checks to make sure they are actually angels based on tag
                //Then checks which side they are for firing either right or left

                //For right
                if (hit != null && hit.CompareTag("Angel") && hit.transform.position.x > transform.position.x)
                {
                    GameObject particle = Instantiate(GO_Particles, rightArrowsParent.transform);
                    

                    //Add rotations for aiming here **************
                    particle.GetComponent<ParticleSystem>().Play();

                    //Play arrow sound effect
                    AudioManager.PlaySoundEffect("arrows", 13);
                }

                //For Left
                if (hit != null && hit.CompareTag("Angel") && hit.transform.position.x < transform.position.x)
                {
                    GameObject particle = Instantiate(GO_Particles, leftArrowsParent.transform);
                    

                    //Rotates to the left side
                    //CAN ADD AIMING HERE
                    particle.transform.Rotate(new Vector3(0, 180, 0));
                    particle.GetComponent<ParticleSystem>().Play();

       
                    //Play arrow sound effect
                    AudioManager.PlaySoundEffect("arrows", 13);

                }
            }

            yield return new WaitForSeconds(.2f);


            // Refresh hit list every loop
            hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, detectionLayer);

            bool hasAngels = false;

            foreach (var hit in hits)
            {
                if (hit != null && hit.CompareTag("Angel"))
                {
                    hasAngels = true;
                    Debug.Log("Damaging angel: " + hit.name);


                    if (hit != null)
                    {

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
            }

            if (!hasAngels)
            {
                //I think this code could be messing things up where archers stop shooting

                //Goes through all the children of the parent shooters and deletes their children
                List<GameObject> Lchildren = GameManager.GetAllChildren(leftArrowsParent);
                for (int i = 0; i < Lchildren.Count; i++)
                {
                    Destroy(Lchildren[i]);
                    i++;
                }

                List<GameObject> Rchildren = GameManager.GetAllChildren(rightArrowsParent);
                for (int i = 0; i < Rchildren.Count; i++)
                {
                    Destroy(Rchildren[i]);
                    i++;
                }


                //Stops damaging and breaks the while loop
                isDamaging = false;
                yield break;
            }

            //Time between attacks
            yield return new WaitForSeconds(1.3f);
        }
    }

}
