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

    private int currentFloor = 0;
    private float currentClickRequirement;
    private float currentClickProgress = 0f;
    private Vector3 nextBuildPosition;

    [SerializeField]
    GameObject topBorder; //For dynamically moving the top border

    public static event Action NewFloor; //Listener for when a new floor is created

    void Start()
    {
        currentClickRequirement = startingClickRequirement;
        nextBuildPosition = towerBase.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            OnClickBuild();
        }

        //Checks if the next build pos is off screen, this will make it so top border scales dynamically
        
        if(nextBuildPosition.y > Camera.main.orthographicSize)
        {
            topBorder.transform.position = nextBuildPosition + new Vector3(0,buildHeightOffset,0);
        }
    }

    //Clicking anywhere on the screen calls this
    public void OnClickBuild()
    {
        currentClickProgress++;

        Debug.Log("I am being clicked!");

        if (currentClickProgress >= currentClickRequirement)
        {
            BuildNewFloor();
            currentClickProgress = 0f;
            currentClickRequirement = Mathf.Ceil(currentClickRequirement * clickRequirementMultiplier);
        }
    }

    //Method to generate random new floor that visually stacks on the last one
    private void BuildNewFloor()
    {
        GameObject prefabToSpawn = floorPrefabs[UnityEngine.Random.Range(0, floorPrefabs.Count)];
        GameObject newFloor = Instantiate(prefabToSpawn, nextBuildPosition, Quaternion.identity, towerBase);

        //Dynamically assign sorting order
        SpriteRenderer sr = newFloor.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = currentFloor; //Or use -currentFloor to reverse direction
        }

        currentFloor++;
        nextBuildPosition += new Vector3(0, buildHeightOffset, 0);
        
        NewFloor?.Invoke(); //New floor is built invoke the event to let the manager know!
    }
}
