using System.Collections;
using TMPro;  // Add the TextMesh Pro namespace
using UnityEngine;

public class DungeonGameController : MonoBehaviour
{
    public GameObject enemyPrefab;      // Reference to the enemy prefab
    public Transform[] spawnPoints;     // Array of spawn points for enemies
    public float timeLimit = 60f;       // Time limit to defeat enemies
    public int totalEnemiesToDefeat = 10; // Total number of enemies to defeat

    private int enemiesDefeated = 0;    // Track the number of defeated enemies
    private float timeRemaining;        // Time remaining in the dungeon
    private bool gameActive = true;     // Flag to track if the game is active

    // References to TextMeshProUGUI components
    public TMP_Text timerText;          // TMP Text for displaying the timer
    public TMP_Text enemiesDefeatedText; // TMP Text for displaying defeated enemies

    void Start()
    {
        timeRemaining = timeLimit;
        UpdateUI();
        StartCoroutine(SpawnEnemies());
    }

    void Update()
    {
        if (gameActive)
        {
            // Update the timer
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndGame(false);  // Time is up, game over
            }

            UpdateUI();
        }
    }

    // Update UI texts
    void UpdateUI()
    {
        // Using TMP_Text components to update the UI text
        timerText.text = "Time Left: " + Mathf.Ceil(timeRemaining).ToString();
        enemiesDefeatedText.text = "Enemies Defeated: " + enemiesDefeated.ToString();
    }

    // This method is called when an enemy is defeated
    public void OnEnemyDefeated()
    {
        enemiesDefeated++;
        if (enemiesDefeated >= totalEnemiesToDefeat)
        {
            EndGame(true);  // Player has defeated all enemies, game won
        }
    }

    // Ends the game, either win or lose
    void EndGame(bool won)
    {
        gameActive = false;
        if (won)
        {
            Debug.Log("You win! All enemies defeated.");
        }
        else
        {
            Debug.Log("Time's up! You lose.");
        }
        // Additional game end logic (e.g., display end screen, stop gameplay)
    }

    // Coroutine to spawn enemies at random spawn points
    IEnumerator SpawnEnemies()
    {
        while (gameActive && enemiesDefeated < totalEnemiesToDefeat)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Length);  // Random spawn point
            Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);  // Spawn enemy at chosen point
            yield return new WaitForSeconds(2f);  // Wait before spawning the next enemy (adjust as needed)
        }
    }
}
