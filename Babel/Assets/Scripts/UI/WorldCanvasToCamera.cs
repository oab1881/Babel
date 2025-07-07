using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvasToCamera : MonoBehaviour
{
    // The camera we want the canvas to match (defaults to main camera if not set)
    public Camera targetCamera;

    void Start()
    {
        // If no camera is assigned in the inspector, use the main camera
        if (targetCamera == null)
            targetCamera = Camera.main;

        // Perform the fitting on start
        FitToScreen();
    }

    void FitToScreen()
    {
        // Get the Canvas component on this GameObject
        Canvas canvas = GetComponent<Canvas>();

        // Ensure the canvas is in World Space mode
        if (canvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogWarning("Canvas must be set to World Space.");
            return;
        }

        // Get the RectTransform of the Canvas to resize it
        RectTransform rt = canvas.GetComponent<RectTransform>();

        // Calculate the distance between the camera and the canvas in world units
        float distance = Vector3.Distance(targetCamera.transform.position, transform.position);

        // Calculate the height of the view frustum at that distance
        float screenHeight = 2f * distance * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

        // Calculate the width based on camera aspect ratio
        float screenWidth = screenHeight * targetCamera.aspect;

        // Set the size of the canvas RectTransform to match the screen size in world units
        rt.sizeDelta = new Vector2(screenWidth, screenHeight);
    }
}
