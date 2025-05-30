using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorkersManager : MonoBehaviour
{
    //Worker count increase and they build every second for you
    uint workerCount = 0;
    uint workerCost = 30;

    //Engineer count is strictly for ui as a multiplyer goes into effect in Clicker.cs
    uint engineerCount = 0;
    uint engineerCost = 100;

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

    public static uint EngineerCount { get; private set; }  //used to access for the particle system in Clicker

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

        //Move and trigger hammer animation & starts particles
        if (workerCount > 0)
        {
            if (hammerAnimator != null)
            {
                hammerAnimator.SetBool("isHammering", true);
            }

            // Move the particles to match hammer and play
            if (clickParticles != null && !clickParticles.isPlaying)
            {
                clickParticles.transform.position = hammerAnimObject.transform.position;

                //Dynamically adjust particle emission based on engineer count
                var emission = clickParticles.emission;
                emission.rateOverTime = engineerCount * 2; // or tweak values

                //Dynamically adjust particle size
                var main = clickParticles.main;
                main.startSize = 0.1f + engineerCount * 0.01f;

                clickParticles.Play();
            }
        }

        //Uses the format numbers function in game manager to make the numbers format properly

        workerCostText.text = GameManager.FormatNumbers(workerCost);
        workerCountText.text = GameManager.FormatNumbers(workerCount);

        engineerCostText.text = GameManager.FormatNumbers(engineerCost);
        engineerCountText.text = GameManager.FormatNumbers(engineerCount);
    }

    //Starts the couroutine for wokers
    private IEnumerator Workers()
    {
        //Every secondincreases the count progress by total number of workers
        //Clicker.currentClickProgress += workerCount;
        float scaledWorkerOutput = workerCount * Clicker.multiplyer;     //Edited to now scale with engineer's multiplyer to make sure workers stay useful in late game
        Clicker.currentClickProgress += scaledWorkerOutput;
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
            workerCost += priceIncreaseWorkers;

            priceIncreaseWorkers += 5;
            IncreaseWorkers();
            UpdateWorkerBreakdown();    //update UI

            // === Spawn TinyGuy ===
            // Generate random X,Y offset to spread them around the top of the tower
            Vector3 spawnPosition = Clicker.Instance.NextBuildPosition + new Vector3(Random.Range(-xOffset, xOffset), Random.Range(-yOffset, 0), 0f);

            //Spawn TinyGuy on top of the tower
            GameObject tinyGuy = Instantiate(tinyGuyPrefab, spawnPosition, Quaternion.identity, tinyGuyParent);
            tinyGuys.Add(tinyGuy);

        }
        else
        {
            TMPFadeWarning.Show(); //Shows the text not enough to buy
        }
    }

    public void BuyEngineer()
    {
        if (GameManager.money >= engineerCost)
        {
            GameManager.money -= engineerCost;
            engineerCost += priceIncreaseEngineers;

            Clicker.IncreaseMultiplyer();
            GameManager.Instance.UpdateMultUI();    //format the multiplier to fix UI issues
            UpdateEngineerBreakdown();  //update UI
            engineerCount++;
            EngineerCount = engineerCount;  //used in Clicker
        }
        else
        {
            TMPFadeWarning.Show(); //Shows the text not enough to buy
        }
    }

    //Call this whenever a new worker is added
    private void UpdateWorkerBreakdown()
    {
        if (workerBreakdown != null)
            workerBreakdown.text = $"{workerCount}";
    }

    //Call this whenever the engineer multiplier changes
    private void UpdateEngineerBreakdown()
    {
        if (engineerBreakdown != null)
            engineerBreakdown.text = Clicker.multiplyer + "x";
    }
}
