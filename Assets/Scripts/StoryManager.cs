using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StoryManager : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public string[] sceneNames;
    public GameObject loadingScreen; 
    public Slider progressBar;    
    private int lastClickedIndex = -1;
    private float lastClickTime = 0f; 
    private float clickResetTime = 2f;
    private int currentIndex = 0;
    private string[] newTMPTexts = new string[]
    {
        //The Beginning - "The Call to Journey"
        "A world unknown, a compass broken. Each shard a question: Where am I? Who am I now? And how far must I walk to find the answers?",
        "Solace is no sanctuary, but its cries for help are loud enough to drown out my own. Can I truly piece together their hope while chasing fragments of my own?",
        "Every journey begins with uncertainty. With only fragments in hand and doubts in my heart, I take the first step—into the unknown, into myself.",

        //The Trials - "The Rising Storm"
        "Beneath the earth lie shadows of greed and whispers of the past. Each dungeon tests more than my strength—it questions my purpose. How much of myself will I lose before I emerge into the light?",
        "Allies once trusted, now shadows in my path. The taste of betrayal is bitter, but the question cuts deeper: Is the compass worth more than the bonds I’ve built along the way?",
        "Standing before the Gatekeeper, I see not an enemy but a mirror. To pass, I must answer the hardest question yet: Am I worthy of the journey I’ve begun?",

        //The Resolution - "Homeward Bound"
        "A throne left empty, a king left forgotten. His madness speaks of the compass’s truth, but will his tragedy become my own? Or will I rise where he fell?",
        "The compass spins one last time, pointing to a choice I never wanted to make. Return to the life I’ve lost, or stay to protect the world I’ve found. What will I choose, and what will it cost?",
        "The journey ends where it began, but I am no longer the same. Whether here or there, home is no longer a place—it’s the path I walked and the lives I’ve touched."
    };

    private string[] newTMPTitles = new string[]
    {
        //The Beginning - "The Call to Journey"
        "THE SHATTERED COMPASS",
        "THE TOWN OF SOLACE",
        "THE FIRST STEP",

        //The Trials - "The Rising Storm"
        "THE DUNGEONS OF DREAD",
        "THE BROTHERHOOD’S BETRAYAL",
        "THE GATEKEEPER",

        //The Resolution - "Homeward Bound"
        "THE FORGOTTEN KING",
        "THE TRIAL OF SACRIFICE",
        "MARCH’S RETURN"
    };

    
    private int clickCount = 0;


    public void Next()
    {
        currentIndex = (currentIndex + 3) % newTMPTexts.Length;
        UpdateText();
    }

    public void Previous()
    {
        currentIndex = (currentIndex - 3 + newTMPTexts.Length) % newTMPTexts.Length;
        UpdateText();
    }

    public void OnButtonClick(int index)
    {
        // click is consecutive
        if (index == lastClickedIndex && Time.time - lastClickTime <= clickResetTime)
        {
            // Load the scene associated with the button
            if (sceneNames != null && index < sceneNames.Length && !string.IsNullOrEmpty(sceneNames[index]))
            {
                StartCoroutine(LoadSceneAsync(sceneNames[index]));
            }
            else
            {
                Debug.LogWarning($"No scene assigned for index {index}");
            }
        }
        else
        {
            // Display text associated with the button
            if (index < newTMPTexts.Length && index < newTMPTitles.Length)
            {
                titleText.text = newTMPTitles[index];
                bodyText.text = newTMPTexts[index];
            }
            else
            {
                Debug.LogWarning($"No text assigned for index {index}");
            }

            // Update last clicked index and time
            lastClickedIndex = index;
            lastClickTime = Time.time;
        }
    }
    private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true); 
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            if (progressBar != null)
            {
                progressBar.value = Mathf.Clamp01(operation.progress / 0.9f);
            }
            yield return null;
        }
    }

    private void UpdateText()
    {
        titleText.text = newTMPTitles[currentIndex];
        bodyText.text = newTMPTexts[currentIndex];
    }
}