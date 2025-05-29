//====== 05/26/25 Jake Wardell ======
/*
 *  Camera movement script - Handles player input and applying forces to camera character
 *  
 *  Attached to the camera
 * 
 * Dependencies: Camera RigidBody
 * 
 *  Changelog:
 *  
 * 
 */
//===================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 50f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float scrollAdjuster = 10.0f;

    private float currentSpeed;

    void Start()
    {
        UpdateSpeed(); //Initialize the speed based on starting floor
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Creates a zeroed out velocity
        Vector2 move = Vector2.zero;

        //Changes it to the relative vectors
        if (Input.GetKey(KeyCode.W)) move += Vector2.up;
        if (Input.GetKey(KeyCode.S)) move += Vector2.down;

        //Sets the velocity
        rb.velocity = move * currentSpeed * Time.deltaTime;

    }

    /*      Scroll Wheel movement not working
    private void Update()
    {
        //Creates a zeroed out velocity
        Vector2 move = Vector2.zero;

        //Scroll wheel movement
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        move += Vector2.up * scroll * scrollAdjuster; //sensitivity multiplier

        //Sets the velocity
        rb.velocity = move * currentSpeed * Time.deltaTime;
    } */

    //Dynamically edit camera speed based off of height level
    private void UpdateSpeed()
    {
        uint floor = GameManager.Instance.floor;

        if (floor >= 200)
            currentSpeed = baseSpeed * 3.0f;
        else if (floor >= 80)
            currentSpeed = baseSpeed * 2.0f;
        else if (floor >= 30)
            currentSpeed = baseSpeed * 1.5f;
        else
            currentSpeed = baseSpeed;
    }
}
