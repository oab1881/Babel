using System;
using UnityEngine;
using System.Collections.Generic;

public class Clicker : MonoBehaviour
{
    [Header("Floor Prefabs")]
    public List<GameObject> floorPrefabs; //Assign 5 prefabs in the Inspector

    [Header("Build Settings")]
    public float buildHeightOffset = 2.8f; //Distance between floors
    public Transform towerBase; //The base position where floors stack
    public int startingClickRequirement = 5;
    public float clickRequirementMultiplier = 1.5f;

    [Header("Hammer Animation")]
    public GameObject hammerAnimObject; //Assigned in inspector
    public Animator hammerAnimator;
    public float hammerTimeout = 0.15f; // Time window to keep hammering after last click
    private float lastClickTime = 0f;

    [Header("Click Particles")]
    public ParticleSystem clickParticles; // Assigned in inspector

    private int currentFloor = 0;
    private float currentClickRequirement;
    public static float currentClickProgress = 0f;
    private Vector3 nextBuildPosition;

    [SerializeField]
    GameObject topBorder; //For dynamically moving the top border

    public static event Action NewFloor; //Listener for when a new floor is created

    public static float multiplyer = 1;

    List<GameObject> floorsList = new List<GameObject>(); //List containing all built floors in the heirarchy

    public static Clicker Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public Vector3 NextBuildPosition => nextBuildPosition;
    public float BuildHeightOffset => buildHeightOffset;


    public float CurrentClickProgress
    {
        get
        {
            return currentClickProgress;
        }
    }

    public float ClickRequirement
    {
        get
        {
            return currentClickRequirement;
        }
    }

    void Start()
    {
        //Starting click requirement is 5 for now
        currentClickRequirement = startingClickRequirement;
        nextBuildPosition = towerBase.position;

        //Get hammer animator object
        if (hammerAnimObject != null)
            hammerAnimator = hammerAnimObject.GetComponent<Animator>();
    }

    private void Update()
    {
        //Check for click input
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            OnClickBuild();
        }

        CheckFloorStatus();

        //Logic for stopping hammering animation
        if (hammerAnimator != null && hammerAnimator.GetBool("isHammering"))
        {
            if (Time.time - lastClickTime > hammerTimeout)
            {
                hammerAnimator.SetBool("isHammering", false);
            }
        }

        //Checks if the next build pos is off screen, this will make it so top border scales dynamically

        if (nextBuildPosition.y > Camera.main.orthographicSize)
        {
            topBorder.transform.position = nextBuildPosition + new Vector3(0,(buildHeightOffset + 5),0);
        }
    }

    //Clicking anywhere on the screen calls this
    public void OnClickBuild()
    {
        // Pick a random FastClick index (0–3) to match your system
        //int randomClickIndex = UnityEngine.Random.Range(0, 4); // 0 to 3 inclusive
        //string sfxName = "FastClick" + (randomClickIndex + 1); // Names start at 1

        //AudioManager.PlaySoundEffect(sfxName, randomClickIndex);

        //Increases click progress by one times the multiplyer or just (the multiplyer)
        currentClickProgress +=  multiplyer;

        lastClickTime = Time.time;

        //Move and trigger hammer animation
        if (hammerAnimator != null)
        {
            hammerAnimObject.transform.position = nextBuildPosition;    //Move hammer to the build position
            hammerAnimator.SetBool("isHammering", true);
        }

        // Move the particles to match hammer and play
        if (clickParticles != null)
        {
            clickParticles.transform.position = hammerAnimObject.transform.position;

            //Dynamically adjust particle emission based on engineer count
            var emission = clickParticles.emission;
            emission.rateOverTime = WorkersManager.EngineerCount * 2; // or tweak values

            // Adjust and clamp particle size
            float engineerBasedSize = 0.1f + WorkersManager.EngineerCount * 0.01f;
            float maxParticleSize = 1.5f;
            float finalSize = Mathf.Clamp(engineerBasedSize, 0f, maxParticleSize);

            var main = clickParticles.main;
            main.startSize = finalSize;

            // Adjust trail width if trails enabled
            var trails = clickParticles.trails;
            if (trails.enabled)
            {
                trails.widthOverTrail = finalSize * 0.5f;
            }

            clickParticles.Play();
        }

        CheckFloorStatus();
    }

    //Method to generate random new floor that visually stacks on the last one
    private void BuildNewFloor()
    {
        //Apply a one-time vertical offset before the first floor is placed
        if (currentFloor == 0)
        {
            nextBuildPosition += new Vector3(0, .5f, 0); // tiny extra offset
        }

        GameObject prefabToSpawn = floorPrefabs[UnityEngine.Random.Range(0, floorPrefabs.Count)];
        GameObject newFloor = Instantiate(prefabToSpawn, nextBuildPosition, Quaternion.identity, towerBase);

        currentFloor++;

        FloorInformation floorInfo = newFloor.GetComponent<FloorInformation>();

        //Sets the health of the new floor to how many clicks it took
        floorInfo.CreateFloor((uint)currentClickProgress, currentFloor);

        //Adds the new floor to list of floor
        GameManager.floorObjects.Add(floorInfo);

        

        //Dynamically assign sorting order
        SpriteRenderer sr = newFloor.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = currentFloor; //Or use -currentFloor to reverse direction
        }

        nextBuildPosition += new Vector3(0, buildHeightOffset, 0);
        
        NewFloor?.Invoke(); //New floor is built invoke the event to let the manager know!

        //AudioManager.PlaySoundEffect("Upgrade4", 7);
    }


    //This method is called by WorkerManager once it figures out if you can buy the stuff.
    public static void IncreaseMultiplyer()
    {
        if (multiplyer == 1) multiplyer = 2;
        else multiplyer *= 1.7f;                //(owen) - I changed this to balance the game better now that workers scale
    }

    void CheckFloorStatus()
    {
        if (currentClickProgress >= currentClickRequirement)
        {
            BuildNewFloor();
            currentClickProgress = 0f;
            currentClickRequirement = Mathf.Ceil(currentClickRequirement * clickRequirementMultiplier);
            hammerAnimObject.transform.position = nextBuildPosition;
        }
    }
}
