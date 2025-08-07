using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private float countDown;
    private float startTime = 60f;
    bool paused = false;
    List<GameObject> children;

    [SerializeField]
    GameObject buttonContainer;

    [SerializeField]
    GameObject eventButtonPrefab;

    List<Events> allEvents;

    Events currentEvent;

    [SerializeField]
    TMP_Text baseText;

    [SerializeField]
    Image baseSprite;

    [SerializeField]
    GameObject closeBtn;

    [SerializeField]
    List<GameObject> choiceButtons;



    // Start is called before the first frame update
    void Start()
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

        choiceButtons = new List<GameObject>();
        countDown = startTime;
        allEvents = GetEvents();
        SelectEvent();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused) countDown -= Time.deltaTime;

       if(countDown < 0.0f)
        {
            paused = true;
            GameManager.PauseGame();
            countDown = startTime;
            Utilities.DisplayAllChildren(gameObject, true);
            closeBtn.SetActive(false);
        }
    }

    void CreateButtons()
    {
        foreach(Choices choice in currentEvent.choices)
        {
            GameObject btn = Instantiate(eventButtonPrefab, buttonContainer.transform);
            Utilities.GetAllChildren(btn)[0].GetComponent<TMP_Text>().text = choice.choiceText;
            choiceButtons.Add(btn);
            btn.GetComponent<Button>().onClick.AddListener(() => { ChoiceClick(btn); });
        }
    }

    void ChoiceClick(GameObject button)
    {
        int index = -1;
        for (int i = 0; i < choiceButtons.Count; i++)
        {
            
            if (button == choiceButtons[i])
            {             
                index = i;   
            }
            Destroy(choiceButtons[i]);
        }
        choiceButtons.Clear();

        ShowResult(currentEvent.choices[index]);
    }

    /// <summary>
    /// Goes through the event.json file and converts events to event objects does this once in start
    /// </summary>
    /// <returns></returns>
    List<Events> GetEvents()
    {
        //Gets a TextAsset reference to the json file
        TextAsset jsonFile = Resources.Load<TextAsset>("Events/events");

        if (jsonFile == null)
        {
            Debug.LogError("event.json not found in Resources folder.");
            return new List<Events>();
        }

        string wrapped = "{ \"events\": " + jsonFile.text + " }";
        EventsWrapper eventsWrapper = JsonUtility.FromJson<EventsWrapper>(wrapped);  


        return eventsWrapper.events;
    }

    void ShowResult(Choices choice)
    {
        
        baseText.text = choice.resultText;
        closeBtn.SetActive(true);

        /*foreach (EffectType key in choice.effects.Keys)
        {
            if (key == EffectType.Money) GameManager.ChangeGold(choice.effects[key]);
            if (key == EffectType.Heresy) GameManager.UpdateHeresy(choice.effects[key]);
            if (key == EffectType.HeresyAMin) HerecyManager.HeresyAMin += choice.effects[key];
        }*/

        
    }

    void SelectEvent()
    {
        currentEvent = allEvents[Random.Range(0, allEvents.Count)];
        baseText.text = currentEvent.baseText;
        CreateButtons();
        Utilities.DisplayAllChildren(gameObject, false);
    }


    public void CloseWindow()
    {
        SelectEvent();
        paused = false;
        countDown = startTime;
        GameManager.ResumeGame();
        Utilities.DisplayAllChildren(gameObject, false);
    }


}
