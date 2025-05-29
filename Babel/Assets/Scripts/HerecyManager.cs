using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerecyManager : MonoBehaviour
{
    bool spawnAngles = false;

    [SerializeField]
    float spawnTime = 30f; //Time in between angle spawns

    private void Start()
    {
        StartCoroutine(SpawnAngles());
    }

    private void Update()
    {
        //We only spawn anagles if herecy is over 50
        if(GameManager.herecy >= 50)
        {
            spawnAngles = true;
        }
        else
        {
            spawnAngles = false;
        }
    }

    private IEnumerator SpawnAngles()
    {
        if (spawnAngles)
        {
            int attackFloor = Random.Range(0, GameManager.floorObjects.Count); //Use this for position stuff
                                                                               //Spawn Angle Code
        }
        yield return new WaitForSeconds(spawnTime);
        StartCoroutine(SpawnAngles());
    }
}
