using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Handles upgrading floors
public class FloorManager : MonoBehaviour
{
    //Set up singleton
    public static FloorManager Instance { get; private set; }


    private void Awake()
    {
        //== Singleton setup ==
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //prevent duplicates
        }
        else
        {
            Instance = this;
        }
        //=================
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Checks if the player has enough money to upgrade
    //From floor information will have to find a way to figure out floor number then get it's values from the list
    /*
    public bool CheckUpgrade()
    {
        if (GameManager.money >= GameManager.floorObjects[Number of the floor once we have it])
        {
            return true;
        }
        else
        {
            TMPFadeWarning.Show(); // Display warning if not enough money
            return false;
        }
    }
    */
}
