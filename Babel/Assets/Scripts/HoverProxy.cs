using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HoverProxy : MonoBehaviour
{
    private LibraryLerp libraryLerp;

    private void Awake()
    {
        libraryLerp = GetComponentInParent<LibraryLerp>();
        if (libraryLerp == null)
            Debug.LogWarning($"HoverProxy on '{name}' couldn't find a LibraryLerp in parents.");
    }

    private void OnMouseEnter()
    {
        libraryLerp?.NotifyPointerEnter();
    }

    private void OnMouseExit()
    {
        libraryLerp?.NotifyPointerExit();
    }
}
