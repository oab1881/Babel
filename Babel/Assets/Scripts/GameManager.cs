using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public uint floor = 0;
    public  static uint money = 0;
    public uint piety = 0;

    [SerializeField]
    private TextMeshProUGUI goldDisplay;

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
    }

    public void NewFloor()
    {
        floor++;
        money++;
        piety++;
    }

    //Method that increments gold and calls UpdateGoldUI
    public void AddGold(float amount)
    {
        money += (uint)amount;
        UpdateGoldUI();
    }

    //Method that updates the gold count in game
    private void UpdateGoldUI()
    {
        if (goldDisplay != null)
        {
            goldDisplay.text = FormatNumbers(money);
        }
    }
        

     //Formats currency using suffixes
     public static string FormatNumbers(uint curr)
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
 }


