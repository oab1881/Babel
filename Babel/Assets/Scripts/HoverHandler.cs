using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class HoverHandlerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject hoverBox;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverBox != null)
            hoverBox.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverBox != null)
            hoverBox.SetActive(false);
    }
}
