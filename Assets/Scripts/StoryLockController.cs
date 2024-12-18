using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HomeByMarch;
using TMPro;
using ExitGames.Client.Photon.StructWrapping;
public class StoryLockController : MonoBehaviour
{
    public Image images;
    public TMP_Text titleText;
    public TMP_Text bodyText;


    int _stepCount;
    [System.Serializable]
    public class StoryLock
    {
        public GameObject lockObject;
        public int requiredSteps;
        public bool isPreviousStoryCompleted = false;
        public bool isUnlocked = false;
    }

    public StoryLock[] storyLocks;
    // public int steps;
    [SerializeField] OverallStepCounter stepCount;
    private Sprite newImage;

    private void Start()
    {
        // stepCount = new OverallStepCounter();
        _stepCount = stepCount.overallSteps; 
        if (_stepCount == 0)
        {
            Debug.LogError("OverallStepCounter instance not found in the scene!");
        }
        
        
        newImage = LoadSprite("stories/unknown");

        foreach (StoryLock storyLock in storyLocks)
        {
            if (storyLock.lockObject != null)
            {
                storyLock.lockObject.SetActive(true);
            }
        }
        Debug.Log("StoryLockController started with " + _stepCount + " steps.");

    }

    void Update()
    {
        if (_stepCount == 0) return;

        foreach (StoryLock storyLock in storyLocks)
        {
            if (!storyLock.isUnlocked && CanUnlock(storyLock))
            {
                UnlockStoryLock(storyLock);
            }
            else if (storyLock.lockObject != null && storyLock.isUnlocked)
            {
                storyLock.lockObject.SetActive(false);
            }
        }
    }

    private bool CanUnlock(StoryLock storyLock)
    {
        //int steps = stepCount.GetOverallSteps();
        return _stepCount >= storyLock.requiredSteps && storyLock.isPreviousStoryCompleted;
        PlayerPrefs.Save();
    }

    // Unlock a specific story lock
    private void UnlockStoryLock(StoryLock storyLock)
    {
        storyLock.isUnlocked = true;
        if (storyLock.lockObject != null)
        {
            storyLock.lockObject.SetActive(false);
        }
    }

    public void OnPreviousStoryComplete(int storyLockIndex)
    {
        if (storyLockIndex >= 0 && storyLockIndex < storyLocks.Length)
        {
            storyLocks[storyLockIndex].isPreviousStoryCompleted = true;
            //storyLocks[storyLockIndex].isUnlocked = true;
        }
    }

    // lock infos: conditions
    private Sprite LoadSprite(string path)
    {
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogError($"Failed to load sprite at path: {path}");
        }
        return sprite;
    }

    // private string newTMPTexts = "Story is still lock. You must first finish the story of";

    private string[] newTMPTitles = new string[]
    {
        //The Beginning - "Compass"
        "INERTIA",
        "POT OF KNOWLEDGE",
        "THE FIRST STEP",

        //A THOUSAND MILES
        "AT EACH OTHER’S THROATS",
        "UNCROSSABLE WALL",
        "DREAM OF THE CELESTIAL CHAMBER",

        //MARCH
        "A FUTURE UNCERTAIN",
        "A PAST ONE CANNOT RETURN TO",
        "WHEN ALL HOPE IS LOST"
    };

    public void OnButtonClick(int index)
    {
        images.sprite = newImage;
        if (index >= 0 && index < storyLocks.Length)
        {
            
            StoryLock storyLock = storyLocks[index];

            if (!storyLock.isUnlocked) // Check if the story is still locked
            {
                string conditions = "To unlock this story:\n\n";
                conditions += $"- Required Steps: {storyLock.requiredSteps}\n";

                // Add the title of the previous story that must be completed
                if (index > 0) // Ensure there is a previous story
                {
                    conditions += $"- Complete \"{newTMPTitles[index - 1]}\" first\n";
                }

                // Set the title and body text to display the conditions
                titleText.text = newTMPTitles[index];
                bodyText.text = conditions;

                // Log the conditions for debugging
                Debug.Log($"Conditions for unlocking {newTMPTitles[index]}:\n{conditions}");
            }
            else
            {
                Debug.Log($"Story {newTMPTitles[index]} is already unlocked.");
            }
        }
        else
        {
            Debug.LogWarning($"Invalid index {index}. No story lock found.");
        }
        Debug.Log($"Button clicked while{CanUnlock(storyLocks[index])} ");
    }
}