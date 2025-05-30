using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 50f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float scrollAdjuster = 10f;     // A bit stronger now
    [SerializeField] private float scrollDecay = 5f;           // How fast it slows down

    private float currentSpeed;
    private float scrollVelocity = 0f;

    void Start()
    {
        UpdateSpeed(); // Initialize the speed based on starting floor
    }

    void FixedUpdate()
    {
        Vector2 move = Vector2.zero;

        // Keyboard input
        if (Input.GetKey(KeyCode.W)) move += Vector2.up;
        if (Input.GetKey(KeyCode.S)) move += Vector2.down;

        // Apply scroll velocity as extra movement
        move += Vector2.up * scrollVelocity;

        // Apply combined movement
        rb.velocity = move * currentSpeed * Time.deltaTime;

        // Gradually reduce scroll velocity (smooth stop)
        scrollVelocity = Mathf.Lerp(scrollVelocity, 0, Time.fixedDeltaTime * scrollDecay);
    }

    void Update()
    {
        // Capture scroll input here (must be done in Update)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            scrollVelocity += scrollInput * scrollAdjuster;
        }
    }

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
