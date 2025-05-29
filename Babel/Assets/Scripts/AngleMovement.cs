using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float arriveThreshold = 0.1f;

    private Transform target;
    private int targetInd;

    // Call this from outside to give the angel a new target floor
    public void SetTarget(Transform newTarget,int targetIndex)
    {
        target = newTarget;
        targetInd= targetIndex;
    }

    private void Update()
    {
        if (target == null) return;

        // Move toward the target
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Optionally: Stop when very close (to avoid jittering)
        if (Vector3.Distance(transform.position, target.position) <= arriveThreshold)
        {
            OnArrive();
        }
    }


    private void OnArrive()
    {
        target = null; // Stop moving
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Tower")
        {
            GameManager.DecreaseFloorHealth(targetInd, 40); //Arbitrary amount for now!
            Destroy(gameObject);
        }
    }
}
