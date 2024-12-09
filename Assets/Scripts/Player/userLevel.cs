using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UserLevel : MonoBehaviour
{

    [SerializeField]
    public TMP_Text levelText;
    public TMP_Text currentStepCountText;
    public TMP_Text currentStepCountTextOutside;
    public TMP_Text totalStepsForNextLevelText;
    public TMP_Text remainingStepsForNextLevelText;

    [Header("ui stuff")]
    public Image experienceBarImage;



    public int currentUserLevel;
    public int totalStepsForNextLevel;
    public int currentStepCount;
    private string stepJsonFilePath;
    public int remainingStepsForNextLevel;
    private string stepCountData;




    void Awake()
    {

        stepJsonFilePath = Application.persistentDataPath + "/stepData.json";
        stepCountData = System.IO.File.ReadAllText(stepJsonFilePath);

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
        currentStepCountText.text = "Total steps: " + currentStepCount.ToString();
        currentStepCountTextOutside.text = currentStepCount.ToString(); // Update the outside text with just the number
        totalStepsForNextLevelText.text = "Walk a total of " + ReformatIntToText(totalStepsForNextLevel) + " steps to advance to Level " + (currentUserLevel + 1);
    }

    void UpdateInformation()
    {


        stepCountData = System.IO.File.ReadAllText(stepJsonFilePath);

        totalStepsForNextLevel = CalculateTotalStepsForLevel(currentUserLevel + 1);
        currentStepCount = JsonUtility.FromJson<StepData>(stepCountData).numberOfSteps;
        remainingStepsForNextLevel = totalStepsForNextLevel - currentStepCount;

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

    // floor(100[(x-1)^2.35])

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