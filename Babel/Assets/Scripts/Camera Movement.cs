using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float baseSpeed = 50f;               // Speed multiplier baseline
    [SerializeField] private Rigidbody2D rb;                      // Reference to camera's Rigidbody2D

    [Header("Scroll and Momentum")]
    [SerializeField] private float scrollAdjuster = 10f;          // How strong the scroll/swipe adds velocity
    [SerializeField] private float scrollDecay = 5f;              // How fast the added scroll momentum fades out

    [Header("Touch Controls")]
    [SerializeField] private float swipeSensitivity = 0.01f;      // How much vertical swipe movement is needed to register (percentage of screen height)

    private float currentSpeed;                                   // Actual movement speed adjusted by current floor
    private float scrollVelocity = 0f;                            // Momentum from scroll wheel or swipe

    // For swipe detection
    private Vector2 touchStartPos;                                // Where the finger first touched
    private bool isSwiping = false;                               // Whether we’re actively tracking a swipe

    void Start()
    {
        UpdateSpeed(); // Initialize currentSpeed based on player's progress (floor)
    }

    void FixedUpdate()
    {
        Vector2 move = Vector2.zero;

        // PC Keyboard movement
        if (Input.GetKey(KeyCode.W)) move += Vector2.up;
        if (Input.GetKey(KeyCode.S)) move += Vector2.down;

        // Scroll momentum from scroll wheel or swipe (acts as an extra up/down force)
        move += Vector2.up * scrollVelocity;

        // Apply movement to Rigidbody (frame-rate independent with Time.deltaTime)
        rb.velocity = move * currentSpeed * Time.fixedDeltaTime;

        // Gradually reduce scroll momentum toward 0 over time
        scrollVelocity = Mathf.Lerp(scrollVelocity, 0, Time.fixedDeltaTime * scrollDecay);
    }

    void Update()
    {
        // Scroll wheel input (works in editor, standalone, WebGL)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Only apply scroll if it’s significant (avoid noise)
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            scrollVelocity += scrollInput * scrollAdjuster;
        }

        // Mobile-specific swipe logic
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        // No active touch — skip
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0); // Only handle first finger (1-touch system)

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStartPos = touch.position;  // Record where the touch started
                isSwiping = true;
                break;

            case TouchPhase.Moved:
                if (!isSwiping) break;

                Vector2 delta = touch.position - touchStartPos;

                // Only count if vertical movement is dominant and large enough
                if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x) && Mathf.Abs(delta.y) > Screen.height * swipeSensitivity)
                {
                    float direction = -Mathf.Sign(delta.y); // 1 = up swipe, -1 = down swipe

                    // Add to scroll velocity (positive = up, negative = down)
                    scrollVelocity += direction * (scrollAdjuster / 4); //Divided the scrollAdjuster to stop it from launching to the top of the tower

                    isSwiping = false; // Prevents multiple swipes from one gesture
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isSwiping = false; // Reset swipe state
                break;
        }
    }

    private void UpdateSpeed()
    {
        // Get current floor from GameManager (can’t compile unless GameManager.Instance.floor exists)
        uint floor = GameManager.Instance.floor;

        // Adjust movement speed based on floor height — makes controls more responsive as the tower gets taller
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
