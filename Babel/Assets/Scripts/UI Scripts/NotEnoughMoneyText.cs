using TMPro;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class TMPFadeWarning : MonoBehaviour
{
    public static TMPFadeWarning Instance { get; private set; }

    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float displayDuration = 1.0f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private TMP_Text tmpText;
    private Coroutine currentRoutine;
    private bool isShowing = false;


    private void Awake()
    {
        Instance = this;
        tmpText = GetComponent<TMP_Text>();
        SetAlpha(0f);
    }

    public static void Show()
    {
        if (Instance != null && !Instance.isShowing)
            Instance.ShowWarning();
        AudioManager.PlaySoundEffect("NotEnoughGold", 8);
    }

    private void ShowWarning()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        isShowing = true;

        yield return FadeTo(1f, fadeInDuration);
        yield return new WaitForSeconds(displayDuration);
        yield return FadeTo(0f, fadeOutDuration);

        isShowing = false;
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = tmpText.color.a;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            SetAlpha(newAlpha);
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
