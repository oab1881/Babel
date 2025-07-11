using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloorInformation : MonoBehaviour
{
    // Reference to  current level of this floor
    uint level = 1;
    uint upgradeCost = 100;
    

    //On start all floors are gonna be set to tier one
    [SerializeField]
    UpgradeType currentUpgrade;


    //Will be deleted once currentUpgrade is fully implemented
    bool isArcherTower = false;
    bool isTemple = false;

    //Get reference to TowerExplode prefab which triggers when the tower is destroyed.
    [SerializeField]
    GameObject TowerExplosion;


    
    //*************** Need owen to check these 
    //List<AngleMovement> currentAttackingAngles = new List<AngleMovement>();

    public bool IsArcherTower => isArcherTower;

    /* *************** Other check
    public List<AngleMovement> CurrentAttackingAngles
    {
        get { return currentAttackingAngles; }
        set { currentAttackingAngles = value; }
    }
    */

    // Set in create floor, used for progression and cost scaling
    public int floorNum;

    // Reference to the upgrade text (child of upgradePanel on all prefab towers)
    [SerializeField] TMP_Text upgradeText;

    // References to the upgrade buttons
    //This will also be moved to upradeType Variable and will be instantiated instead
    [SerializeField] GameObject baseUpgrade;
    [SerializeField] GameObject archerUpgrade;
    [SerializeField] GameObject templeUpgrade;

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
    [SerializeField] FloorStyle[] availableStyles; // Drag 5 ScriptableObjects here in Inspector
    private FloorStyle currentStyle; // Randomly chosen style assigned when floor is created

    // Shared sprites for all archer/tower/temple upgrades (same across styles)
    [SerializeField] Sprite archerLv1Sprite;
    [SerializeField] Sprite archerLv2Sprite;
    [SerializeField] Sprite templeSprite;
    [SerializeField] Sprite cathedralSprite;

    // Called from GameManager when a new floor is created
    public void CreateFloor(uint health, int floorNum)
    {
        this.floorNum = floorNum;

        // Randomly pick a visual style from the list and assign the base level 1 sprite
        currentStyle = availableStyles[Random.Range(0, availableStyles.Length)];
        imageComponenet.sprite = currentStyle.baseLv1;
    }

    void Start()
    {
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
    }

    private void OnMouseEnter()
    {
        ShowButtons();

        if (isArcherTower)
        {
            leftArcherRadiusScript.ShowRadius();
            rightArcherRadiusScript.ShowRadius();
        }
    }

    private void OnMouseExit()
    {
        HideButtons();

        if (isArcherTower)
        {
            leftArcherRadiusScript.HideRadius();
            rightArcherRadiusScript.HideRadius();
        }
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
        if (GameManager.money >= upgradeCost)
        {
            return true;
        }
        else
        {
            TMPFadeWarning.Show(); // Display warning if not enough money
            return false;
        }
    }


    //All upgrades will be pointless in new version
    // Called by base upgrade button
    public void baseUpgreade()
    {
        if (!CheckUpgrade()) return;

        // Change sprite based on current level (uses selected style)
        if (level == 1) imageComponenet.sprite = currentStyle.baseLv2;
        else imageComponenet.sprite = currentStyle.baseLv3;

        // Increase gold per second based on level
        if (level == 1) goldGeneratorScript.GoldPerSecond += 40;
        if (level == 2) goldGeneratorScript.GoldPerSecond += 100;

        level++;
        ApplyUpgradeCost(700); // Apply cost increase and refresh UI
    }

    public void ArcherUpgrade()
    {
        if (!CheckUpgrade()) return;

        isArcherTower = true;

        // Archer towers use shared sprites regardless of initial base style
        imageComponenet.sprite = (level == 1) ? archerLv1Sprite : archerLv2Sprite;

        level++;
        //goldGeneratorScript.GoldPerSecond += 10; archers won't make any more money now

        // Show archer visuals
        leftArcher.SetActive(true);
        rightArcher.SetActive(true);

        SetDetectionRadius();

        if (isArcherTower)
        {
            leftArcherRadiusScript.ShowRadius();
            rightArcherRadiusScript.ShowRadius();
        }

        ApplyUpgradeCost(2500);
    }

    public void TempleUpgrade()
    {
        if (!CheckUpgrade()) return;

        isTemple = true;
        level++;

        imageComponenet.sprite = templeSprite;

        // Temple affects Herecy mechanics
        GameManager.DecreaseHerecy(30); //for balancing i reduced it to 30 from 50
        HerecyManager.herecyAMin += 3;
        goldGeneratorScript.GoldPerSecond = 0;

        ApplyUpgradeCost(950);
    }


    //This will be pointless in new system
    // Plays audio, updates UI, and recalculates cost for next upgrade
    private void ApplyUpgradeCost(int baseIncrease)
    {
        AudioManager.PlaySoundEffect("Upgrade", 5);
        GameManager.money -= upgradeCost;
        GameManager.Instance.UpdateGoldUI();
        IncreaseCost(baseIncrease);
        HideButtons();
        ShowButtons();
    }



    // Increases detection radius for both archers
    //Should change this to universal for other attack objects in the future
    //Add the new params, (ArcherInfoScript object, float newRadius)
    private void SetDetectionRadius()
    {
        float newRadius = (leftArcherInfoScript.DetectionRadius == 0) ? 1.6f : leftArcherInfoScript.DetectionRadius * 2;

        leftArcherInfoScript.DetectionRadius = newRadius;
        rightArcherInfoScript.DetectionRadius = newRadius;
        leftArcherRadiusScript.DetectionRadius = newRadius;
        rightArcherRadiusScript.DetectionRadius = newRadius;

        rightArcherRadiusScript.GenerateCircle();
        leftArcherRadiusScript.GenerateCircle();
    }

    // Hides upgrade buttons and resets display
    private void HideButtons()
    {
        StartPanelLerp(panelStartPos);

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy &&
                child.gameObject.name != "Canvas" &&
                child.gameObject.name != "TowerPanel (1)" &&
                child.gameObject.name != "TowerPanel" &&
                child.gameObject.name != "UpgradeCost" &&
                child.gameObject.name != "TowerImage")
            {
                child.gameObject.SetActive(false);
            }
        }

        if (IsArcherTower)
        {
            leftArcher.SetActive(true);
            rightArcher.SetActive(true);
        }

        towerHighlight.SetActive(false);
    }

    // Shows relevant upgrade buttons based on current level
    private void ShowButtons()
    {
        if (level == 1)
        {
            baseUpgrade.SetActive(true);
            archerUpgrade.SetActive(true);
        }

        if (level == 2 && !isArcherTower)
        {
            baseUpgrade.SetActive(true);
            templeUpgrade.SetActive(true);
        }

        if (level == 2 && isArcherTower)
        {
            archerUpgrade.transform.position = new Vector3(0, archerUpgrade.transform.position.y, 0);
            archerUpgrade.SetActive(true);
        }

        if (level < 3 || (level == 3 && isTemple))
        {
            upgradePanel.SetActive(true);
            StartPanelLerp(panelTargetPos);
        }

        towerHighlight.SetActive(true);

        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "Canvas" ||
                child.gameObject.name == "UpgradeCost" ||
                child.gameObject.name == "TowerPanel (1)" ||
                child.gameObject.name == "TowerPanel")
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    // Increases upgrade cost and updates the display text
    private void IncreaseCost(int amount)
    {
        upgradeCost += (uint)(amount + (30 * floorNum));
        upgradeText.text = GameManager.FormatNumbers(upgradeCost);
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