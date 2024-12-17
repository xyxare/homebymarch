using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HomeByMarch;
using HomeByMarch;  // Add this directive to reference the StepCountDemo class
public class StoryLockController : MonoBehaviour
{
    [System.Serializable]
    public class StoryLock
    {
        public GameObject lockObject;   
        public int requiredSteps;      
        public bool isPreviousStoryCompleted = false;  
        public bool isUnlocked = false; 
    }

    public StoryLock[] storyLocks;  
    private StepCountDemo stepCountDemo;  // Reference to the StepCountDemo class

    void Start()
    {
        stepCountDemo = FindObjectOfType<StepCountDemo>();  // Find the StepCountDemo instance
    }

    void Update()
    {
        foreach (StoryLock storyLock in storyLocks)
        {
            if (!storyLock.isUnlocked && CanUnlock(storyLock))
            {
                UnlockStoryLock(storyLock);
            }
        }
        PlayerPrefs.Save();
    }

    bool CanUnlock(StoryLock storyLock)
    {
        return storyLock.isPreviousStoryCompleted && stepCountDemo.dailyStepCount >= storyLock.requiredSteps;
    }

    // Unlock a specific story lock
    void UnlockStoryLock(StoryLock storyLock)
    {
        if (storyLock.lockObject != null)
        {
            storyLock.lockObject.SetActive(false); 
            storyLock.isUnlocked = true;         
            Debug.Log($"{storyLock.lockObject.name} unlocked!");
        }
    }

    public void OnPreviousStoryComplete(int storyLockIndex)
    {
        if (storyLockIndex >= 0 && storyLockIndex < storyLocks.Length)
        {
            storyLocks[storyLockIndex].isPreviousStoryCompleted = true;
        }
    }
}