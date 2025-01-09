using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

using HomeByMarch;
public class DungeonGameController : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float timeLimit = 60f;
    public int totalEnemiesToDefeat = 10;
    public GameObject exitDungeon;
    private int enemiesDefeated = 0;
    private int enemiesSpawned = 0;
    private float timeRemaining;
    private bool gameActive = false;

    public TMP_Text timerText;
    public TMP_Text enemiesDefeatedText;
    public TMP_Text enemiesRemainingText;
    public Button startButton;
    public GameObject challengePanel;
    public GameObject goldRewardPanel;
    public GameObject StoryPanelEnd;

    public bool useTimeLimit = true;
    public bool isStoryDungeon = true;

    public List<GameObject> winPanels; // List of win panels
    public float firstPanelBias = 0.6f; // Chance of showing the first panel

    public StoryLockController storyLockController;

    public int dungeonIndex;
    public string itemClaimedKey = "RewardClaimed";

    public float spawnInterval = 2f;

    public SFXManager sfxManager;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        goldRewardPanel.SetActive(false);

        // Hide challengePanel only if it is a story dungeon
        if (isStoryDungeon)
        {
            challengePanel.SetActive(false);
        }

        // Ensure all win panels are hidden initially
        foreach (var panel in winPanels)
        {
            panel.SetActive(false);
        }
    }

    void StartGame()
    {
        challengePanel.SetActive(false);
        gameActive = true;
        enemiesDefeated = 0;
        enemiesSpawned = 0;
        timeRemaining = timeLimit;
        UpdateUI();
        StartCoroutine(SpawnEnemies());
    }

    void Update()
    {
        if (gameActive)
        {
            if (useTimeLimit)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    EndGame(false);
                }
            }

            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (useTimeLimit)
        {
            timerText.text = Mathf.Ceil(timeRemaining).ToString();
        }
        else
        {
            timerText.text = "";
        }

        enemiesDefeatedText.text = enemiesDefeated + "/" + totalEnemiesToDefeat;
        int remainingEnemies = totalEnemiesToDefeat - enemiesDefeated;
        enemiesRemainingText.text = "Remaining: " + remainingEnemies;
    }

    public void OnEnemyDefeated()
    {
        enemiesDefeated++;
        UpdateUI();

        if (enemiesDefeated >= totalEnemiesToDefeat)
        {
            EndGame(true);
        }
    }

    void EndGame(bool won)
    {
        gameActive = false;

        if (won)
        {
            storyLockController.SetStoryCompletionStatus(dungeonIndex, true);

            int claimedStatus = PlayerPrefs.GetInt(itemClaimedKey, 0);
            if (claimedStatus == 1)
            {
                Debug.Log("Opening Gold Reward Panel");
                if (isStoryDungeon)
                {

                    StoryPanelEnd.SetActive(true);
                }
                goldRewardPanel.SetActive(true);
                exitDungeon.SetActive(true);
            }
            else
            {
                Debug.Log("Randomizing and opening a Win Panel");
                if (isStoryDungeon)
                {

                    StoryPanelEnd.SetActive(true);
                }
                exitDungeon.SetActive(true);

                // Randomize the win panel with bias
                GameObject selectedPanel = SelectRandomWinPanel();
                selectedPanel.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Game over! Time's up or enemies not defeated.");
        }
    }

    GameObject SelectRandomWinPanel()
    {
        if (winPanels.Count == 0) return null;

        // Randomize with bias towards the first panel
        float randomValue = Random.value; // Value between 0 and 1
        if (randomValue < firstPanelBias)
        {
            return winPanels[0]; // Higher chance of selecting the first panel
        }
        else
        {
            // Select a random panel from the rest of the list
            int randomIndex = Random.Range(1, winPanels.Count);
            return winPanels[randomIndex];
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (gameActive && enemiesSpawned < totalEnemiesToDefeat)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            GameObject enemy = Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);

            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null && sfxManager != null)
            {
                enemyScript.SFXManager = sfxManager;
            }

            enemiesSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            challengePanel.SetActive(true);
        }
    }
}
