using UnityEngine;

public class LibraryLerp : MonoBehaviour
{
    [SerializeField] private Transform hoverBox;         // the visual you move (child)
    [SerializeField] private float hoverOffsetY = 0.2f;  // local units up
    [SerializeField] private float smoothTime = 0.1f;    // SmoothDamp time (lower = snappier)

    private Vector3 startPos;
    private Vector3 targetPos;
    private Vector3 velocity = Vector3.zero;
    private bool isHovering = false;

    private void Awake()
    {
        if (hoverBox == null)
            Debug.LogError("LibraryLerp: assign hoverBox (the visual transform) in the Inspector.");

        startPos = hoverBox != null ? hoverBox.localPosition : Vector3.zero;
        targetPos = startPos;
    }

    private void Update()
    {
        if (hoverBox == null) return;

        // Smoothly move toward target every frame
        hoverBox.localPosition = Vector3.SmoothDamp(
            hoverBox.localPosition,
            targetPos,
            ref velocity,
            smoothTime
        );
    }

    // Called by HoverProxy when any registered collider gets an enter
    public void NotifyPointerEnter()
    {
        if (isHovering) return; // already up
        isHovering = true;
        velocity = Vector3.zero; // reset velocity for consistent easing
        targetPos = startPos + Vector3.up * hoverOffsetY;
        // Debug.Log("LibraryLerp: Enter -> lift");
    }

    // Called by HoverProxy when a collider exits — parent will only close
    // if the mouse is not currently over any child/managed collider.
    public void NotifyPointerExit()
    {
        if (!isHovering) return; // already down

        if (IsPointerOverAnyChildCollider())
        {
            // still over some part of the library/scroll; ignore this exit
            return;
        }

        // actually left
        isHovering = false;
        velocity = Vector3.zero;
        targetPos = startPos;
        // Debug.Log("LibraryLerp: Exit -> lower");
    }

    // Raycast/Overlap check to see if the mouse is currently over any collider
    // that is a child of this LibraryLerp's transform.
    private bool IsPointerOverAnyChildCollider()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        // 3D check
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.transform.IsChildOf(transform))
                return true;
        }

        // 2D check (useful if you are using BoxCollider2D etc.)
        Vector3 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 point2D = new Vector2(worldPoint.x, worldPoint.y);
        Collider2D col2d = Physics2D.OverlapPoint(point2D);
        if (col2d != null && col2d.transform.IsChildOf(transform))
            return true;

        return false;
    }
}
