using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayTutorial : MonoBehaviour
{
    private bool hasShownStartDialogue = false;
    private bool hasShownThreeFloorsDialogue = false;
    private bool hasShownFiveFloorsDialogue = false;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI basharBox;
    [SerializeField] private TextMeshProUGUI beliorBox;
    [SerializeField] private GameObject speakerBox;
    [SerializeField] private GameObject bashar;
    [SerializeField] private GameObject belior;

    [Header("Typing Settings")]
    [SerializeField] private float typingDelay = 0.05f;
    [SerializeField] private float lineDelay = 0.5f;

    private Queue<IEnumerator> dialogueQueue = new Queue<IEnumerator>();
    private bool isDialoguePlaying = false;

    void Start()
    {
        //Reset music
        AudioManager.StopSound(0);
        AudioManager.PlayMusic("MesopotamianLullaby", 0);

        if (basharBox != null) basharBox.gameObject.SetActive(false);
        if (beliorBox != null) beliorBox.gameObject.SetActive(false);
        if (speakerBox != null) speakerBox.gameObject.SetActive(false);

        EnqueueDialogue(RunIntroDialogue());
    }

    void Update()
    {
        int floorCount = FindObjectsOfType<FloorInformation>().Length;

        if (floorCount >= 3 && !hasShownThreeFloorsDialogue)
        {
            hasShownThreeFloorsDialogue = true;
            EnqueueDialogue(ThreeFloorsDialogue());
        }

        if (floorCount >= 5 && !hasShownFiveFloorsDialogue)
        {
            hasShownFiveFloorsDialogue = true;
            EnqueueDialogue(FiveFloorsDialogue());
        }

        if (!isDialoguePlaying && dialogueQueue.Count > 0)
        {
            StartCoroutine(RunDialogueQueue());
        }
    }

    private void EnqueueDialogue(IEnumerator dialogue)
    {
        dialogueQueue.Enqueue(dialogue);
    }

    private IEnumerator RunDialogueQueue()
    {
        isDialoguePlaying = true;
        while (dialogueQueue.Count > 0)
        {
            yield return StartCoroutine(dialogueQueue.Dequeue());
        }
        isDialoguePlaying = false;
    }

    private IEnumerator RunIntroDialogue()
    {
        if (hasShownStartDialogue) yield break;
        hasShownStartDialogue = true;

        yield return new WaitForSeconds(1.5f);

        belior.SetActive(true);
        yield return Speak(Speaker.Belior, "Build with purpose, child. We climb not for glory — but grace.");
        yield return new WaitForSeconds(0.5f);

        bashar.SetActive(true);
        belior.SetActive(false);
        yield return Speak(Speaker.Bashar, "As financial overseer, you must manage our sla..");
        yield return Speak(Speaker.Bashar, "I mean workers, engineers, warriors, and priests.");
        yield return Speak(Speaker.Bashar, "Click to begin construction.");
        yield return Speak(Speaker.Bashar, "Workers will handle the building for us..");
        yield return Speak(Speaker.Bashar, "while engineers will increase our efficiency.");
        yield return new WaitForSeconds(1);
        bashar.SetActive(false);
    }

    private IEnumerator ThreeFloorsDialogue()
    {
        belior.SetActive(true);
        bashar.SetActive(false);
        yield return Speak(Speaker.Belior, "Be wary of his majesty’s heresy.");
        yield return Speak(Speaker.Belior, "Be sure to build temples to quell his abomination to avoid divine punishment.");
        belior.SetActive(false);
    }

    private IEnumerator FiveFloorsDialogue()
    {
        bashar.SetActive(true);
        belior.SetActive(false);
        yield return Speak(Speaker.Bashar, "Hire archers to combat any entities that are sent to halt my ascent.");
        yield return Speak(Speaker.Bashar, "Now get on with it, I expect progress, not excuses.");
        bashar.SetActive(false);
    }

    private IEnumerator Speak(Speaker who, string line)
    {
        speakerBox.SetActive(true);
        TextMeshProUGUI activeBox = who == Speaker.Bashar ? basharBox : beliorBox;
        TextMeshProUGUI inactiveBox = who == Speaker.Bashar ? beliorBox : basharBox;

        if (inactiveBox != null) inactiveBox.gameObject.SetActive(false);

        if (activeBox != null)
        {
            activeBox.gameObject.SetActive(true);
            yield return TypeLine(activeBox, line);
            yield return new WaitForSeconds(lineDelay);
            activeBox.gameObject.SetActive(false);
        }

        speakerBox.SetActive(false);
    }

    private IEnumerator TypeLine(TextMeshProUGUI targetText, string line)
    {
        targetText.text = "";
        for (int i = 0; i < line.Length; i++)
        {
            targetText.text += line[i];
            yield return new WaitForSeconds(typingDelay);
        }
    }

    public enum Speaker
    {
        Belior,
        Bashar
    }
}
