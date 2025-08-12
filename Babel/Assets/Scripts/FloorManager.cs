using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Handles upgrading floors
public class FloorManager : MonoBehaviour
{
    //Set up singleton
    public static FloorManager Instance { get; private set; }

    public static uint floor = 0;
    public static List<FloorInformation> floorObjects = new List<FloorInformation>();


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

        GameManager.Reset += Restart;
    }

    // Start is called before the first frame update
    void Start()
    {
        Clicker.NewFloor += NewFloor;
    }



    //Attached to the clicker event that recieves the signal a new floor was built
    public void NewFloor()
    {
        floor++;
        GameManager.IncreaseGold(100);
        HerecyManager.IncreaseHeresy(5);

        //Every 20 floors make the number that spawn in a group increase
        if (floorObjects.Count % 20 == 0)
        {
            HerecyManager.spawnNumber++;
        }
    }

    private void Restart()
    {
        UnSubscribe();
    }

    void UnSubscribe()
    {
        Clicker.NewFloor -= NewFloor;
        GameManager.Reset -= UnSubscribe;
    }
}
