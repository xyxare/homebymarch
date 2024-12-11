using Repforge.StepCounterPro;
using System;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.Android;

namespace HomeByMarch
{
    [Serializable]
    public class StepData
    {
        public int numberOfSteps; // Steps for today
        public int dailySteps;    // Daily steps count
        public int overallSteps;  // Overall steps count
        public string lastSavedDate; // Store the last saved date to check if a new day has started
    }

    public class StepCountDemo : MonoBehaviour
    {
        public TMP_Text text, debugText;
        public Canvas permissionCanvas;
         // Event channel to broadcast step count changes

        private int dailyStepCount;
        private int overallStepCount;  // Track overall steps
        private string stepJsonFilePath;

        private void OnEnable()
        {
            // Subscribe to the step gained event
            GameEventsManager.instance.stepCountEvents.onStepsGained += StepsGained;
            GameEventsManager.instance.stepCountEvents.onStepCountChange += StepCountChanged;
        }

        private void OnDisable()
        {
            // Unsubscribe from the step gained event
            GameEventsManager.instance.stepCountEvents.onStepsGained -= StepsGained;
            GameEventsManager.instance.stepCountEvents.onStepCountChange -= StepCountChanged;
        }

        void Start()
        {
            InitializeStepCounter();
            LoadStepData();  // Load saved step data
            RequestPermission();  // Request permissions for step counting

            StepCounterRequest permissionRequest = new StepCounterRequest();
            permissionRequest
                .OnPermissionGranted(OnPermissionGranted)
                .OnPermissionDenied(OnPermissionDenied)
                .RequestPermission();
        }

        private void InitializeStepCounter()
        {
            stepJsonFilePath = Application.persistentDataPath + "/stepData.json";
            debugText.text = stepJsonFilePath;
        }

        private void OnPermissionGranted()
        {
            // Fetch today's steps after permission is granted
            StepsToday();
        }

        private void OnPermissionDenied()
        {
            permissionCanvas.gameObject.SetActive(true);
        }

        private void LoadStepData()
        {
            if (File.Exists(stepJsonFilePath))
            {
                string jsonString = File.ReadAllText(stepJsonFilePath);
                StepData data = JsonUtility.FromJson<StepData>(jsonString);

                if (data.lastSavedDate != DateTime.Today.ToString("yyyy-MM-dd"))
                {
                    dailyStepCount = 0;  // Reset daily steps if it's a new day
                }
                else
                {
                    dailyStepCount = data.dailySteps;  // Load today's steps
                }

                overallStepCount = data.overallSteps;  // Load the overall step count
            }
            else
            {
                overallStepCount = 0;
                dailyStepCount = 0;
            }
        }

        private void SaveStepData()
        {
            StepData data = new StepData
            {
                numberOfSteps = dailyStepCount,
                dailySteps = dailyStepCount,
                overallSteps = overallStepCount,
                lastSavedDate = DateTime.Today.ToString("yyyy-MM-dd")
            };

            string stepCountString = JsonUtility.ToJson(data);
            File.WriteAllText(stepJsonFilePath, stepCountString);
            Debug.Log("Step data saved: " + stepCountString);
        }

        public void StepsToday()
        {
            StepCounterRequest request = new StepCounterRequest();
            request
                .Since(DateTime.Today)
                .OnQuerySuccess((value) =>
                {
                    dailyStepCount = value;  // Update daily steps
                    overallStepCount += dailyStepCount;  // Update overall step count

                    text.text = value.ToString();  // Update UI

                    // Trigger step gained event
                    GameEventsManager.instance.stepCountEvents.StepsGained(dailyStepCount);

                    SaveStepData();  // Save data after updating steps
                })
                .OnPermissionDenied(() => permissionCanvas.gameObject.SetActive(true))
                .Execute();
        }

        private void StepsGained(int steps)
        {
            // Log the steps gained
            Debug.Log($"Steps gained: {steps}");

            // Update the step count using the gained steps
            dailyStepCount += steps;

            // Trigger the step count change event
            GameEventsManager.instance.stepCountEvents.StepCountChange(dailyStepCount);
        }

        private void StepCountChanged(int updatedSteps)
        {
            // Log the updated step count
            Debug.Log($"Step count updated: {updatedSteps}");

            // Update the UI
            text.text = updatedSteps.ToString();

            // Optionally, invoke the FloatEventChannel
            
        }

        async void RequestPermission()
        {
#if UNITY_ANDROID
            AndroidRuntimePermissions.Permission fileManagementResult = await AndroidRuntimePermissions.RequestPermissionAsync("android.permission.MANAGE_EXTERNAL_STORAGE");
#endif
        }
    }
}
