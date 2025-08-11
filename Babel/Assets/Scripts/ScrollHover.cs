using System.Collections;
using UnityEngine;

public class ScrollHover : MonoBehaviour
{
    [Header("Hover Animation")]
    [SerializeField] private Animator buttonAnimator; // Animator for the scroll button hover
    [SerializeField] private string hoverTrigger = "isHovering";
    [SerializeField] private string stopHoverTrigger = "stoHovering";

    [Header("Big Scroll Movement & Animation")]
    [SerializeField] private Transform bigScroll;             // The big scroll object
    [SerializeField] private Animator bigScrollAnimator;      // Animator for the big scroll
    [SerializeField] private string openTrigger = "OpenScroll";
    [SerializeField] private string closeTrigger = "CloseScroll";
    [SerializeField] private Vector3 bigScrollOffscreen;      // Local pos offscreen
    [SerializeField] private Vector3 bigScrollOnscreen;       // Local pos onscreen
    [SerializeField] private float lerpSpeed = 5f;            // How fast it moves
    [SerializeField] private float closeAnimationDuration = 1f; // Duration of closing animation in seconds

    private bool isBigScrollVisible = false;
    private bool isAnimating = false;  // lock flag
    private Coroutine moveCoroutine;

    private void OnMouseEnter()
    {
        if (buttonAnimator != null)
            buttonAnimator.SetTrigger(hoverTrigger);
    }

    private void OnMouseExit()
    {
        if (buttonAnimator != null)
            buttonAnimator.SetTrigger(stopHoverTrigger);
    }

    private void OnMouseDown()
    {
        if (isAnimating || bigScroll == null || bigScrollAnimator == null)
            return; // ignore clicks during animation

        if (!isBigScrollVisible)
        {
            StartCoroutine(LerpAndOpen());
        }
        else
        {
            StartCoroutine(CloseAndLerpDown());
        }
    }

    private IEnumerator LerpAndOpen()
    {
        isAnimating = true;

        yield return StartCoroutine(LerpBigScroll(bigScrollOnscreen));
        isBigScrollVisible = true;

        bigScrollAnimator.SetTrigger(openTrigger);

        // Wait a tiny frame to let animation start (optional)
        yield return null;

        isAnimating = false;
    }

    private IEnumerator CloseAndLerpDown()
    {
        isAnimating = true;

        bigScrollAnimator.SetTrigger(closeTrigger);

        yield return new WaitForSeconds(closeAnimationDuration);

        yield return StartCoroutine(LerpBigScroll(bigScrollOffscreen));
        isBigScrollVisible = false;

        isAnimating = false;
    }

    private IEnumerator LerpBigScroll(Vector3 targetLocalPos)
    {
        Vector3 startPos = bigScroll.localPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * lerpSpeed;
            if (t > 1f) t = 1f;

            bigScroll.localPosition = Vector3.Lerp(startPos, targetLocalPos, t);
            yield return null;
        }

        bigScroll.localPosition = targetLocalPos;
        moveCoroutine = null;
    }
}
