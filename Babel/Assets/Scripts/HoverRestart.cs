using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverRestart : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI buttonText;

    private string originalText; //Will store the randomized defeat message
    private string hoverText = "You Lose";

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
}