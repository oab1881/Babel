using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorInformation : MonoBehaviour
{
    //Reference to health and current level of this floor
    uint health;
    uint level = 1;
    uint upgradeCost = 50;

    //Reference to plus button which will upgrade the tower
    [SerializeField]
    GameObject upgrageButton;

    //Reference to Highlight outline
    [SerializeField]
    GameObject towerHighlight;

    //Reference to Price Panel and Lerp Info
    [SerializeField]
    GameObject upgradePanel;
    [SerializeField] float moveDistance = 5f; //How far to move left
    [SerializeField] float lerpSpeed = 5f;
    private Vector3 panelStartPos;
    private Vector3 panelTargetPos;
    private Coroutine moveCoroutine;

    [SerializeField]
    GoldGenerator goldGeneratorScript;

    //Creates a floor by setting the floor health equal to the clicks it took to build the floor
    //This is done in clicker.cs on GameManager
    public void CreateFloor(uint health)
    {
        this.health = health;
    }

    void Start()
    {
        panelStartPos = upgradePanel.transform.localPosition;
        panelTargetPos = panelStartPos + Vector3.left * moveDistance;
    }


    private void Update()
    {
        if (level >= 3)
        {
            upgrageButton.SetActive(false);
        }
    }

    // Commented out til we can fix the upgrade button to display properly

    //When the mouse enters this object we display the upgrade button
    private void OnMouseEnter()
    {
        //Only shows the upgrade button if the level of the floor is below 
        if (level < 3)
        {
            upgrageButton.SetActive(true);
            towerHighlight.SetActive(true);
        }

        //Move panel to the left
        StartPanelLerp(panelTargetPos);
    }

    //When the mouse leaves we see if the upgrade button is active and set it to not be active
    private void OnMouseExit()
    {
        if (upgrageButton.activeInHierarchy)
        {
            upgrageButton.SetActive(false);
            towerHighlight.SetActive(false);
        }

        //Move panel back to the right
        StartPanelLerp(panelStartPos);
    }

    public void CheckUpgrade()
    {
        if(GameManager.money >= upgradeCost)
        {
            level++;
            //Make the prefab switch styles
            goldGeneratorScript.GoldPerSecond += 10;
            GameManager.money -= upgradeCost;
            upgradeCost += 50;
        }
        else
        {
            Debug.Log("Cant upgrade not enough money");
        }
    }

    //Used to move the panel from behind the tower to display upgrade price
    void StartPanelLerp(Vector3 targetPos)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(LerpPanel(targetPos));
    }

    IEnumerator LerpPanel(Vector3 targetPos)
    {
        Vector3 startPos = upgradePanel.transform.localPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * lerpSpeed;
            upgradePanel.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        upgradePanel.transform.localPosition = targetPos; //Snap exactly at end
    }




}
