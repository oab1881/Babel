using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerecyManager : MonoBehaviour
{
    bool spawnAngles = false;

    [SerializeField]
    float spawnTime = 30f; //Time in between angle spawns

    public static int herecyAMin = 3;

    //Reference to the script that says angles incoming
    [SerializeField]
    TMPFloatingTextBlink AngelsIncomingText;

    [SerializeField]
    TMPFloatingTextBlink blinkingText;

    [SerializeField]
    GameObject AnglePrefab;

    [SerializeField]
    GameObject leftSpawn;

    [SerializeField]
    GameObject rightSpawn;

    public static HerecyManager Instance;

    //A static varialbe increased in GameManager AddFloor function
    //It is increased there so that it happens only once and doesn't cause the bug of multiple spawning when testing floor count
    public static int spawnNumber = 1;

    //The time between each indidual angle in a group of angles spawning so they are not on top of each other
    [SerializeField]
    private float spawnDiff = 2f;


    private void Awake()
    {
        Instance= this;
    }

    private void Start()
    {
        //Starts both the loops for spawning angles and increasing herecy
        StartCoroutine(HerecyAMin());
        StartCoroutine(SpawnAngles());
    }

    private void Update()
    {
        //end game if heresy hits 100
        if (GameManager.herecy >= 100)
        {
            FloorInformation.ExplodeEntireTower();
        }

        //We only spawn anagles if herecy is over 50
        else if (GameManager.herecy >= 50)
        {
            //Debug.Log(GameManager.herecy);
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
            //Play Angel Theme
            AngleMovement.PlayMusicOnSpawn();
            AngelsIncomingText.ShowBlink("A herald approaches!");

            //Uses spawn number to loop and at the bottom figures out if it should wait a few extra seconds
            for (int i = 0; i < spawnNumber; i++)
            {
                int attackFloor = Random.Range(0, GameManager.floorObjects.Count);

                // Decide spawn side
                bool spawnRight = Random.value > 0.5f;
                GameObject spawnPoint = spawnRight ? rightSpawn : leftSpawn;

                GameObject newObj = Instantiate(AnglePrefab, spawnPoint.transform.position, Quaternion.identity);
                newObj.GetComponent<AngleMovement>().SetTarget(GameManager.floorObjects[attackFloor].transform, attackFloor, spawnRight);



                yield return new WaitForSeconds(spawnDiff);
            }
        }

        //Spawns after spawn time minus however many were spawned using the 2second interval in between
        yield return new WaitForSeconds(spawnTime - (spawnDiff * spawnNumber));
        StartCoroutine(SpawnAngles());
    }


    //Generates herecy every minute
    private IEnumerator HerecyAMin()
    {
        //Increases the gameplay manager herecy
        GameManager.herecy += (uint)herecyAMin;
        
        //Uses custom blinking text to make herecy fade in and display the increase
        if(blinkingText != null)blinkingText.ShowBlink("+"+GameManager.FormatNumbers(herecyAMin));

        //Waits 60 seconds before doing it again
        yield return new WaitForSeconds(60f);

        StartCoroutine(HerecyAMin());
    }
}
