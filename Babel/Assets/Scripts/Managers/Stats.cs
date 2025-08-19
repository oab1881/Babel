using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stats : MonoBehaviour
{
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
    [SerializeField]
    private TextMeshProUGUI populationField;
    [SerializeField]
    private TextMeshProUGUI floorsField; 
    [SerializeField]
    private TextMeshProUGUI taxesField;
    [SerializeField]
    private TextMeshProUGUI heresyPerMinField;




    private void Awake()
    {
        money = new StatsVal(0, false);    
        heresy = new StatsVal(0, false, 100);
        engineerCount = new StatsVal(0, false);
        workerCount= new StatsVal(0, false);
        floorCount = new StatsVal(0, false);
        taxes = new StatsVal(0, false);
        population = new StatsVal(0, false);
        heresyPerMin = new StatsVal(0, false);
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
