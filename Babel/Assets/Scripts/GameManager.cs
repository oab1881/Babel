using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public uint floor = 0;
    public uint money = 0;
    public uint piety = 0;

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
}
