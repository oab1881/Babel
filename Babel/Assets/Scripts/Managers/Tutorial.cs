using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI basharBox; //text from bashar
    [SerializeField] private TextMeshProUGUI beliorBox; //text from belior
    [SerializeField] private TextMeshProUGUI basharBox2; //text from bashar
    [SerializeField] private TextMeshProUGUI beliorBox2; //text from belior
    [SerializeField] private GameObject basharBox2Parent;   
    [SerializeField] private GameObject beliorBox2Parent;
    [SerializeField] private GameObject singleBoxParent; //Parent of speakerBox
    [SerializeField] private GameObject textBackdrop;
    [SerializeField] private GameObject creditsBackdrop;
    [SerializeField] private TextMeshProUGUI credits;
    [SerializeField] private GameObject Bashar;
    [SerializeField] private GameObject Belior;
    [SerializeField] private GameObject BABEL;
    public enum Speaker { Bashar, Belior } //Enums for who is currently speaking

    [Header("Typing Settings")]
    [SerializeField] private float typingDelay = 0.05f; // Delay between characters
    [SerializeField] private float lineDelay = 0.5f;    // Delay after each line

    private bool skipRequested = false;

    private string[] creditsLine =
    {
        "Made by Owen Beck and Jake Wardell"
    };

    private void Start()
    {
        StartCoroutine(PlayIntro());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            skipRequested = true;
        }
    }

    private IEnumerator PlayIntro()
    {
        basharBox.text = "";
        beliorBox.text = "";
        basharBox.gameObject.SetActive(false);
        beliorBox.gameObject.SetActive(false);

        yield return Speak(Speaker.Bashar, "Greetings underling.");
        yield return Speak(Speaker.Bashar, "I am Lord Bashar, rightful ruler of this God-forsaken land.");
        yield return new WaitForSeconds(lineDelay);

        yield return Speak(Speaker.Bashar, "I have summoned you to oversee the Babel Project.");
        yield return Speak(Speaker.Bashar, "— the only hope for humanity…");
        yield return new WaitForSeconds(lineDelay);
        yield return Speak(Speaker.Bashar, "and my ascent.");
        yield return new WaitForSeconds(lineDelay);
        

        yield return Speak(Speaker.Bashar, "We will claw our way into the heavens…");
        yield return Speak(Speaker.Bashar, "and I shall take the throne of God Himself.");
        yield return new WaitForSeconds(lineDelay * 2);

        // Belior enters
        //if (Bashar != null) Bashar.SetActive(false);
        if (Belior != null) Belior.SetActive(true);
        //shake screen
        CameraShake.Shake();

        yield return Speak(Speaker.Belior, "HERESY!!!");
        yield return new WaitForSeconds(0.6f);

        yield return Speak(Speaker.Belior, "The tower shall be our stairway to salvation, not your diabolical fantasies.");
        yield return new WaitForSeconds(lineDelay);

        yield return Speak(Speaker.Belior, "I have seen the Heavens, child. It burned these eyes clean.");
        yield return Speak(Speaker.Belior, "Now I walk in blindness...");
        yield return new WaitForSeconds(lineDelay);

        yield return Speak(Speaker.Belior, "I strive to feel the divine presence of the Lord, not merely see it.");
        yield return new WaitForSeconds(lineDelay * 2);

        // Back to Bashar
        //if (Belior != null) Belior.SetActive(false);
        //if (Bashar != null) Bashar.SetActive(true);

        yield return Speak(Speaker.Bashar, "His Worshipfulness simply stared into the sun for too long.");
        yield return new WaitForSeconds(lineDelay);

        yield return Speak(Speaker.Bashar, "God has abandoned us here. We are in need of a new deity.");
        yield return Speak(Speaker.Bashar, "This divine calling is mine alone.");
        yield return new WaitForSeconds(lineDelay * 2);

        // Belior reacts
        //if (Bashar != null) Bashar.SetActive(false);
        //if (Belior != null) Belior.SetActive(true);
        //shake screen
        CameraShake.Shake();

        yield return Speak(Speaker.Belior, "BLASPHEMY!!!");
        yield return new WaitForSeconds(lineDelay * 2);

        // Both active
        if (Bashar != null) Bashar.SetActive(true);

        //SPLIT TEXTBOX LOGIC WILL GO HERE
        //yield return Speak(Speaker.Bashar, "We build to reach the heavens…");
        //yield return new WaitForSeconds(lineDelay * 1.2f);
        //
        //yield return Speak(Speaker.Bashar, "—to conquer the sky, to seize God’s crown!");
        //yield return new WaitForSeconds(lineDelay);
        //
        //yield return Speak(Speaker.Belior, "—to ascend in spirit, to bask in His eternal light!");
        //yield return new WaitForSeconds(lineDelay);
        //
        //yield return Speak(Speaker.Bashar, "To rule creation with mortal hands.");
        //yield return new WaitForSeconds(lineDelay);
        //
        //yield return Speak(Speaker.Belior, "To kneel before it in divine awe.");
        //yield return new WaitForSeconds(lineDelay);

        // SPLIT TEXTBOX LOGIC
        yield return SpeakTogether("We build to reach the heavens…", "We build to reach the heavens…");
        yield return new WaitForSeconds(lineDelay * 1.2f);

        yield return SpeakTogether("—to conquer the sky, to seize God’s crown!", "—to ascend in spirit, to bask in His eternal light!");
        yield return new WaitForSeconds(lineDelay);

        yield return SpeakTogether("To rule creation with mortal hands.", "To kneel before it in divine awe.");
        yield return new WaitForSeconds(lineDelay);


        //Split dialogue logic ends here
        ReturnToSingleBox();
        yield return Speak(Speaker.Bashar, "Then let us build, old man.");
        yield return new WaitForSeconds(lineDelay);

        yield return Speak(Speaker.Belior, "Yes… let us build.");
        yield return new WaitForSeconds(1.25f);

        // Title drop
        basharBox.gameObject.SetActive(false);
        beliorBox.gameObject.SetActive(false);

        if (textBackdrop != null) textBackdrop.SetActive(false);
        if (Bashar != null) Bashar.SetActive(false);
        if (Belior != null) Belior.SetActive(false);

        //shake screen
        CameraShake.Shake();

        if (BABEL != null) BABEL.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (creditsBackdrop != null) creditsBackdrop.SetActive(true);
        yield return TypeLine(credits, creditsLine[0]);
        yield return new WaitForSeconds(3f);

        //Load Gameplay scene
        SceneManager.LoadScene("Tutorial");
        AudioManager.StopSound(0);
        AudioManager.PlayMusic("MesopotamianLullaby", 0);
    }

    public IEnumerator TypeLine(TextMeshProUGUI targetText, string line)
    {
        targetText.text = "";
        skipRequested = false;

        for (int i = 0; i < line.Length; i++)
        {
            if (skipRequested)
            {
                targetText.text = line;
                yield break;
            }

            targetText.text += line[i];
            yield return new WaitForSeconds(typingDelay);
        }

        skipRequested = false;
    }

    //Toggles text boxes depending on who is speaking
    public IEnumerator Speak(Speaker who, string line)
    {
        TextMeshProUGUI activeBox;
        GameObject activeGO;
        GameObject inactiveGO;

        if (who == Speaker.Bashar)
        {
            activeBox = basharBox;
            activeGO = basharBox.gameObject;
            inactiveGO = beliorBox.gameObject;
        }
        else
        {
            activeBox = beliorBox;
            activeGO = beliorBox.gameObject;
            inactiveGO = basharBox.gameObject;
        }

        // Toggle visibility
        activeGO.SetActive(true);
        inactiveGO.SetActive(false);

        yield return TypeLine(activeBox, line);
    }

    public IEnumerator SpeakTogether(string basharLine, string beliorLine)
    {
        // Switch to split mode
        if (singleBoxParent != null) singleBoxParent.SetActive(false);
        if (basharBox2Parent != null) basharBox2Parent.SetActive(true);
        if (beliorBox2Parent != null) beliorBox2Parent.SetActive(true);

        basharBox2.text = "";
        beliorBox2.text = "";

        Coroutine basharTyping = StartCoroutine(TypeLine(basharBox2, basharLine));
        Coroutine beliorTyping = StartCoroutine(TypeLine(beliorBox2, beliorLine));

        yield return basharTyping;
        yield return beliorTyping;
    }

    public void ReturnToSingleBox()
    {
        if (basharBox2Parent != null) basharBox2Parent.SetActive(false);
        if (beliorBox2Parent != null) beliorBox2Parent.SetActive(false);
        if (singleBoxParent != null) singleBoxParent.SetActive(true);
    }

    public void SkipTutorial()
    {
        AudioManager.StopSound(0);
        AudioManager.PlayMusic("MesopotamianLullaby", 0);
        SceneManager.LoadScene("Tutorial");
    }
}
