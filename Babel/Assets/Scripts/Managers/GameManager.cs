using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    


    public  static uint money = 0;
    public static uint herecy = 0;
    public static int health = 3;

    [SerializeField]
    private TextMeshProUGUI goldDisplay;

    [SerializeField]
    private TextMeshProUGUI engineerDisplay;

    [SerializeField]
    private GameObject restartButton;

    [SerializeField]
    private TextMeshProUGUI restartText;

    private bool isGameOver = false;

    public string[] defeatMessages = new string[] {
    "You Lose",          // English
    "Perdiste",          // Spanish
    "Tu as perdu",       // French
    "Du hast verloren",  // German
    "Hai perso",         // Italian
    "Voce perdeu",       // Portuguese
    "Tu has perdut",     // Catalan
    "Zure galera",       // Basque
    "Jij verliest",      // Dutch
    "Sen kaybettin",     // Turkish
    "Pierdeti",          // Romanian
    "Tu caiste",         // Galician
    "Izgubio si",        // Croatian
    "Ti si izgubio",     // Serbian (Latin script)
    "Du tapte",          // Norwegian
    "Porazka",           // Polish
    };




    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //prevent duplicates
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        AudioManager.SetVolume(0, 0.1f);
        AudioManager.PlayMusic("BabelAmbient", 0);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) money += 10000;
        if (Input.GetKeyDown(KeyCode.R)) RestartGame();
    }

    

    //Method that increments gold and calls UpdateGoldUI
    public void AddGold(float amount)
    {
        money += (uint)amount;
        UpdateGoldUI();
    }

    //Method that updates the gold count in game
     public void UpdateGoldUI()
    {
        if (goldDisplay != null)
        {
            goldDisplay.text = FormatNumbers(money);
        }
    }

    //Method that updates the multiplier count
    //Does not do anything
    public void UpdateMultUI()
    {
        if (engineerDisplay != null)
        {
            engineerDisplay.text = FormatNumbers((int)Clicker.multiplyer);
        }
    }


    //Formats currency using suffixes
    public static string FormatNumbers(float curr)
     {
        string[] suffixes = {
        "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No",
        "Dc", "Ud", "Dd", "Td", "Qad", "Qid", "Sxd", "Spd", "Ocd",
        "Nod", "Vg", "Uvg", "Dvg"
        };
        double value = (double)curr;
         if (value < 1000)
             return value.ToString("0");

         int suffixIndex = 0;
         while (value >= 1000 && suffixIndex < suffixes.Length - 1)
         {
             value /= 1000;
             suffixIndex++;
         }

         return value.ToString("0.##") + suffixes[suffixIndex];
     }
    

    public static void DecreaseHerecy(int amount)
    {
        if(amount > herecy) herecy = 0;
        
        else herecy -= (uint)amount;
    }


    //This uses function in FloorInfo.cs to reduce the health and is called in 
    //AngleMovement.cs
    public static void DecreaseHealth(int damageAmount) //Keeping damage amount differnet angles may do different damage
    {
        health-= damageAmount;

        Debug.Log(health);
        if (health == 0)
        {
            FloorInformation.ExplodeEntireTower(); //Kaboom
        }
    }
    
    //Global gameover logic
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("GAME OVER - Tower destroyed");

        HerecyManager.Instance.gameObject.SetActive(false); //Temp adding this in so game doesn't crash when game ends

        //Freeze all gold generation
        FloorInformation[] allFloors = FindObjectsOfType<FloorInformation>();
        foreach (var floor in allFloors)
        {
            var goldGen = floor.GetComponent<GoldGenerator>();
            if (goldGen != null) goldGen.enabled = false;
        }

        //Disable all CoinPopups
        CoinPopup[] coinPopups = FindObjectsOfType<CoinPopup>();
        foreach (var popup in coinPopups)
        {
            popup.gameObject.SetActive(false);
        }

        //Stop player interaction (clicking)
        Clicker clicker = FindObjectOfType<Clicker>();
        if (clicker != null) clicker.enabled = false;

        //Disable Hammering
        WorkersManager workMgr = FindObjectOfType<WorkersManager>();
        if (workMgr != null)
        {
            workMgr.hammerAnimObject.SetActive(false);  //shut off hammer
            workMgr.clickParticles.gameObject.SetActive(false); //shut off particles
        }
        else
        {
            Debug.LogWarning("WorkersManager not found in scene.");
        }

        //Set You Lose text in a foreign language
        string chosenMessage = defeatMessages[Random.Range(0, defeatMessages.Length)];

        // Set the message
        if (restartText != null)
        {
            restartText.text = chosenMessage;
        }
        else
        {
            Debug.LogWarning("restartText reference not set in inspector.");
        }



        //Trigger UI Game Over screen or scene reload
        restartButton.SetActive(true);
        
    }

    //Function to reload the scene to restart
    public void RestartGame()
    {
        Debug.Log("RESET GAME");
        // Reset all static game variables
        money = 0;
        herecy = 0;
        health = 3;
        FloorManager.floor = 0;
        FloorManager.floorObjects.Clear();
        Clicker.multiplyer = 1; // If you have a multiplier, reset it too
        //Need to reset size of multiplyer particles here ****

        // Destroy current towers manually
        FloorInformation[] floors = FindObjectsOfType<FloorInformation>();
        foreach (var floor in floors)
        {
            Destroy(floor.gameObject);
        }

        // Reset any other persistent game objects or singletons
        isGameOver = false;

        // If you have player prefs or saved state, clear those too
        // PlayerPrefs.DeleteAll(); // Optional

        // Reload scene after small delay
        StartCoroutine(ReloadSceneWithDelay(0.3f));
    }

    private IEnumerator ReloadSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Gameplay");
    }

}


