using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HerecyManager : MonoBehaviour
{

    public static HerecyManager Instance;



    private int herecyAMin = 3;


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




    [Header("Configs")]
    [SerializeField]
    private bool canSpawnAngels = true;
    [SerializeField]
    private bool canHeresyAMin = true;
    bool spawnAngles = false;
    private bool isBlinking = false;



    //A static varialbe increased in GameManager AddFloor function
    //It is increased there so that it happens only once and doesn't cause the bug of multiple spawning when testing floor count
    public static int spawnNumber = 1;
    [Header("SpawnInfo")]
    [SerializeField]
    float spawnTime = 30f; //Time in between angle spawns

    //The time between each indidual angle in a group of angles spawning so they are not on top of each other
    [SerializeField]
    private float spawnDiff = 2f;


    [Header("Heresy")]
    [SerializeField]
    private int maxHeresy;


    /// <summary>
    /// Getter and setter for heresy
    /// Doesn't let it go over the max heresy value set in heresy manager
    /// </summary>
    public static int HeresyAMin
    {
        get { return Instance.herecyAMin; }
        set
        {
            Instance.herecyAMin = value;
            if (Instance.herecyAMin > Instance.maxHeresy) Instance.herecyAMin = Instance.maxHeresy;
        }
    }

    public bool CanSpawnAngels
    {
        set { canSpawnAngels = value; }
    }

    public bool CanHeresyAMin
    {
        set { canHeresyAMin = value;}
    }

    private void Awake()
    {
        Instance= this;
    }

    private void Start()
    {
        //Stops the couroutines on start to ensure no double heresy or double spawns
        StopAllCoroutines();

        //Starts both the loops for spawning angles and increasing herecy
        StartCoroutine(HerecyAMin());
        StartCoroutine(SpawnAngles());

    }

    private void Update()
    {
        CheckEnd();

        //We only spawn anagles if herecy is over 50
        if (GameManager.herecy >= 50)
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

    //Increases Heresy and 
    public static void ChangeHeresy(int amount)
    {
        //Increases the gameplay manager herecy
        if (amount < 0 && GameManager.herecy + amount < 0) GameManager.herecy = 0;
        else GameManager.herecy += (uint)amount;

        //Uses custom blinking text to make herecy fade in and display the increase
        if (Instance.blinkingText != null && amount > 0) Instance.blinkingText.ShowBlink("+" + GameManager.FormatNumbers(amount));
        else if(Instance.blinkingText != null) Instance.blinkingText.ShowBlink(GameManager.FormatNumbers(amount));
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


    /// <summary>
    /// Starts spawning angels and instantiating them on screen
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnAngles()
    {
        //If we even want angels to spawn
        if (canSpawnAngels)
        {

            //Checks for if heresy is over 50
            if (spawnAngles)
            {
                //Play Angel Theme
                AngleMovement.PlayMusicOnSpawn();
                Utilities.Flash(AngelsIncomingBox, 2.5f);
                AngelsIncomingText.ShowBlink("A herald approaches!");

                //Uses spawn number to loop and at the bottom figures out if it should wait a few extra seconds
                for (int i = 0; i < spawnNumber; i++)
                {
                    int attackFloor = Random.Range(0, FloorManager.floorObjects.Count - 4);
                    if (attackFloor < 0) attackFloor = 1;

                    // Decide spawn side
                    bool spawnRight = Random.value > 0.5f;
                    GameObject spawnPoint = spawnRight ? rightSpawn : leftSpawn;

                    GameObject newObj = Instantiate(AnglePrefab, spawnPoint.transform.position, Quaternion.identity);
                    newObj.GetComponent<AngleMovement>().SetTarget(FloorManager.floorObjects[attackFloor].transform, attackFloor, spawnRight);



                    yield return new WaitForSeconds(spawnDiff);
                }
            }
        }

         //Spawns after spawn time minus however many were spawned using the 2second interval in between
         yield return new WaitForSeconds(spawnTime - (spawnDiff * spawnNumber));
         StartCoroutine(SpawnAngles());
        
    }


    //Generates herecy every minute
    private IEnumerator HerecyAMin()
    {
        //Waits 60 seconds before doing it
        yield return new WaitForSeconds(60f);



        if (canHeresyAMin) ChangeHeresy(herecyAMin);

        StartCoroutine(HerecyAMin()); //this might be causing a bug
    }

    private void CheckEnd()
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
    }
}
