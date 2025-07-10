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
    private float touchScrollVelocity = 0f;                       // Continuous scroll velocity from touch drag


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

        // Add momentum from both scroll wheel and touch
        float combinedScroll = scrollVelocity + touchScrollVelocity;
        move += Vector2.up * combinedScroll;

        // Apply movement to Rigidbody
        rb.velocity = move * currentSpeed * Time.fixedDeltaTime;

        // Decay both momentum sources
        scrollVelocity = Mathf.Lerp(scrollVelocity, 0, Time.fixedDeltaTime * scrollDecay);
        touchScrollVelocity = Mathf.Lerp(touchScrollVelocity, 0, Time.fixedDeltaTime * scrollDecay);
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
        if (Input.touchCount == 0)
        {
            isSwiping = false;
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStartPos = touch.position;
                isSwiping = true;
                break;

            case TouchPhase.Moved:
                if (!isSwiping) break;

                // Continuous scroll based on finger drag delta
                float verticalDelta = touch.deltaPosition.y / Screen.height;
                touchScrollVelocity += verticalDelta * scrollAdjuster;
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isSwiping = false;
                break;
        }
    }


    private void UpdateSpeed()
    {
        // Get current floor from GameManager (can’t compile unless GameManager.Instance.floor exists)
        uint floor = FloorManager.floor;

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
