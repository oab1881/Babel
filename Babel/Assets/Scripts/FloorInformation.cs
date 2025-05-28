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

    [SerializeField]
    GoldGenerator goldGeneratorScript;

    //Creates a floor by setting the floor health equal to the clicks it took to build the floor
    //This is done in clicker.cs on GameManager
    public void CreateFloor(uint health)
    {
        this.health = health;
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
        }
    }

    //When the mouse leaves we see if the upgrade button is active and set it to not be active
    private void OnMouseExit()
    {
        if (upgrageButton.activeInHierarchy)
        {
            upgrageButton.SetActive(false);
        }
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




}
