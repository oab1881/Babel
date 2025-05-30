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

    [SerializeField]
    GameObject AnglePrefab;

    [SerializeField]
    GameObject leftSpawn;

    [SerializeField]
    GameObject rightSpawn;

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
            int attackFloor = Random.Range(0, GameManager.floorObjects.Count);

            // Decide spawn side
            bool spawnRight = Random.value > 0.5f;
            GameObject spawnPoint = spawnRight ? rightSpawn : leftSpawn;

            GameObject newObj = Instantiate(AnglePrefab, spawnPoint.transform.position, Quaternion.identity);
            newObj.GetComponent<AngleMovement>().SetTarget(GameManager.floorObjects[attackFloor].transform, attackFloor, spawnRight);
        }

        yield return new WaitForSeconds(spawnTime);
        StartCoroutine(SpawnAngles());
    }

    private IEnumerator HerecyAMin()
    {
        GameManager.herecy += (uint)herecyAMin;
        if(blinkingText != null)blinkingText.ShowBlink("+"+GameManager.FormatNumbers(herecyAMin));
        yield return new WaitForSeconds(60f);

        StartCoroutine(HerecyAMin());
    }
}
