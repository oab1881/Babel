using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorkersManager : MonoBehaviour
{
    //Worker count increase and they build every second for you
    uint workerCost = 30;

    //Engineer count is strictly for ui as a multiplyer goes into effect in Clicker.cs
    uint engineerCost = 60;

    [SerializeField]
    Stats statsScript;

    public static bool canBuy = true;

    //Hover boxes logic for worker/engineer breakdown
    [SerializeField]
    GameObject workerBox;
    [SerializeField]
    GameObject workerBtn;
    [SerializeField]
    TMP_Text workerBreakdown;
    [SerializeField]
    GameObject engineerBox;
    [SerializeField]
    GameObject engineerBtn;
    [SerializeField]
    TMP_Text engineerBreakdown;

    [SerializeField]
    uint priceIncreaseEngineers = 100;

    [SerializeField]
    uint priceIncreaseWorkers = 10;

    [Header("Click Particles")]
    public ParticleSystem clickParticles; // Assigned in inspector

    //Workaround to make the hammering animation play when builders are building
    [Header("Hammer Animation")]
    public GameObject hammerAnimObject; //Assigned in inspector
    public Animator hammerAnimator;
    public float hammerTimeout = 0.15f; //Time window to keep hammering after last click

    [Header("Tiny Guy Settings")]
    [SerializeField] GameObject tinyGuyPrefab;
    [SerializeField] Transform tinyGuyParent; //Attach the hammer and anvil in inspector
    [SerializeField] float xOffset;
    [SerializeField] float yOffset;

    List<GameObject> tinyGuys = new List<GameObject>();



    //For all the texts on the worker and engineer ui
    [SerializeField]
    TMP_Text workerCostText;

    [SerializeField]
    TMP_Text workerCountText;


    [SerializeField]
    TMP_Text engineerCostText;

    [SerializeField]
    TMP_Text engineerCountText;

    private static WorkersManager Instance;

    public static uint WorkersCount
    {
        get { return (uint)Instance.statsScript.WorkerCount.Val; }
    }

    public static uint EngineersCount
    {
        get { return (uint)Instance.statsScript.EngineerCount.Val; }
    }

    private void Awake()
    {
        Instance= this;
    }

    private void Start()
    {
        //We start the coroutine once so it starts
        StartCoroutine(Workers());
    }


    // Update is called once per frame
    void Update()
    {
        
        //We start the coroutine to generate clicks from workers

        //Move and trigger hammer animation & starts particles
        if (WorkersCount > 0)
        {
            if (hammerAnimator != null)
            {
                hammerAnimator.SetBool("isHammering", true);
            }

            // Move the particles to match hammer and play
            if (clickParticles != null && !clickParticles.isPlaying)
            {
                clickParticles.Play();
            }
        }

        //Uses the format numbers function in game manager to make the numbers format properly

        workerCostText.text = GameManager.FormatNumbers(workerCost);
        workerCountText.text = GameManager.FormatNumbers(WorkersCount);

        engineerCostText.text = GameManager.FormatNumbers(engineerCost);
        engineerCountText.text = GameManager.FormatNumbers(EngineersCount);
    }

    //Starts the couroutine for wokers
    private IEnumerator Workers()
    {
        //Every secondincreases the count progress by total number of workers
        //Clicker.currentClickProgress += workerCount;
        float scaledWorkerOutput = WorkersCount * Clicker.multiplyer;     //Edited to now scale with engineer's multiplyer to make sure workers stay useful in late game
        Clicker.currentClickProgress += scaledWorkerOutput;
        //Then wait a second
        yield return new WaitForSeconds(1f);

        //Once done do the couroutine again
        StartCoroutine(Workers());
    }


    //Increases the worker counts a function used by the buttons.
    //Will need to incorporate a way to check for enough money
    //Also need to implement a fail vs success outcome
    private void IncreaseWorkers()
    {
        statsScript.WorkerCount.Add(1);
        AudioManager.PlaySoundEffect("Upgrade2", 6);
    }


    public void BuyWorker()
    {
        if (canBuy)
        {
            if (GameManager.Money >= workerCost)
            {
                GameManager.IncreaseGold(-workerCost);
                workerCost += priceIncreaseWorkers;

                priceIncreaseWorkers += 5;
                IncreaseWorkers();
                UpdateWorkerBreakdown();    //update UI

                // === Spawn TinyGuy ===
                // Generate random X,Y offset to spread them around the top of the tower
                Vector3 spawnPosition = Clicker.Instance.NextBuildPosition + new Vector3(Random.Range(-xOffset, xOffset), Random.Range(-yOffset, -0.2f), 0f); ;

                //Spawn TinyGuy on top of the tower
                GameObject tinyGuy = Instantiate(tinyGuyPrefab, spawnPosition, Quaternion.identity, tinyGuyParent);
                tinyGuys.Add(tinyGuy);

            }
            else
            {
                TMPFadeWarning.Show(); //Shows the text not enough to buy
            }
        }
    }

    //Attached to the engineer button's onClick event
    public void BuyEngineer()
    {
        if (canBuy)
        {
            if (GameManager.Money >= engineerCost)
            {
                GameManager.IncreaseGold(-engineerCost); ;
                engineerCost += priceIncreaseEngineers;
                Clicker.IncreaseMultiplyer();
                UpdateEngineerBreakdown();  //update UI
                statsScript.EngineerCount.Add(1);
                AudioManager.PlaySoundEffect("Upgrade2", 6);
            }
            else
            {
                TMPFadeWarning.Show(); //Shows the text not enough to buy
            }
        }
    }

    //Call this whenever a new worker is added
    private void UpdateWorkerBreakdown()
    {
        if (workerBreakdown != null)
            workerBreakdown.text = $"{WorkersCount}";
    }

    //Call this whenever the engineer multiplier changes
    private void UpdateEngineerBreakdown()
    {
        if (engineerBreakdown != null)
            engineerBreakdown.text = GameManager.FormatNumbers((int)Clicker.multiplyer) + "x";  
    }
}
