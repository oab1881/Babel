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
    [SerializeField]
    float speed;

    [SerializeField]
    Rigidbody2D rb;
    // Update is called once per frame
    void FixedUpdate()
    {

        //Creates a zeroed out velocity
        Vector2 move = Vector2.zero;

        //Changes it to the relative vectors
        if (Input.GetKey(KeyCode.W)) move += Vector2.up;
        if (Input.GetKey(KeyCode.S)) move += Vector2.down;

        //Sets the velocity
        rb.velocity = move * speed * Time.deltaTime;

    }
}
