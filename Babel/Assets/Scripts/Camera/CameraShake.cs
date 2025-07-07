using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Duration of the shake in seconds
    public float defaultShakeDuration = 0.5f;

    // Strength of the shake
    public float defaultShakeMagnitude = 0.2f;

    // Static reference to this script
    public static CameraShake Instance;

    // Original position of the camera before shaking
    private Vector3 originalPos;

    void Awake()
    {
        // Store reference for static access
        Instance = this;

        // Save the original local position of the camera
        //originalPos = transform.localPosition;
    }

    /// <summary>
    /// Static function to trigger camera shake from anywhere.
    /// </summary>
    /// <param name="duration">How long the shake should last</param>
    /// <param name="magnitude">How strong the shake is</param>
    public static void Shake(float duration, float magnitude)
    {
        if (Instance != null)
        {
            Instance.StopAllCoroutines(); // Stop ongoing shake if any
            Instance.StartCoroutine(Instance.ShakeCoroutine(duration, magnitude));
        }
        else
        {
            Debug.LogWarning("CameraShake instance not found in the scene.");
        }
    }

    /// <summary>
    /// Triggers camera shake using default settings.
    /// </summary>
    public static void Shake()
    {
        Shake(Instance.defaultShakeDuration, Instance.defaultShakeMagnitude);
    }

    /// <summary>
    /// Coroutine that performs the actual shaking effect.
    /// </summary>
    private System.Collections.IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        originalPos = transform.position;
        while (elapsed < duration)
        {
            // Generate random x and y offset
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            // Apply the offset
            transform.localPosition = originalPos +  new Vector3(offsetX, offsetY, 0f);

            // Wait for next frame
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
       
    }

    public static void StopShake()
    {
        Instance.StopAllCoroutines();
    }
}
