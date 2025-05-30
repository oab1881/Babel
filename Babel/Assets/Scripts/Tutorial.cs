using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI basharBox; //text from bashar
    [SerializeField] private GameObject textBackdrop;
    [SerializeField] private GameObject creditsBackdrop;
    [SerializeField] private TextMeshProUGUI credits;
    [SerializeField] private GameObject Bashar;
    [SerializeField] private GameObject BABEL;

    [SerializeField] private float typingDelay = 0.05f; // Delay between characters
    [SerializeField] private float lineDelay = 0.5f;    // Delay after each line

    private bool skipRequested = false;

    // Your lines
    private string[] lines = {
        "Greetings underling, I am Lord Bashar.",
        "You must be my new overseer.",
        "Our goal here is to build a tower to reach the heavens.",
        "You must hire workers to build for you,",
        "and engineers to increase our efficiency.",
        "You must click to build, and use W/S to scroll",
        "Now get out there and begin building my tower, my kingdom,... my"
    };

    private string[] creditsLine =
    {
        "Made by Owen Beck and Jake Wardell"
    };

    private void Start()
    {
        StartCoroutine(PlayTutorial());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            skipRequested = true;
        }
    }

    private IEnumerator PlayTutorial()
    {
        basharBox.text = "";

        foreach (string line in lines)
        {
            yield return StartCoroutine(TypeLine(basharBox, line));
            yield return new WaitForSeconds(lineDelay);
        }

        yield return new WaitForSeconds(1.5f);

        //Tutorial done: shut off Bashar and backdrop, turn on BABEL titles
        if (Bashar != null) Bashar.SetActive(false);
        if (textBackdrop != null) textBackdrop.SetActive(false);
        if (BABEL != null) BABEL.SetActive(true);   //activate title
        if (credits != null) creditsBackdrop.SetActive(true);   //activate credits

        foreach (string line in creditsLine)
        {
            yield return StartCoroutine(TypeLine(credits, line));
            yield return new WaitForSeconds(lineDelay + 1);
        }

        yield return new WaitForSeconds(3.0f);

        Debug.Log("Tutorial done. BABEL title now active.");

        //Load Gameplay scene
        SceneManager.LoadScene("Gameplay");
        AudioManager.StopSound(0);
        AudioManager.PlayMusic("MesopotamianLullaby", 0);
    }

    private IEnumerator TypeLine(TextMeshProUGUI targetText, string line)
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
}
