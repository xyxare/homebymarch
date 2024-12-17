using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HomeByMarch;

public class UserLevel : MonoBehaviour
{
    [SerializeField] public TMP_Text levelText;
    public TMP_Text currentStepCountText;
    public TMP_Text currentStepCountTextOutside;
    public TMP_Text totalStepsForNextLevelText;
    public TMP_Text remainingStepsForNextLevelText;
    public TMP_Text overallStepCountText;  // Added for overall steps display

    [Header("UI stuff")]
    public Image experienceBarImage;

    public StepCountDemo stepCountDemo;

    public int currentUserLevel;
    public int totalStepsForNextLevel;
    public int currentStepCount;
    public int dailyStepCount;  // Added for daily steps tracking
    public int overallStepCount; // Added for overall steps tracking
    public int remainingStepsForNextLevel; // Declare remaining steps for next level

    [Header("User Info")]
    public TMP_Text userNameText; // Add a field for username display
    public PlayerData playerData; // Reference to the PlayerData script

    private string stepJsonFilePath;
    private string stepCountData;

    void Awake()
    {
        stepJsonFilePath = Application.persistentDataPath + "/stepData.json";
        stepCountData = File.ReadAllText(stepJsonFilePath);

        // Load PlayerData from the scene
        playerData = FindObjectOfType<PlayerData>(); // Make sure PlayerData is attached to a GameObject
    }

    void Update()
    {
        UpdateInformation();
        UpdateText();
        UpdateExperienceBar();
    }

    public int CalculateTotalStepsForLevel(int level)
    {
        return (100 * Mathf.FloorToInt(Mathf.Pow(level - 1, 2.35f)));
    }

    void UpdateText() 
    {
        levelText.text = currentUserLevel.ToString();
        currentStepCountText.text = "Daily steps: " + currentStepCount.ToString();
        currentStepCountTextOutside.text = currentStepCount.ToString(); // Update the outside text with just the number
        totalStepsForNextLevelText.text = "Walk a total of " + ReformatIntToText(totalStepsForNextLevel) + " steps to advance to Level " + (currentUserLevel + 1);
        remainingStepsForNextLevelText.text = "Remaining steps for next level: " + ReformatIntToText(remainingStepsForNextLevel); // Update remaining steps text

        // Display overall steps in the new text field
        overallStepCountText.text = "Overall steps: " + ReformatIntToText(overallStepCount);

        // Display the username if PlayerData is available
        if (userNameText != null && playerData != null)
        {
            userNameText.text = playerData.playerName; // Show the username from PlayerData
        }
    }

    void UpdateInformation()
    {
        // Read the step data
        stepCountData = File.ReadAllText(stepJsonFilePath);
        StepData data = JsonUtility.FromJson<StepData>(stepCountData);

        currentStepCount = data.numberOfSteps;
        dailyStepCount = data.dailySteps;
        overallStepCount = data.overallSteps;

        totalStepsForNextLevel = CalculateTotalStepsForLevel(currentUserLevel + 1);
        remainingStepsForNextLevel = totalStepsForNextLevel - currentStepCount; // Update remaining steps for next level

        if (currentStepCount >= totalStepsForNextLevel)
        {
            currentUserLevel++;
        }
    }

    public string ReformatIntToText(int number)
    {
        if (number >= 10000)
        {
            return "" + Mathf.Floor(number / 1000) + "K";
        }
        else
        {
            return number.ToString();
        }
    }

    public void UpdateExperienceBar()
    {
        int totalStepsForPreviousLevel = CalculateTotalStepsForLevel(currentUserLevel);
        int differenceInSteps = totalStepsForNextLevel - totalStepsForPreviousLevel;

        float fillAmount = (float)(differenceInSteps - remainingStepsForNextLevel) / differenceInSteps;

        if (experienceBarImage != null)
        {
            experienceBarImage.fillAmount = fillAmount;
        }
    }
}
