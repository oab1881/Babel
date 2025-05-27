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

    private int currentFloor = 0;
    private float currentClickRequirement;
    public static float currentClickProgress = 0f;
    private Vector3 nextBuildPosition;

    [SerializeField]
    GameObject topBorder; //For dynamically moving the top border

    public static event Action NewFloor; //Listener for when a new floor is created

    public static uint multiplyer = 1;

    List<GameObject> floorsList = new List<GameObject>(); //List containing all built floors in the heirarchy

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
            topBorder.transform.position = nextBuildPosition + new Vector3(0,buildHeightOffset,0);
        }
    }

    //Clicking anywhere on the screen calls this
    public void OnClickBuild()
    {
        //Increases click progress by one times the multiplyer or just (the multiplyer)
        currentClickProgress+=  multiplyer;

        lastClickTime = Time.time;

        //Move and trigger hammer animation
        if (hammerAnimator != null)
        {
            hammerAnimObject.transform.position = nextBuildPosition;    //Move hammer to the build position
            hammerAnimator.SetBool("isHammering", true);
        }

        CheckFloorStatus();
    }

    //Method to generate random new floor that visually stacks on the last one
    private void BuildNewFloor()
    {
        // Apply a one-time vertical offset before the first floor is placed
        if (currentFloor == 0)
        {
            nextBuildPosition += new Vector3(0, .5f, 0); // tiny extra offset
        }

        GameObject prefabToSpawn = floorPrefabs[UnityEngine.Random.Range(0, floorPrefabs.Count)];
        GameObject newFloor = Instantiate(prefabToSpawn, nextBuildPosition, Quaternion.identity, towerBase);


        //Sets the health of the new floor to how many clicks it took
        newFloor.GetComponent<FloorInformation>().CreateFloor((uint)currentClickProgress);

        //Adds the new floor to list of floor
        floorsList.Add(newFloor);

        currentFloor++;

        //Dynamically assign sorting order
        SpriteRenderer sr = newFloor.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = currentFloor; //Or use -currentFloor to reverse direction
        }

        nextBuildPosition += new Vector3(0, buildHeightOffset, 0);
        
        NewFloor?.Invoke(); //New floor is built invoke the event to let the manager know!
    }


    //Function to be used later which increases multiplyer
    //Buttons in ui will call this function
    //Will have to incorporate a way to figure out if player has enough money
    //Also need to implement a fail vs success outcome
    public static void IncreaseMultiplyer()
    {
        multiplyer *= multiplyer;
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
