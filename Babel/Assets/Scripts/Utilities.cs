using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    private static MonoBehaviour coroutineRunner;

    /// <summary>
    /// Initalizes the script called in manager
    /// Lets Utilites borrow the coroutine of the game manager
    /// </summary>
    /// <param name="runner">The game object that it will borrow the coroutine of</param>
    public static void Initialize(MonoBehaviour runner)
    {
        coroutineRunner = runner;
    }

    /// <summary>
    /// Flashes in a sprite renderer that is already faded out
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="duration"></param>
    public static void Flash(SpriteRenderer renderer, float duration)
    {
        if (coroutineRunner != null)
            coroutineRunner.StartCoroutine(FadeSpriteAlpha(renderer, duration));
        else
            Debug.LogWarning("FlashUtility: Initialize with a MonoBehaviour before using Flash.");
    }

    public static void Flash(Image image, float duration)
    {
        if (coroutineRunner != null)
            coroutineRunner.StartCoroutine(FadeImageAlpha(image, duration));
        else
            Debug.LogWarning("FlashUtility: Initialize with a MonoBehaviour before using Flash.");
    }

    private static IEnumerator FadeSpriteAlpha(SpriteRenderer renderer, float duration)
    {
        Color original = renderer.color;
        Color transparent = new Color(original.r, original.g, original.b, 0f);
        Color visible = new Color(original.r, original.g, original.b, 1f);

        renderer.color = transparent;

        float half = duration / 2f;
        float t = 0f;

        // Fade In
        while (t < half)
        {
            float alphaT = t / half;
            renderer.color = Color.Lerp(transparent, visible, alphaT);
            t += Time.deltaTime;
            yield return null;
        }

        renderer.color = visible;
        t = 0f;

        // Fade Out
        while (t < half)
        {
            float alphaT = t / half;
            renderer.color = Color.Lerp(visible, transparent, alphaT);
            t += Time.deltaTime;
            yield return null;
        }

        renderer.color = original; // restore original alpha
    }

    private static IEnumerator FadeImageAlpha(Image image, float duration)
    {
        Color original = image.color;
        Color transparent = new Color(original.r, original.g, original.b, 0f);
        Color visible = new Color(original.r, original.g, original.b, 1f);

        image.color = transparent;

        float half = duration / 2f;
        float t = 0f;

        // Fade In
        while (t < half)
        {
            float alphaT = t / half;
            image.color = Color.Lerp(transparent, visible, alphaT);
            t += Time.deltaTime;
            yield return null;
        }

        image.color = visible;
        t = 0f;

        // Fade Out
        while (t < half)
        {
            float alphaT = t / half;
            image.color = Color.Lerp(visible, transparent, alphaT);
            t += Time.deltaTime;
            yield return null;
        }

        image.color = original; // restore original alpha
    }
}
