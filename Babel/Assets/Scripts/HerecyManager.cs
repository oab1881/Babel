using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerecyManager : MonoBehaviour
{
    bool spawnAngles = false;

    [SerializeField]
    float spawnTime = 30f; //Time in between angle spawns

    public static int herecyAMin = 3;

    [SerializeField]
    TMPFloatingTextBlink blinkingText;

    private void Start()
    {
        StartCoroutine(HerecyAMin());
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

    private IEnumerator HerecyAMin()
    {
        GameManager.herecy += (uint)herecyAMin;
        blinkingText.ShowBlink("+"+GameManager.FormatNumbers(herecyAMin));
        yield return new WaitForSeconds(60f);

        StartCoroutine(HerecyAMin());
    }
}
