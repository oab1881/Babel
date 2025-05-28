using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorkersManager : MonoBehaviour
{
    //Worker count increase and they build every second for you
    uint workerCount = 0;
    uint workerCost = 100;

    //Engineer count is strictly for ui as a multiplyer goes into effect in Clicker.cs
    uint engineerCount = 0;
    uint engineerCost = 100;

    [SerializeField]
    uint priceIncrease = 100;

    //Workaround to make the hammering animation play when builders are building
    [Header("Hammer Animation")]
    public GameObject hammerAnimObject; //Assigned in inspector
    public Animator hammerAnimator;
    public float hammerTimeout = 0.15f; //Time window to keep hammering after last click


    //For all the texts on the worker and engineer ui
    [SerializeField]
    TMP_Text workerCostText;

    [SerializeField]
    TMP_Text workerCountText;


    [SerializeField]
    TMP_Text engineerCostText;

    [SerializeField]
    TMP_Text engineerCountText;

    private void Start()
    {
        //We start the coroutine once so it starts
        StartCoroutine(Workers());
    }

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

        //Move and trigger hammer animation
        if (hammerAnimator != null)
        {
            hammerAnimator.SetBool("isHammering", true);
        }

        workerCostText.text = workerCost.ToString();
        workerCountText.text = workerCount.ToString();

        engineerCostText.text = engineerCost.ToString();
        engineerCountText.text = engineerCount.ToString();
    }

    //Starts the couroutine for wokers
    private IEnumerator Workers()
    {
        //Every secondincreases the count progress by total number of workers
        Clicker.currentClickProgress += workerCount;
        //Then wait a second
        yield return new WaitForSeconds(1f);

        //Once done do the couroutine again
        StartCoroutine(Workers());
    }


    //Increases the worker counts a function to be used later by the buttons.
    //Will need to incorporate a way to check for enough money
    //Also need to implement a fail vs success outcome
    private void IncreaseWorkers()
    {
        workerCount++;
    }


    public void BuyWorker()
    {
        if(GameManager.money >= workerCost)
        {
            GameManager.money -= workerCost;
            workerCost += priceIncrease;
            IncreaseWorkers();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    public void BuyEngineer()
    {
        if (GameManager.money >= engineerCost)
        {
            GameManager.money -= engineerCost;
            engineerCost += priceIncrease;
            Clicker.IncreaseMultiplyer();
            engineerCount++;
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }
}
