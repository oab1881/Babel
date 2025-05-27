using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//THis script is attached to each floor prefab to track gold generation
public class GoldGenerator : MonoBehaviour
{
    public float goldPerSecond = 10f;
    public GameObject coinPopupPrefab;
    [SerializeField]
    //Canvas uiCanvas; // Drag your Canvas into this field

    private void OnEnable()
    {
        StartCoroutine(GenerateGold());
    }

    //Each tower piece will generate 10 gold per 5 seconds
    private IEnumerator GenerateGold()
    {
        while (true)
        {
            GameManager.Instance.AddGold(goldPerSecond);
            Debug.Log("Gold generated. Current gold: " + GameManager.Instance.money);

            //Instantiate popup (uses coinPopup script)
            if (coinPopupPrefab != null)
            {
                Vector3 popupPosition = transform.position + new Vector3(2.0f, 0.0f, 0);
                GameObject popup = Instantiate(coinPopupPrefab, popupPosition, Quaternion.identity);
                popup.transform.localScale = Vector3.one; // Force proper scale
                //popup.transform.SetParent(uiCanvas.transform, false); *BUGGED************************************

                TextMeshPro text = popup.GetComponentInChildren<TextMeshPro>();
                if (text != null)
                {
                    text.text = $"+{goldPerSecond}";
                }
            }

            yield return new WaitForSeconds(5f);
        }
    }
}

