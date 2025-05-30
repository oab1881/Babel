using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public uint floor = 0;
    public static List<FloorInformation> floorObjects = new List<FloorInformation>();


    public  static uint money = 0;
    public static uint herecy = 0;

    [SerializeField]
    private TextMeshProUGUI goldDisplay;

    [SerializeField]
    private TextMeshProUGUI engineerDisplay;

    [SerializeField]
    private TMPFloatingTextBlink blinkingHerecyIncreaseText;




    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //prevent duplicates
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Clicker.NewFloor += NewFloor;
        AudioManager.SetVolume(0, 0.1f);
        AudioManager.PlayMusic("BabelAmbient", 0);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) money += 10000;
    }

    //Attached to the clicker event that recieves the signal a new floor was built
    public void NewFloor()
    {
        floor++;
        money++;
        herecy+= 5;

        //if(blinkingHerecyIncreaseText != null)blinkingHerecyIncreaseText.ShowBlink("+5");
    }

    //Method that increments gold and calls UpdateGoldUI
    public void AddGold(float amount)
    {
        money += (uint)amount;
        UpdateGoldUI();
    }

    //Method that updates the gold count in game
     public void UpdateGoldUI()
    {
        if (goldDisplay != null)
        {
            goldDisplay.text = FormatNumbers(money);
        }
    }

    //Method that updates the multiplier count
    //Does not do anything
    public void UpdateMultUI()
    {
        if (engineerDisplay != null)
        {
            engineerDisplay.text = FormatNumbers((int)Clicker.multiplyer);
        }
    }


    //Formats currency using suffixes
    public static string FormatNumbers(float curr)
     {
        string[] suffixes = {
        "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No",
        "Dc", "Ud", "Dd", "Td", "Qad", "Qid", "Sxd", "Spd", "Ocd",
        "Nod", "Vg", "Uvg", "Dvg"
        };
        double value = (double)curr;
         if (value < 1000)
             return value.ToString("0");

         int suffixIndex = 0;
         while (value >= 1000 && suffixIndex < suffixes.Length - 1)
         {
             value /= 1000;
             suffixIndex++;
         }

         return value.ToString("0.##") + suffixes[suffixIndex];
     }
    

    public static void DecreaseHerecy(int amount)
    {
        if(amount > herecy) herecy = 0;
        
        else herecy -= (uint)amount;
    }


    //This uses function in FloorInfo.cs to reduce the health of a floor and is called in 
    //AngleMovement.cs
    public static void DecreaseFloorHealth(int index, int damageAmount)
    {
        floorObjects[index].DamageFloor(damageAmount);
    }
 }


