using TMPro;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class TMPFloatingTextBlink : MonoBehaviour
{
    [SerializeField] private float fadeInDuration = 0.15f;
    [SerializeField] private float displayDuration = 0.4f;
    [SerializeField] private float fadeOutDuration = 0.3f;

    private TMP_Text tmpText;
    private Coroutine currentRoutine;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        SetAlpha(0f);
    }

    public void ShowBlink(string message)
    {
        tmpText.text = message;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(BlinkSequence());
    }

    private IEnumerator BlinkSequence()
    {
        yield return FadeTo(1f, fadeInDuration);
        yield return new WaitForSeconds(displayDuration);
        yield return FadeTo(0f, fadeOutDuration);
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = tmpText.color.a;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color c = tmpText.color;
        c.a = alpha;
        tmpText.color = c;
    }
}
