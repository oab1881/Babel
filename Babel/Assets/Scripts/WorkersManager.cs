using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkersManager : MonoBehaviour
{
    //Worker count increase and they build every second for you
    uint workerCount = 0;

    // Update is called once per frame
    void Update()
    {
        //We see if we should increase our workers or our multiplyer
        //This is temperaroy until we get buttons in then the buttons can call respective functions
        if (Input.GetKeyDown(KeyCode.V))
        {
            workerCount++;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Clicker.multiplyer++;
        }

        //We start the coroutine to generate clicks from workers
        StartCoroutine(Workers());
    }

    //Starts the couroutine for wokers
    private IEnumerator Workers()
    {
        //Every secondincreases the count progress by total number of workers
        Clicker.currentClickProgress += workerCount;
        //Then wait a second
        yield return new WaitForSeconds(1f);
    }


    //Increases the worker counts a function to be used later by the buttons.
    //Will need to incorporate a way to check for enough money
    //Also need to implement a fail vs success outcome
    public void IncreaseWorkers()
    {
        workerCount++;
    }
}
