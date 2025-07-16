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
    /// <param name="renderer">SpriteRenderer to fade in then out</param>
    /// <param name="duration">Time to flash in then flash out</param>
    public static void Flash(SpriteRenderer renderer, float duration)
    {
        //Checks to make sure there is a coroutine that can run
        if (coroutineRunner != null)
            coroutineRunner.StartCoroutine(FadeSpriteAlpha(renderer, duration));
        else
            Debug.LogWarning("FlashUtility: Initialize with a MonoBehaviour before using Flash.");
    }

    /// <summary>
    /// Function that is called to initatiate the flash public
    /// </summary>
    /// <param name="image">UI image to flash in then out</param>
    /// <param name="duration">the time to flash in then out</param>
    public static void Flash(Image image, float duration)
    {
        //Checks to make sure there is a coroutine that can run
        if (coroutineRunner != null)
            coroutineRunner.StartCoroutine(FadeImageAlpha(image, duration));
        else
            Debug.LogWarning("FlashUtility: Initialize with a MonoBehaviour before using Flash.");
    }


    /// <summary>
    /// Coroutine that handles actually doing the fading
    /// </summary>
    /// <param name="renderer">The sprite renderer to fade in then out</param>
    /// <param name="duration">The time</param>
    /// <returns>Nothing</returns>
    private static IEnumerator FadeSpriteAlpha(SpriteRenderer renderer, float duration)
    {
        //Original color to get the alpha value
        Color original = renderer.color;
        //Sets the tranparent to be the original with a 0 alpha
        
        //Same thing but with alpah being 1
        Color visible = new Color(original.r, original.g, original.b, 1f);


        //Figures out what half the time is 
        //And starts time at 0
        float half = duration / 2f;
        float t = 0f;

        // Fade In
        //Does it til time is halfed
        while (t < half)
        {
            //Fades over the alpha of time in half
            //AlphaT increases over time
            float alphaT = t / half;

            //Sets the color to the lerp between the original and the visible which is the distance between the 2 values
            renderer.color = Color.Lerp(original, visible, alphaT);
            //Increases t by time
            t += Time.deltaTime;
            yield return null;
        }

        //Sets it to fully visible
        renderer.color = visible;
        //Resets t
        t = 0f;

        // Fade Out
        //Does it til time is halfed
        //Same as above with values switched
        while (t < half)
        {
            float alphaT = t / half;
            renderer.color = Color.Lerp(visible, original, alphaT);
            t += Time.deltaTime;
            yield return null;
        }

        //Sets our color back to orignial
        renderer.color = original; // restore original alpha
    }

    /// <summary>
    /// Fades UI image out using lerp
    /// </summary>
    /// <param name="image">Image to fade</param>
    /// <param name="duration">time to fade</param>
    /// <returns>Nothing</returns>
    private static IEnumerator FadeImageAlpha(Image image, float duration)
    {
        //Follows the exact same as sprite renderer function except with an image instead


        Color original = image.color;
        
        Color visible = new Color(original.r, original.g, original.b, 1f);


        float half = duration / 2f;
        float t = 0f;

        // Fade In
        while (t < half)
        {
            float alphaT = t / half;
            image.color = Color.Lerp(original, visible, alphaT);
            t += Time.deltaTime;
            yield return null;
        }

        image.color = visible;
        t = 0f;

        // Fade Out
        while (t < half)
        {
            float alphaT = t / half;
            image.color = Color.Lerp(visible, original, alphaT);
            t += Time.deltaTime;
            yield return null;
        }

        image.color = original; // restore original alpha
    }
}
