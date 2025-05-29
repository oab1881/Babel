using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorInformation : MonoBehaviour
{
    //Reference to health and current level of this floor
    uint health;
    uint level = 1;
    uint upgradeCost = 50;
    uint maxHealth;
    bool isArcherTower = false;
    int archerTowerLv = -1; //Negative 1 signafies it doesn't exist *Note I may delete this later not sure of it's use*
    uint damagePerSecond = 10;
    //Vector for range? Radius??
    bool isTemple = false;

    int currentSprite = -1; //Current Sprite -1 is used to help calculate where in the list we are for upgrades

    //Reference to plus button which will upgrade the tower
    [SerializeField]
    GameObject baseUpgrade;

    [SerializeField]
    GameObject archerUpgrade;

    //Reference to Highlight outline
    [SerializeField]
    GameObject towerHighlight;

    [SerializeField]
    GameObject templeUpgrade;

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
    public void CreateFloor(uint health)
    {
        this.health = health;
        maxHealth= health;
    }

    void Start()
    {
        panelStartPos = upgradePanel.transform.localPosition;
        panelTargetPos = panelStartPos + Vector3.left * moveDistance;
    }


    private void Update()
    {
        //Make invisible on click
    }

    // Commented out til we can fix the upgrade button to display properly

    //When the mouse enters this object we display the upgrade button
    private void OnMouseEnter()
    {
        //Shows the upgrade buttons for level 1
        if (level == 1)
        {
            //Sets the money upgrade button to visible
            baseUpgrade.SetActive(true);
            archerUpgrade.SetActive(true);
            
        }

        if(level == 2 && isArcherTower == false)
        {
            baseUpgrade.SetActive(true);
            //Temple upgrade

        }

        //Highlight the tower no matter the upgrade level
        towerHighlight.SetActive(true);

        //Move panel to the left
        StartPanelLerp(panelTargetPos);
    }

    //When the mouse leaves we see if the upgrade button is active and set it to not be active
    private void OnMouseExit()
    {
        /*//If the button is active and the mouse moves out accounts for it when it is upgraded to max
        if (level == 1)
        {
            baseUpgrade.SetActive(false);
            archerUpgrade.SetActive(false);
            
        }*/


        foreach (Transform child in this.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                child.gameObject.SetActive(false);
            }
        }

       
        towerHighlight.SetActive(false);

        //Move panel back to the right
        StartPanelLerp(panelStartPos);

    }

    //Checks to see if an upgreade can be done
    public bool CheckUpgrade()
    {
        if(GameManager.money >= upgradeCost)
        {
            return true;
        }
        else
        {
            Debug.Log("Cant upgrade not enough money");
            return false;
        }
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



    public void baseUpgreade()
    {
        if (CheckUpgrade())
        {
            //Check what sprite we are setting to
            //Check Change sprite array for the reference for this index
            if(currentSprite == -1)
            {
                currentSprite = 0;
            }
            else
            {
                currentSprite = 1;
            }

            //Actually set those sprites
            sR.sprite = changeSprites[currentSprite];

            //Increase how much money is generated
            if(level == 1) goldGeneratorScript.GoldPerSecond += 10;
            if (level == 2) goldGeneratorScript.GoldPerSecond += 30;

            //Increase our overall level counter
            level++;
            GameManager.money -= upgradeCost;
            upgradeCost += 50;
        }
    }

    public void ArcherUpgrade()
    {
        if (CheckUpgrade())
        {
            isArcherTower = true;

            //Make the prefab switch styles match the if statements
            goldGeneratorScript.GoldPerSecond += 20;
            GameManager.money -= upgradeCost;
            upgradeCost += 100;
        }
    }

    public void TempleUpgrade()
    {
        if (CheckUpgrade())
        {
            //Make the prefab switch styles match the if statements
            goldGeneratorScript.GoldPerSecond = 0;
            GameManager.money -= upgradeCost;
            upgradeCost += 100;

        }
    }


}
