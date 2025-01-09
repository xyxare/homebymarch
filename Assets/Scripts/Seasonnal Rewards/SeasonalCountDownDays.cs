using System;
using TMPro;
using UnityEngine;

public class SeasonalDaysCountDown : MonoBehaviour
{
    public TextMeshProUGUI countdownText; // Reference to the TextMeshProUGUI component
    public DateTime targetDate; // The target date for the countdown

    void Start()
    {
        // Start the countdown immediately
        StartCoroutine(UpdateCountdown());
    }

    System.Collections.IEnumerator UpdateCountdown()
    {
        while (true)
        {
            DateTime targetDate = DateTime.Now.Date.AddDays(20);
            TimeSpan timeRemaining = targetDate - DateTime.Now.Date;

            if (timeRemaining.TotalDays <= 0)
            {
                countdownText.text = "0 Days Left";
                yield break; // Stop the countdown when the target date is reached
            }

            // Format the remaining time as Days
            countdownText.text = string.Format("{0} Days Left", timeRemaining.Days);

            yield return new WaitForSeconds(86400); // Wait for 1 day (86400 seconds) before updating the time again
        }
    }
}