using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;

public class Stats : MonoBehaviour
{
    public static Stats Instance;

    public static Action loop;

    //General Stats
    private StatsVal money;
    private StatsVal heresy;
    private StatsVal workerCount;
    private StatsVal engineerCount;

    //Census Stats
    private StatsVal population;
    private StatsVal taxes;
    private StatsVal floorCount;
    private StatsVal heresyPerMin;

    //Populace Stats
    public StatsVal dogma;
    public StatsVal culture;
    public StatsVal power;

    //Census stat text fields
    [Header("Census Text Fields")]
    [SerializeField]
    private TextMeshProUGUI populationField;
    [SerializeField]
    private TextMeshProUGUI floorsField; 
    [SerializeField]
    private TextMeshProUGUI taxesField;
    [SerializeField]
    private TextMeshProUGUI heresyPerMinField;

    //Populace sliders
    [Header("Populace Sliders")]
    [SerializeField] private Slider dogmaSlider;
    [SerializeField] private Slider cultureSlider;
    [SerializeField] private Slider powerSlider;




    private void Awake()
    {
        // --- Singleton setup ---
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;


        money = new StatsVal(0, false);    
        heresy = new StatsVal(0, false, 100);
        engineerCount = new StatsVal(0, false);
        workerCount= new StatsVal(0, false);
        floorCount = new StatsVal(0, false);
        taxes = new StatsVal(0, false);
        population = new StatsVal(0, false);
        heresyPerMin = new StatsVal(0, false);
    }

    private void Start()
    {
        //Setup ranges (-100 to 100, starting at 0 in the middle)
        if (dogmaSlider != null) {
        dogmaSlider.minValue = -100;
        dogmaSlider.maxValue = 100;
        dogmaSlider.value = 0; // start in the middle
        }

        if (cultureSlider != null) {
            cultureSlider.minValue = -100;
            cultureSlider.maxValue = 100;
            cultureSlider.value = 0;
        }

        if (powerSlider != null) {
            powerSlider.minValue = -100;
            powerSlider.maxValue = 100;
            powerSlider.value = 0;
        }
    }



    public StatsVal Money
    {
        get { return money; }
        set { money = value; }
    }

    public StatsVal Heresy
    {
        get { return heresy; }
        set { heresy = value; }
    }



    public StatsVal WorkerCount
    {
        get { return workerCount; }
        set { workerCount = value; }
    }
    public StatsVal EngineerCount
    {
        get { return engineerCount; }
        set { engineerCount = value; }
    }

    private void Update()
    {
        //Inovkes the loop event every frame each statVal will call it's checkMods function
        loop?.Invoke();


        //Count all current floors
        int currentFloors = FindObjectsOfType<FloorInformation>().Length;
        floorCount.Modify(currentFloors);

        //Calculate total taxes (sum of goldPerSecond across all floors)
        float totalTaxes = 0;
        foreach (GoldGenerator g in FindObjectsOfType<GoldGenerator>())
        {
            totalTaxes += g.GoldPerSecond;
        }
        taxes.Modify(totalTaxes);

        //Grab heresy per minute from HerecyManager
        if (HerecyManager.Instance != null)
            heresyPerMin.Modify(HerecyManager.HeresyAMin);


        /// ---- UPDATE STATS HERE ----
        //Update Floor text
        if (floorsField != null)
            floorsField.text = $"{GameManager.FormatNumbers(floorCount.Val)}";

        //Update tax text here
        if (taxesField != null)
            taxesField.text = $"{GameManager.FormatNumbers(taxes.Val)}/sec";

        //Update heresyPerMin
        if (heresyPerMinField != null)
            heresyPerMinField.text = $"{GameManager.FormatNumbers(heresyPerMin.Val)}/min";
    }

    // --- Increment Methods for Populace Stats ---
    public void IncrementDogma(int amount) => ModifyStat(dogma, dogmaSlider, amount);
    public void IncrementCulture(int amount) => ModifyStat(culture, cultureSlider, amount);
    public void IncrementPower(int amount) => ModifyStat(power, powerSlider, amount);

    //Method that allows us to modify the populace stats specifically
    private void ModifyStat(StatsVal stat, Slider slider, int amount)
    {
        if (stat == null || slider == null) return;

        //Add first
        stat.Add(amount);

        //Clamp result manually between -100 and 100
        float clampedVal = Mathf.Clamp(stat.Val, -100, 100);
        stat.Modify(clampedVal);

        //Sync to UI
        slider.value = clampedVal;
    }


    public void ResetStats()
    {
        money.Modify(0);
        heresy.Modify(0);
        workerCount.Modify(0);
        engineerCount.Modify(0);
        dogma.Modify(0);
        floorCount.Modify(0);
        taxes.Modify(0);
        population.Modify(0);
        heresyPerMin.Modify(0);
    }
}
