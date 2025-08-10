using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public  static uint money = 0;
    public static uint herecy = 0;
    public static int health = 3;

    public int finalScore;

    [SerializeField]
    private TextMeshProUGUI goldDisplay;

    [SerializeField]
    private TextMeshProUGUI engineerDisplay;

    [SerializeField]
    private GameObject restartButton;

    [SerializeField]
    private TextMeshProUGUI restartText;

    //An event for when the game is reset
    //Subscribers: FloorManager(UnSubscribe())
    //Invoked in RestartGame()
    public static event Action Reset;

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
        Utilities.Initialize(this);
    }

    private void Start()
    {
        AudioManager.SetVolume(0, 0.1f);
        AudioManager.PlayMusic("BabelAmbient", 0);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) ChangeGold(10000);
        if (Input.GetKeyDown(KeyCode.R)) RestartGame();
    }

    

    //Method that increments gold and calls UpdateGoldUI
    //Everywhere where gold is changed uses this function either through passing in positive or negative amount
    public static void ChangeGold(float amount)
    {
        money += (uint)amount;
        Instance.UpdateGoldUI();
    }

    //Method that updates the gold count in game
     public void UpdateGoldUI()
    {
        if (goldDisplay != null)
        {
            goldDisplay.text = FormatNumbers(money);
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

    //Pauses all activity in the game
    public static void PauseGame()
    {
        Camera.main.gameObject.GetComponent<CameraMovement>().enabled = false;
        foreach (FloorInformation floor in FloorManager.floorObjects)
        {
            floor.Pause();
            floor.enabled = false;
        }
        HerecyManager.Instance.CanSpawnAngels = false;
        HerecyManager.Instance.CanHeresyAMin = false;
        Clicker.canBuild = false;
        WorkersManager.canBuy = false;
    }


    //Resumes all activity in the game
    public static void ResumeGame()
    {
        Camera.main.gameObject.GetComponent<CameraMovement>().enabled = true;
        foreach (FloorInformation floor in FloorManager.floorObjects)
        {
            floor.Resume();
            floor.enabled = true;
        }
        HerecyManager.Instance.CanSpawnAngels = true;
        HerecyManager.Instance.CanHeresyAMin = true;
        Clicker.canBuild = true;
        WorkersManager.canBuy = true;
    }


    
    //Global gameover logic
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Before destroying anything:
        FloorInformation[] allFloors = FindObjectsOfType<FloorInformation>();
        finalScore = allFloors.Length; // or use loop to get highest floorNum

        Debug.Log("GAME OVER - Tower destroyed");

        HerecyManager.Instance.gameObject.SetActive(false); //Temp adding this in so game doesn't crash when game ends


        PauseGame();

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
        string chosenMessage = defeatMessages[UnityEngine.Random.Range(0, defeatMessages.Length)];

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
        WorkersManager.EngineerCount = 0;

        FloorManager.floorObjects.Clear();
        Reset?.Invoke();
        HerecyManager.HeresyAMin = 3;

        Clicker.multiplyer = 1; // If you have a multiplier, reset it too
        Clicker.Instance.particlesEnabled = false;
        Clicker.ResetParticles();

        ResumeGame();


        //Destroy current towers manually
        FloorInformation[] floors = FindObjectsOfType<FloorInformation>();
        foreach (var floor in floors)
        {
            Destroy(floor.gameObject);
        }

        //Reset any other persistent game objects or singletons
        AudioManager.StopSound(0);
        AudioManager.PlayMusic("MesopotamianLullaby", 0);
        isGameOver = false;


        //Stop all gameplay-related coroutines and systems
        StopAllCoroutines(); // Stop any running here
        

        // Reload scene after small delay
        StartCoroutine(ReloadSceneWithDelay(0.3f));
    }

    private IEnumerator ReloadSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Gameplay");
    }
}


