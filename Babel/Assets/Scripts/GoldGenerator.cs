using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//THis script is attached to each floor prefab to track gold generation
public class GoldGenerator : MonoBehaviour
{
    public float goldPerSecond = 10f;
    public GameObject coinPopupPrefab;
    GameObject uiCanvas;

    public float GoldPerSecond
    {
        set { goldPerSecond = value; }
        get { return goldPerSecond;}
    }

    private void OnEnable()
    {
        //Check if we have gotten a reference to our canvas
        if(uiCanvas == null) uiCanvas = GameObject.Find("UICanvas");
        StartCoroutine(GenerateGold());
    }

    //Each tower piece will generate 10 gold per 5 seconds
    private IEnumerator GenerateGold()
    {
        while (true)
        {
            GameManager.Instance.AddGold(goldPerSecond);

            //Debug.Log("Gold generated. Current gold: " + GameManager.Instance.money);

            //Instantiate popup (uses coinPopup script)
            if (coinPopupPrefab != null)
            {
                Vector3 popupPosition = transform.position + new Vector3(2.5f, 0.0f, 0);

                //We instantiate the coin and set parent to the uiCanvas
                GameObject popup = Instantiate(coinPopupPrefab, popupPosition, Quaternion.identity, uiCanvas.transform);
                popup.transform.localScale = Vector3.one; // Force proper scale
                

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

