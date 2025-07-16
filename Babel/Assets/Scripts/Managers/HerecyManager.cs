using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    Image AngelsIncomingBox;

    [SerializeField]
    TMPFloatingTextBlink blinkingText;

    [SerializeField]
    GameObject AnglePrefab;

    [SerializeField]
    GameObject leftSpawn;

    [SerializeField]
    GameObject rightSpawn;

    [SerializeField]
    GameObject heresyBar;

    private Coroutine blinkCoroutine;
    private bool isBlinking = false;


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
        StopAllCoroutines();
        //Starts both the loops for spawning angles and increasing herecy
        StartCoroutine(HerecyAMin());
        StartCoroutine(SpawnAngles());

    }

    private void Update()
    {
        
        //end game if heresy hits 100
        if (GameManager.herecy >= 100)
        {
            // Find the top floor and enable its Lightning child
            FloorInformation[] allFloors = FindObjectsOfType<FloorInformation>();
            if (allFloors.Length > 0)
            {
                // Sort from top to bottom
                System.Array.Sort(allFloors, (a, b) => b.floorNum.CompareTo(a.floorNum));
                Transform lightning = allFloors[0].transform.Find("Lightning");
                if (lightning != null)
                {
                    lightning.gameObject.SetActive(true);
                    AudioManager.PlaySoundEffect("lightning", 14);
                }
                else
                {
                    Debug.LogWarning("No 'Lightning' child found on top floor.");
                }
            }

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

        // Heresy bar blinking logic
        if (GameManager.herecy > 85)
        {
            if (!isBlinking)
            {
                blinkCoroutine = StartCoroutine(BlinkHeresyBar());
                isBlinking = true;
            }
        }
        else
        {
            if (isBlinking)
            {
                StopCoroutine(blinkCoroutine);
                heresyBar.SetActive(true); //Ensure it's visible when done
                isBlinking = false;
            }
        }


    }

    //Method to make the heresy bar blink when you almost max out
    private IEnumerator BlinkHeresyBar()
    {
        while (true)
        {
            heresyBar.SetActive(!heresyBar.activeSelf);
            yield return new WaitForSeconds(0.1f); //Adjust blink speed here 
        }
    }


    private IEnumerator SpawnAngles()
    {
        if (spawnAngles)
        {
            //Play Angel Theme
            AngleMovement.PlayMusicOnSpawn();
            Utilities.Flash(AngelsIncomingBox, 2.5f);
            AngelsIncomingText.ShowBlink("A herald approaches!");

            //Uses spawn number to loop and at the bottom figures out if it should wait a few extra seconds
            for (int i = 0; i < spawnNumber; i++)
            {
                int attackFloor = Random.Range(0, FloorManager.floorObjects.Count);

                // Decide spawn side
                bool spawnRight = Random.value > 0.5f;
                GameObject spawnPoint = spawnRight ? rightSpawn : leftSpawn;

                GameObject newObj = Instantiate(AnglePrefab, spawnPoint.transform.position, Quaternion.identity);
                newObj.GetComponent<AngleMovement>().SetTarget(FloorManager.floorObjects[attackFloor].transform, attackFloor, spawnRight);



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
        if(blinkingText != null) blinkingText.ShowBlink("+"+GameManager.FormatNumbers(herecyAMin));

        //Waits 60 seconds before doing it again
        yield return new WaitForSeconds(60f);

        StartCoroutine(HerecyAMin()); //this might be causing a bug
    }
}
