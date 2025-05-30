using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloorInformation : MonoBehaviour
{
    // Reference to health and current level of this floor
    uint health;
    uint level = 1;
    uint upgradeCost = 100;
    uint maxHealth;

    bool isArcherTower = false;
    bool isTemple = false;

    List<AngleMovement> currentAttackingAngles = new List<AngleMovement>();

    public bool IsArcherTower => isArcherTower;

    public List<AngleMovement> CurrentAttackingAngles
    {
        get { return currentAttackingAngles; }
        set { currentAttackingAngles = value; }
    }

    // Set in create floor, used for progression and cost scaling
    int floorNum;

    // Reference to the upgrade text (child of upgradePanel on all prefab towers)
    [SerializeField] TMP_Text upgradeText;

    // References to the upgrade buttons
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
    [SerializeField] SpriteRenderer sR;

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
        this.health = health;
        this.floorNum = floorNum;
        maxHealth = health;

        // Randomly pick a visual style from the list and assign the base level 1 sprite
        currentStyle = availableStyles[Random.Range(0, availableStyles.Length)];
        sR.sprite = currentStyle.baseLv1;
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

    // Called by base upgrade button
    public void baseUpgreade()
    {
        if (!CheckUpgrade()) return;

        // Change sprite based on current level (uses selected style)
        if (level == 1) sR.sprite = currentStyle.baseLv2;
        else sR.sprite = currentStyle.baseLv3;

        // Increase gold per second based on level
        if (level == 1) goldGeneratorScript.GoldPerSecond += 20;
        if (level == 2) goldGeneratorScript.GoldPerSecond += 70;

        level++;
        ApplyUpgradeCost(700); // Apply cost increase and refresh UI
    }

    public void ArcherUpgrade()
    {
        if (!CheckUpgrade()) return;

        isArcherTower = true;

        // Archer towers use shared sprites regardless of initial base style
        sR.sprite = (level == 1) ? archerLv1Sprite : archerLv2Sprite;

        level++;
        goldGeneratorScript.GoldPerSecond += 10;

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

        sR.sprite = templeSprite;

        // Temple affects Herecy mechanics
        GameManager.DecreaseHerecy(50);
        HerecyManager.herecyAMin += 3;
        goldGeneratorScript.GoldPerSecond = 0;

        ApplyUpgradeCost(950);
    }

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
    private void SetDetectionRadius()
    {
        float newRadius = (leftArcherInfoScript.DetectionRadius == 0) ? 1.5f : leftArcherInfoScript.DetectionRadius * 2;

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
                child.gameObject.name != "UpgradeCost")
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

    // Called by GameManager to apply damage to this floor
    public void DamageFloor(int amount)
    {
        if (amount > health)
        {
            // This would be game over
            Debug.Log("Game Over!");
        }
        else
        {
            health -= (uint)amount;
        }
    }
}