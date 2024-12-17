using System.Collections;
using TMPro;  // Add the TextMesh Pro namespace
using UnityEngine;
using UnityEngine.UI;  // Add the UI namespace for Button

public class DungeonGameController : MonoBehaviour
{
    public GameObject enemyPrefab;      // Reference to the enemy prefab
    public Transform[] spawnPoints;     // Array of spawn points for enemies
    public float timeLimit = 60f;       // Time limit to defeat enemies
    public int totalEnemiesToDefeat = 10; // Total number of enemies to defeat

    private int enemiesDefeated = 0;    // Track the number of defeated enemies
    private int enemiesSpawned = 0;     // Track the number of enemies spawned
    private float timeRemaining;        // Time remaining in the dungeon
    private bool gameActive = false;    // Flag to track if the game is active

    // References to TextMeshProUGUI components
    public TMP_Text timerText;          // TMP Text for displaying the timer
    public TMP_Text enemiesDefeatedText; // TMP Text for displaying defeated enemies
    public Button startButton;          // Reference to the start button
    public GameObject winPanel;         // Reference to the Win Panel (UI)
    public GameObject challengePanel;   // Reference to the Dungeon Challenge Panel (UI)

    void Start()
    {
        // Add listener to the start button to start the game
        startButton.onClick.AddListener(StartGame);
        winPanel.SetActive(false); // Make sure the win panel is hidden at the start
    }

    // This method is called when the Start button is clicked
    void StartGame()
    {
        gameActive = true;        // Set the game to active
        timeRemaining = timeLimit; // Reset the timer
        enemiesDefeated = 0;      // Reset the defeated enemies count
        enemiesSpawned = 0;       // Reset the spawned enemies count
        UpdateUI();  
        challengePanel.SetActive(false);             // Update the UI to reflect the initial state
        StartCoroutine(SpawnEnemies()); // Start spawning enemies
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
        timerText.text = Mathf.Ceil(timeRemaining).ToString();
        enemiesDefeatedText.text = enemiesDefeated.ToString() + "/" + totalEnemiesToDefeat;
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
            winPanel.SetActive(true); // Show the win panel
        }
        else
        {
            Debug.Log("Time's up! You lose.");
        }
        // Additional game end logic (e.g., stop gameplay, show end screen)
    }

    // Coroutine to spawn enemies at random spawn points
    IEnumerator SpawnEnemies()
    {
        while (gameActive && enemiesSpawned < totalEnemiesToDefeat)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Length);  // Random spawn point
            Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);  // Spawn enemy at chosen point
            enemiesSpawned++;  // Increment the spawn count
            yield return new WaitForSeconds(2f);  // Wait before spawning the next enemy (adjust as needed)
        }
    }
}
