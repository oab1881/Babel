using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverRestart : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float typingDelay = 0.04f;

    private string originalText; //Will store the randomized defeat message
    private string hoverText = "You Lose";

    private void OnEnable()
    {
        if (scoreText != null)
        {
            string scoreLine = $"Score: {GameManager.Instance.finalScore} floors";
            StartCoroutine(TypeLine(scoreText, scoreLine));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText == null) return;

        //Cache the original defeat message on first hover
        if (string.IsNullOrEmpty(originalText))
            originalText = buttonText.text;

        buttonText.text = hoverText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null && !string.IsNullOrEmpty(originalText))
            buttonText.text = originalText; //set it back to original
    }

    private IEnumerator TypeLine(TextMeshProUGUI targetText, string line)
    {
        targetText.text = "";
        for (int i = 0; i < line.Length; i++)
        {
            targetText.text += line[i];
            yield return new WaitForSeconds(typingDelay);
        }
    }
}