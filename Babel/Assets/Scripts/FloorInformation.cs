using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloorInformation : MonoBehaviour
{
    //Reference to health and current level of this floor
    uint health;
    uint level = 1;
    uint upgradeCost = 100;
    uint maxHealth;
    bool isArcherTower = false;
    int archerTowerLv = -1; //Negative 1 signafies it doesn't exist *Note I may delete this later not sure of it's use*
    uint damagePerSecond = 0;

    List<AngleMovement> currentAttackingAngles = new List<AngleMovement>();


    public uint DamagePerSecond { get { return damagePerSecond; } }
    public bool IsArcherTower { get { return isArcherTower; } }

    public List<AngleMovement> CurrentAttackingAngles { 
        get { return currentAttackingAngles; }
        set { currentAttackingAngles = value; }
    }

    //Set in create floor we can use this for progression and for effecting other floors
    int floorNum;

    //Reference to upgradeText (child of upgradePanel on all prefab towers)
    [SerializeField]
    TMP_Text upgradeText;


    //Vector for range? Radius??
    bool isTemple = false;

    int currentSprite = -1; //Current Sprite -1 is used to help calculate where in the list we are for upgrades

    //Reference to plus button which will upgrade the tower
    [SerializeField]
    GameObject baseUpgrade;

    [SerializeField]
    GameObject archerUpgrade;

    [SerializeField]
    GameObject templeUpgrade;


    //Reference to Highlight outline
    [SerializeField]
    GameObject towerHighlight;

    
    //Reference to Price Panel and Lerp Info
    [SerializeField]
    GameObject upgradePanel;
    [SerializeField] float moveDistance = 5f; //How far to move left
    [SerializeField] float lerpSpeed = 5f;
    private Vector3 panelStartPos;
    private Vector3 panelTargetPos;
    private Coroutine moveCoroutine;

    [SerializeField]
    GoldGenerator goldGeneratorScript;

    [SerializeField]
    SpriteRenderer sR;

    [SerializeField]
    SphereCollider LeftCollider;

    [SerializeField]
    SphereCollider RightCollider;



    //0 BaseUpgrade LV2
    //1 BaseUpgrade LV3
    //2 ArcherTower LV1
    //3 ArcherTower LV2
    //4 Temple
    //5 Catherdral
    [SerializeField]
    Sprite[] changeSprites;

    //Creates a floor by setting the floor health equal to the clicks it took to build the floor
    //This is done in clicker.cs on GameManager
    public void CreateFloor(uint health, int floorNum)
    {
        this.health = health;
        this.floorNum= floorNum;
        maxHealth= health;
    }

    void Start()
    {
        panelStartPos = upgradePanel.transform.localPosition;
        panelTargetPos = panelStartPos + Vector3.left * moveDistance;

        //Set upgrade price in the text and increases it based on floor
        upgradeCost *= (uint)floorNum;
        upgradeText.text = GameManager.FormatNumbers(upgradeCost);


    }


    private void Update()
    {
    }

    // Commented out til we can fix the upgrade button to display properly

    //When the mouse enters this object we display the upgrade button
    private void OnMouseEnter()
    {
        ShowButtons();
    }

    //When the mouse leaves we see if the upgrade button is active and set it to not be active
    private void OnMouseExit()
    {
        HideButtons();
    }

    

    //Used to move the panel from behind the tower to display upgrade price
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

        upgradePanel.transform.localPosition = targetPos; //Snap exactly at end
    }

    //Upgrades *Note in future we may want to change to graph of nodes 


    //Checks to see if an upgreade can be done
    public bool CheckUpgrade()
    {
        if (GameManager.money >= upgradeCost)
        {
            return true;
        }
        else
        {
            TMPFadeWarning.Show();
            return false;
        }
    }

    //Called on the BaseUpgradeButton onClick event
    public void baseUpgreade()
    {
        if (CheckUpgrade())
        {
            //Check what sprite we are setting to
            //Check Change sprite array for the reference for this index
            if(level == 1)
            {
                UpdateSprite(0);
            }
            else
            {
                UpdateSprite(1);
            }

            
            //Increase how much money is generated
            if(level == 1) goldGeneratorScript.GoldPerSecond += 20; //Will make 30
            if (level == 2) goldGeneratorScript.GoldPerSecond += 70;    //Will make 100

            //Increase our overall level counter
            level++;
            GameManager.money -= upgradeCost;
            IncreaseCost(700);

            HideButtons();
            ShowButtons();
        }
    }

    public void ArcherUpgrade()
    {
        if (CheckUpgrade())
        {
            isArcherTower = true;

            if (level == 1) UpdateSprite(2);
            else UpdateSprite(3);
           

            //Make the prefab switch styles match the if statements
            level++;
            goldGeneratorScript.GoldPerSecond += 10; // Will Make 20
            GameManager.money -= upgradeCost;

            IncreaseCost(2500); // Increases the price to 2800

            HideButtons();
            ShowButtons();
        }
    }

    public void TempleUpgrade()
    {
        if (CheckUpgrade())
        {
            isTemple = true;
            level++;

            UpdateSprite(4);

            //Make the prefab switch styles match the if statements
            GameManager.DecreaseHerecy(50); //Decreases herecy by 50
            HerecyManager.herecyAMin += 3; //Increases total amount per minute
            goldGeneratorScript.GoldPerSecond = 0;
            GameManager.money -= upgradeCost;
            IncreaseCost(950);


            HideButtons();
            ShowButtons();
        }
    }


    //Function that hides buttons that shouldn't be displayed based on level
    private void HideButtons()
    {
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.activeInHierarchy && (child.gameObject.name != "Canvas" || child.gameObject.name != "TowerPanel (1)" || child.gameObject.name != "TowerPanel"))
            {
                child.gameObject.SetActive(false);
            }
        }


        towerHighlight.SetActive(false);

        //Move panel back to the right
        StartPanelLerp(panelStartPos);
    }


    //Opposite of hide buttons shows buttons based on level should be
    private void ShowButtons()
    {
        //Takes the level into account and hides buttons accordingly
        //Shows the upgrade buttons for level 1
        if (level == 1)
        {
            //Sets the money upgrade button to visible
            baseUpgrade.SetActive(true);
            archerUpgrade.SetActive(true);

        }

        //For a tower upgraded to base lv 2
        if (level == 2 && isArcherTower == false)
        {
            baseUpgrade.SetActive(true);
            templeUpgrade.SetActive(true);

        }

        if(level == 2 && isArcherTower == true)
        {
            //Move archer button to the middle
            archerUpgrade.transform.position = new Vector3(0, archerUpgrade.transform.position.y, 0);
            archerUpgrade.SetActive(true);
        }

        if(level < 3)
        {
            //Move panel to the left
            StartPanelLerp(panelTargetPos);
        }

        if(level == 3 && isTemple == true)
        {
            //Move panel to the left
            StartPanelLerp(panelTargetPos);
        }

        

        //Highlight the tower no matter the upgrade level
        towerHighlight.SetActive(true);

        foreach (Transform child in this.transform)
        {
            if (child.gameObject.name == "Canvas" || child.gameObject.name == "UpgradeCost" || child.gameObject.name == "TowerPanel (1)" || child.gameObject.name == "TowerPanel")
            {
                child.gameObject.SetActive(true);
            }
        }

        
    }


    //Increases the cost of upgrades
    private void IncreaseCost(int amount)
    {
        upgradeCost += (uint)(amount + (30* floorNum)); 

        // Update text using format numbers
        upgradeText.text = GameManager.FormatNumbers(upgradeCost);
    }


    private void UpdateSprite(int spriteIndex)
    {
        //Actually set those sprites
        sR.sprite = changeSprites[spriteIndex];

    }

    //Causes damage to the floor is called in GameManager as it has a list to all floor
    public void DamageFloor(int amount)
    {
        if (amount > health)
        {
            //This would be game over
            Debug.Log("Game Over!");
        }
        else health -= (uint)amount;
    }
}
