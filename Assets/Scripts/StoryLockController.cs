using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryLockController : MonoBehaviour
{
    public GameObject[] storyLocks;

    private bool storyCompleted = false;

    void Update()
    {
        if (storyCompleted)
        {
            DisableStoryLocks();
        }
    }

    public void OnStoryComplete()
    {
        storyCompleted = true;
    }
    void DisableStoryLocks()
    {
        foreach (GameObject storyLock in storyLocks)
        {
            if (storyLock != null)
            {
                storyLock.SetActive(false); 
            }
        }
    }
}
