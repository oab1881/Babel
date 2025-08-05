using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//This script replaces Unity's buggy sprite swap feature (fixed a bug with the hovering shutting off after pressing a button)

public class HoverButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public Image targetImage;
    public Sprite normalSprite;
    public Sprite hoverSprite;

    private bool isHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetImage.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetImage.sprite = normalSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //If we're still hovering, restore the hover sprite
        if (isHovering)
        {
            targetImage.sprite = hoverSprite;
        }
        else
        {
            targetImage.sprite = normalSprite;
        }
    }
}
