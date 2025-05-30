using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ShowArcherRadius : MonoBehaviour
{
    // Radius used to determine size of the circle visual
    private float detectionRadius = 0;

    // More segments means a smoother circle
    public int segments = 64;

    // Reference to LineRenderer component used to draw the circle
    private LineRenderer lineRenderer;

    // Public property to set and get the detection radius
    public float DetectionRadius
    {
        set { detectionRadius = value; }
        get { return detectionRadius; }
    }

    private void Awake()
    {
        // Get the LineRenderer component attached to this GameObject
        lineRenderer = GetComponent<LineRenderer>();

        // We want the circle to be relative to this object's transform, not world space
        lineRenderer.useWorldSpace = true;

        // Ensure the circle is closed by connecting the last point to the first
        lineRenderer.loop = true;

        // Set the number of points (segments) in the circle
        lineRenderer.positionCount = segments;

        // Set the width of the circle's line
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // Use a transparent default sprite shader material for the line
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Set the color of the line to semi-transparent green
        lineRenderer.startColor = new Color(0f, 1f, 0f, 0.5f);
        lineRenderer.endColor = new Color(0f, 1f, 0f, 0.5f);

        // Hide the circle initially
        lineRenderer.enabled = false;

        // Generate the initial circle points
        GenerateCircle();
    }

    // Generates a circular shape based on the detection radius and number of segments
    public void GenerateCircle()
    {
        Vector3[] points = new Vector3[segments];

        for(int i = 0; i < segments; i++)
{
            float angle = ((float)i / segments) * 2f * Mathf.PI;
            float x = Mathf.Cos(angle) * detectionRadius;
            float y = Mathf.Sin(angle) * detectionRadius;

            // Center the circle around this object's world position
            points[i] = transform.position + new Vector3(x, y, 0);
        }



        lineRenderer.SetPositions(points);
    }


    // Enables the LineRenderer to make the circle visible
    public void ShowRadius()
    {
        lineRenderer.enabled = true;
    }

    // Disables the LineRenderer to hide the circle
    public void HideRadius()
    {
        lineRenderer.enabled = false;
    }
}
