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

    private int health = 5;

    //Called externally to set the floor this angel should attack
    public void SetTarget(Transform newTarget, int targetIndex, bool spawnOnRight)
    {
        target = newTarget;
        targetInd = targetIndex;
        targetY = target.position.y;
        movingRight = spawnOnRight;

        //Set X target to the side of the tower
        float towerWidth = 0.5f; //Change this if tower width changes
        targetX = target.position.x + (movingRight ? towerWidth : -towerWidth);

        //Save current X as base position for swaying
        baseX = transform.position.x;
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
                pos.x = baseX + Mathf.Sin(Time.time * swayFrequency) * swayAmplitude;
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
        GameManager.DecreaseFloorHealth(targetInd, 400000); //Temporary fixed damage value
        //Add sound effect
        //Add explosion effect
        Destroy(gameObject);
    }

    //Detect collision with tower and deal damage
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            GameManager.DecreaseFloorHealth(targetInd, 40); //Temporary fixed damage value
            
        }
    }

    public void DecreaseAngleHealth(int damage)
    {
        if(damage > 5)
        {
            Debug.Log("Angle Dead");
            Destroy(gameObject); 
        }
    }
}
