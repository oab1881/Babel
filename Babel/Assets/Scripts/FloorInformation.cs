using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorInformation : MonoBehaviour
{
    uint health;
    uint level = 1;


    public void CreateFloor(uint health)
    {
        this.health = health;
    }
}
