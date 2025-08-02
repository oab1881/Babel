using System.Collections;
using UnityEngine;

public class AngleMovement : MonoBehaviour
{
    //Speed at which the angel slowly descends
    [SerializeField] private float descendSpeed = 1.5f;

    //Speed at which the angel dashes horizontally into the tower
    [SerializeField] private float dashSpeed = 10f;

    //How far left/right the angel sways while descending
    [SerializeField] private float swayAmplitude = 0.5f;

    //How fast the angel sways left/right
    [SerializeField] private float swayFrequency = 2f;

    //How close the angel must be to snap to position
    [SerializeField] private float arriveThreshold = 0.1f;

    //Reference to the target floor
    private Transform target;

    //Target floor index (used to determine which floor to damage)
    private int targetInd;

    //Enum to track movement phase (descending vs dashing)
    private enum MovePhase { Descend, Dash }

    //Current movement phase
    private MovePhase phase = MovePhase.Descend;

    //Y position the angel needs to reach before dashing
    private float targetY;

    //Final X position the angel moves to (side of the tower)
    private float targetX;

    //Determines if this angel spawned on the right side
    private bool movingRight;

    //Base X position used for swaying
    private float baseX;

    private float health = 3;

    [SerializeField]
    private GameObject explosionPrefab; //will spawn at angel's death location

    //Makes all the angles sway in different patterns
    private float swayOffset;

    //Called externally to set the floor this angel should attack
    public void SetTarget(Transform newTarget, int targetIndex, bool spawnOnRight)
    {
        target = newTarget;
        targetInd = targetIndex;
        float baseTargetY = target.position.y;

        // Higher tiers lock in earlier (higher up)
        if (GameManager.herecy >= 90)
            targetY = baseTargetY + 1.5f;
        else if (GameManager.herecy >= 80)
            targetY = baseTargetY + 1.0f;
        else if (GameManager.herecy >= 70)
            targetY = baseTargetY + 0.5f;
        else
            targetY = baseTargetY;

        movingRight = spawnOnRight;

        //Set X target to the side of the tower
        float towerWidth = 0.5f; //Change this if tower width changes
        targetX = target.position.x + (movingRight ? towerWidth : -towerWidth);

        //Save current X as base position for swaying
        baseX = transform.position.x;
        
        //Adds an offset to sway so the angles don't move the exact same
        swayOffset = Random.Range(0f, 2f * Mathf.PI); // Random phase offset between 0 and 2 PI

        
        //Arbitrary scaling based on current herecy
        //Makes the angle bigger and gives more health
        if (GameManager.herecy >= 90)
        {
            health = 9.5f;
            transform.localScale = new Vector3(0.4f, 0.4f, 0);
            Debug.Log("Angle tier 3");
        }
        else if (GameManager.herecy >= 80)
        {
            health = 8f;
            transform.localScale = new Vector3(0.3f, 0.3f, 0);
            Debug.Log("Angle tier 2");
        }

        else if (GameManager.herecy >= 70)
        {
            health = 6f;
            transform.localScale = new Vector3(0.25f, 0.25f, 0);
            Debug.Log("Angle tier 1");
        }
        else if(GameManager.herecy < 70)
        {
            transform.localScale = new Vector3(0.2f, 0.2f, 0);
        }
    }

private void Update()
    {
        if (target == null) return;

        Vector3 pos = transform.position;

        //PHASE 1: DESCEND
        if (phase == MovePhase.Descend)
        {
            //Move down to match the target Y level
            if (Mathf.Abs(pos.y - targetY) > arriveThreshold)
            {
                pos.y = Mathf.MoveTowards(pos.y, targetY, descendSpeed * Time.deltaTime);

                //Sway left/right during descent
                pos.x = baseX + Mathf.Sin(Time.time * swayFrequency + swayOffset) * swayAmplitude;
            }
            else
            {
                //Snap to final Y, lock X position for the dash phase
                pos.y = targetY;
                baseX = pos.x;
                phase = MovePhase.Dash;
            }
        }

        //PHASE 2: DASH
        else if (phase == MovePhase.Dash)
        {
            //Move quickly into the side of the tower
            pos.x = Mathf.MoveTowards(pos.x, targetX, dashSpeed * Time.deltaTime);

            //If we are close enough to the edge, trigger arrival
            if (Mathf.Abs(pos.x - targetX) <= arriveThreshold)
            {
                OnArrive();
            }
        }

        //Apply position change
        transform.position = pos;
    }

    //Stops movement once angel reaches its final X
    private void OnArrive()
    {
        target = null;
        GameManager.DecreaseHealth(1); //Temporary fixed damage value
        //Add sound effect
        AudioManager.PlaySoundEffect("explode 3", 12);
        //Add explosion effect
        CameraShake.Shake();
        Destroy(gameObject);
        ResumeMusic();
    }

    //Detect collision with tower and deal damage
    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            GameManager.DecreaseHealth(1); //Temporary fixed damage value
            
        }
    }*/

    public void DecreaseAngleHealth(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Angle Dead");

            // Instantiate explosion prefab at this position
            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                AudioManager.PlaySoundEffect("explode 3", 12);

                // Optionally destroy the explosion object after it finishes
                Destroy(explosion, 1f); // 1 second delay, adjust as needed
            }

            //Resume the music when angel dies
            ResumeMusic();
            Destroy(gameObject);
        }
    }
    
    //Interrupt main theme when angel arrives to play angel theme
    public static void PlayMusicOnSpawn()
    {
        AudioManager.StopSound(0);
        AudioManager.PlayMusic("AngelAttack", 0);

    }
    //Resume music after angel dies
    public static void ResumeMusic()
    {
        AudioManager.StopSound(0);
        AudioManager.PlayMusic("MesopotamianLullaby", 0);
    }

}
