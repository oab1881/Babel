using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloorInformation : MonoBehaviour
{
    uint upgradeCost = 100;
    

    //On start all floors are gonna be set to tier one
    [SerializeField]
    UpgradeType currentUpgrade;


    //Get reference to TowerExplode prefab which triggers when the tower is destroyed.
    [SerializeField]
    GameObject TowerExplosion;

    // Set in create floor, used for progression and cost scaling
    public int floorNum;

    // Reference to the upgrade text (child of upgradePanel on all prefab towers)
    [SerializeField] TMP_Text upgradeText;

    // References to the upgrade buttons
    [SerializeField] GameObject buttonPrefab;
    List<GameObject> buttons;


    // Highlight outline for when mouse hovers
    [SerializeField] GameObject towerHighlight;

    // Lerp movement for upgrade panel display
    [SerializeField] GameObject upgradePanel;
    [SerializeField] float moveDistance = 5f; // How far to move left
    [SerializeField] float lerpSpeed = 5f;
    private Vector3 panelStartPos;
    private Vector3 panelTargetPos;
    private Coroutine moveCoroutine;


    [SerializeField] GoldGenerator goldGeneratorScript;
    [SerializeField] Image imageComponenet;


    //Should prob make these gameObjects to be instantiated
    //Instead of always having them active
    [SerializeField] GameObject leftArcher;
    [SerializeField] GameObject rightArcher;
    ShowArcherRadius leftArcherRadiusScript;
    ShowArcherRadius rightArcherRadiusScript;
    Archers leftArcherInfoScript;
    Archers rightArcherInfoScript;


    // Style definitions used for base upgrade levels (each FloorStyle has 3 sprites)
    private FloorStyle currentStyle; // Randomly chosen style assigned when floor is created
    private int styleIndex = 0;



    // Called from GameManager when a new floor is created
    public void CreateFloor(uint health, int floorNum)
    {
        this.floorNum = floorNum;

        // Randomly pick a visual style from the list and assign the base level 1 sprite
        currentStyle = currentUpgrade.floorStyle[Random.Range(0, currentUpgrade.floorStyle.Count)];
        imageComponenet.sprite = currentStyle.styles[styleIndex];
        styleIndex++;


        goldGeneratorScript.GoldPerSecond = 20;
    }

    void Start()
    {
        buttons = new List<GameObject>();
        panelStartPos = upgradePanel.transform.localPosition;
        panelTargetPos = panelStartPos + Vector3.left * moveDistance;

        // Initial cost scales based on floor number
        upgradeCost *= (uint)floorNum;
        upgradeText.text = GameManager.FormatNumbers(upgradeCost);

        // Get references to the radius display and archer info scripts on both sides
        leftArcherRadiusScript = leftArcher.GetComponent<ShowArcherRadius>();
        rightArcherRadiusScript = rightArcher.GetComponent<ShowArcherRadius>();
        leftArcherInfoScript = leftArcher.GetComponent<Archers>();
        rightArcherInfoScript = rightArcher.GetComponent<Archers>();

        //Creates buttons on creation
        CreateButtons();
    }

    private void Update()
    {
        //Basic update state machine for specific functionality of each floor
        if (currentUpgrade.currectType == floorType.baseFloor)
        {
            
        }
        else if (currentUpgrade.currectType == floorType.archer)
        {
            leftArcherInfoScript.CanAttack = true;
            rightArcherInfoScript.CanAttack = true;
        }
        else if (currentUpgrade.currectType == floorType.temple)
        {

        }
    }

    private void OnMouseEnter()
    {
        ShowButtons();
    }

    private void OnMouseExit()
    {
        HideButtons();
    }

    // Moves the upgrade panel UI to target position smoothly
    void StartPanelLerp(Vector3 targetPos)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(LerpPanel(targetPos));
    }

    IEnumerator LerpPanel(Vector3 targetPos)
    {
        Vector3 startPos = upgradePanel.transform.localPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * lerpSpeed;
            upgradePanel.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        upgradePanel.transform.localPosition = targetPos; // Snap exactly at end
    }

    // Checks if the player has enough money to upgrade
    public bool CheckUpgrade()
    {
        if (GameManager.Money >= upgradeCost)
        {
            return true;
        }
        else
        {
            TMPFadeWarning.Show(); // Display warning if not enough money
            return false;
        }
    }


    public void Upgrade(GameObject buttonReference)
    {
        //Checks if we can upgrade indicates if not and returns so below doesn't run
        if (!CheckUpgrade()) return;

        //Plays the upgrade sound, changes the gold value and 
        AudioManager.PlaySoundEffect("Upgrade", 5);
        GameManager.IncreaseGold(-upgradeCost);
        
        //Used to figure out what button was clicked
        int index = 0;
        //Deletes all current buttons
        for(int i = 0;  i < buttons.Count; i++)
        {
            if (buttonReference == buttons[i]) index = i; //Checks to see if it was the clicked button
            Destroy(buttons[i]);
        }
        buttons.Clear();

 
        //Sets the current upgrade to the next one based on index and list of upgrades
        currentUpgrade = currentUpgrade.nextUpgrades[index];
       
        //Increases the cost of upgrade TO the value that is passed in
        IncreaseCost(currentUpgrade.nextUpgradecost);

        //Change the sprite
        //Checks if the there are any floor styles
        if(currentUpgrade.floorStyle.Count != 0)
        {
            //Copyies above where it will select a random floor style of the options it is given
            currentStyle = currentUpgrade.floorStyle[Random.Range(0, currentUpgrade.floorStyle.Count)];
            //Style index 0 is the first one
            styleIndex = 0;

            //Sets the sprite to this new image
            imageComponenet.sprite = currentStyle.styles[styleIndex];
            //Increases the styleindex for next time
            styleIndex++;
        }
        else
        {
            //Sets the sprite using the style index that was increased in the last upgrade run
            imageComponenet.sprite = currentStyle.styles[styleIndex];
            //Increases styleindex for next time
            styleIndex++;
        }

        //Hide buttons -> Create the new buttons dynamically -> Show Buttons
        HideButtons();
        CreateButtons();
        ShowButtons();

        //Update stats
        goldGeneratorScript.GoldPerSecond = currentUpgrade.goldPerSecond;
        HerecyManager.IncreaseHeresy(currentUpgrade.herecyChange);
        HerecyManager.HeresyAMin += currentUpgrade.herecyPerSecond;
        //Insert population stuff here

        //Set up attacking details
        SetDetectionRadius(currentUpgrade.attackRange);
        rightArcherInfoScript.DamageASecond = currentUpgrade.dps;
        leftArcherInfoScript.DamageASecond = currentUpgrade.dps;

    }

    // Increases upgrade cost and updates the display text
    private void IncreaseCost(int amount)
    {
        upgradeCost = (uint)(amount + (30 * floorNum));
        upgradeText.text = GameManager.FormatNumbers(upgradeCost);
    }

    // Increases detection radius for both archers
    //Should change this to universal for other attack objects in the future
    //Add the new params, (ArcherInfoScript object, float newRadius)
    private void SetDetectionRadius(float newRadius) 
    { 
    
        leftArcherInfoScript.DetectionRadius = newRadius;
        rightArcherInfoScript.DetectionRadius = newRadius;
        leftArcherRadiusScript.DetectionRadius = newRadius;
        rightArcherRadiusScript.DetectionRadius = newRadius;

        rightArcherRadiusScript.GenerateCircle();
        leftArcherRadiusScript.GenerateCircle();
    }

    // Shows relevant upgrade buttons based on current level
    private void ShowButtons()
    {
        //If there are more upgrades show the price panel
        if (currentUpgrade.nextUpgrades.Count != 0) StartPanelLerp(panelTargetPos);

        //Highlight the tower
        towerHighlight.SetActive(true);

        //For every button show it
        foreach (GameObject btn in buttons)
        {
            btn.SetActive(true);
        }

        //Display the radius
        leftArcherRadiusScript.ShowRadius();
        rightArcherRadiusScript.ShowRadius();
    }


    // Hides upgrade buttons and resets display
    private void HideButtons()
    {
        StartPanelLerp(panelStartPos);

        //Highlight the tower
        towerHighlight.SetActive(false);

        foreach (GameObject btn in buttons)
        {
            btn.SetActive(false);
        }

        leftArcherRadiusScript.HideRadius();
        rightArcherRadiusScript.HideRadius();
    }

    /// <summary>
    /// Helper function for creating buttons dynamically for each upgrade
    /// </summary>
    void CreateButtons()
    {
        //For every upgrade the current upgrade can upgrade to
        for (int i = 0; i < currentUpgrade.nextUpgrades.Count; i++)
        {
            //Creates a button and makes it parent the the image of the tower gameobject
            //Do this because the it dynamically positions the buttons
            buttons.Add(Instantiate(buttonPrefab, imageComponenet.gameObject.transform));

            //Gets the hoverStyle from the current upgrade
            //Base State
            //Hover state
            HoverButtons buttonStyle = buttons[i].GetComponent<HoverButtons>();

            //Sets the buttons button styles to the button Script that each button prefab has 
            buttonStyle.normalSprite = currentUpgrade.nextUpgrades[i].buttons.Default;
            buttonStyle.hoverSprite = currentUpgrade.nextUpgrades[i].buttons.Click;

            //Sets the starting button image to the base
            buttonStyle.targetImage.sprite = buttonStyle.normalSprite;

            //We then create an arrow function so we can pass a value to the called function which is the current index
            //Index represents the link between the button and the upgrade it is attached to
            //Need this index = i for the lambda function using just i causes an error
            int index = i;
            buttons[i].GetComponent<Button>().onClick.AddListener(() => { Upgrade(buttons[index]); });
        }
    }

    /// <summary>
    /// Pauses archers and gold generation
    /// </summary>
    public void Pause()
    {
        //If floor is archer type stop attacking
        if(currentUpgrade.currectType == floorType.archer)
        {
            leftArcherInfoScript.CanAttack = false;
            rightArcherInfoScript.CanAttack = false;
        }

        //Stop generating gold
        goldGeneratorScript.StopAllCoroutines();
        goldGeneratorScript.enabled = false;
    }

    /// <summary>
    /// Resumes archers and gold generation
    /// </summary>
    public void Resume()
    {
        //If floor is archer type then resume attacking
        if (currentUpgrade.currectType == floorType.archer)
        {
            leftArcherInfoScript.CanAttack = true;
            rightArcherInfoScript.CanAttack = true;
        }

        //Stop generating gold
        goldGeneratorScript.enabled = true;

    }


    //Method to explode the whole tower upon game Over
    public static void ExplodeEntireTower()
    {
        FloorInformation[] allFloors = FindObjectsOfType<FloorInformation>();

        //Sort from top to bottom (highest explodes first)
        System.Array.Sort(allFloors, (a, b) => b.floorNum.CompareTo(a.floorNum));
        GameManager.Instance.StartCoroutine(ExplodeFloorsWithDelay(allFloors, 0.2f));
        GameManager.Instance.GameOver();
    }

    public static IEnumerator ExplodeFloorsWithDelay(FloorInformation[] floors, float delay)
    {
        foreach (FloorInformation floor in floors)
        {
            if (floor != null)
            {
                floor.ExplodeFloor();


                CameraShake.Shake();
                yield return new WaitForSeconds(delay);
            }
        }
        
    }

    //Method to destroy each floor
    public void ExplodeFloor()
    {
        AudioManager.PlaySoundEffect("explode 3", 12);
        if (TowerExplosion != null)
        {
            TowerExplosion.SetActive(true);
            TowerExplosion.transform.SetParent(null); //Detach explosion so it's not destroyed with the floor
        }

        //Hide visuals
        imageComponenet.enabled = false;

        if (goldGeneratorScript != null)
            goldGeneratorScript.enabled = false;

        towerHighlight.SetActive(false);
         

        //Disable this script so nothing else runs
        this.enabled = false;

        //Actually destroy the entire floor GameObject after delay
        Destroy(gameObject, 1f); //Adjust delay as needed for explosion effect to finish
    }
}