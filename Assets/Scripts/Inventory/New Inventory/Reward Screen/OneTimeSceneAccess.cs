using UnityEngine;
using UnityEngine.SceneManagement;

public class OneTimeSceneAccess : MonoBehaviour
{
    public string sceneName = "OneTimeScene"; // Name of the scene that should only be accessed once
    public string sceneAccessedKey = "SceneAccessed"; // Key for PlayerPrefs

    void Start()
    {
        // Check if the scene has been accessed before
        if (PlayerPrefs.GetInt(sceneAccessedKey, 0) == 0)
        {
            // Scene hasn't been accessed yet, allow access to it
            PlayerPrefs.SetInt(sceneAccessedKey, 1); // Mark the scene as accessed
            PlayerPrefs.Save();

            // Load the scene
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            // Scene has already been accessed, show a message or load a different scene
            Debug.Log("This scene can only be accessed once.");
        }
    }
}
