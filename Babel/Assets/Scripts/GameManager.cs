using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public uint floor = 0;
    public uint money = 0;
    public uint piety = 0;

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

    public void AddGold(float amount)
    {
        money += (uint)amount;
    }
}
