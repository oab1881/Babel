using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkipTutorialHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI buttonText;

    private string defaultText = "Skip Tutorial";
    private string hoverText = "Disgrace his Majesty";

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.text = hoverText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
            buttonText.text = defaultText;
    }
}
