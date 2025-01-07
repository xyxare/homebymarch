using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviourPunCallbacks
{
    private int playerCount = 0;
    [SerializeField] private GameObject youWinPanel; // Reference to the "You Win" panel in the Canvas
    [SerializeField] private GameObject gamePanel; // Reference to the game panel in the Canvas
    [SerializeField] private Button exitButton; // Button to exit to the lobby
    [SerializeField] private TextMeshProUGUI playerCountText; // Reference to the player count text
    [SerializeField] private TextMeshProUGUI timerText; // Reference to the timer text
    [SerializeField] private TextMeshProUGUI winTimeText; // Reference to the win time text
    [SerializeField] private Button startTimerButton; // Button to start the timer
    [SerializeField] private float detectionRadius = 5f; // Radius of the sphere for SphereCastAll
    [SerializeField] private LayerMask playerLayer; // Layer mask to detect only players
    [SerializeField] private float countdownDuration = 300f; // Countdown duration in seconds
    [SerializeField] private GameObject detectionCenter; // GameObject where OverlapSphere will be spawned

    private bool timerRunning = false; // Timer state
    private float remainingTime; // Remaining time for countdown

    private int requiredPlayerPairs = 2; // Set the required number of players (can be adjusted)

    void Start()
    {
        if (youWinPanel != null)
        {
            youWinPanel.SetActive(false); // Hide the "You Win" panel at the start
        }
        else
        {
            Debug.LogError("You Win Panel reference is null. Please assign it in the Inspector.");
        }

        if (gamePanel != null)
        {
            gamePanel.SetActive(false); // Hide the game panel at the start
        }
        else
        {
            Debug.LogError("Game Panel reference is null. Please assign it in the Inspector.");
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClick); // Add listener to exit button
        }
        else
        {
            Debug.LogError("Exit Button reference is null. Please assign it in the Inspector.");
        }

        if (playerCountText != null)
        {
            playerCountText.text = $"0/{requiredPlayerPairs}"; // Initialize player count display
        }
        else
        {
            Debug.LogError("Player Count Text reference is null. Please assign it in the Inspector.");
        }

        if (timerText != null)
        {
            timerText.text = "0:00"; // Initialize the stopwatch display
        }
        else
        {
            Debug.LogError("Timer Text reference is null. Please assign it in the Inspector.");
        }

        if (startTimerButton != null)
        {
            startTimerButton.onClick.AddListener(OnStartTimerButtonClick); // Start the timer when button is clicked
            startTimerButton.gameObject.SetActive(PhotonNetwork.IsMasterClient); // Show the button only to the Master Client
        }
        else
        {
            Debug.LogError("Start Timer Button reference is null. Please assign it in the Inspector.");
        }
    }

    void Update()
    {
        // Only the Master Client runs the timer
        if (PhotonNetwork.IsMasterClient)
        {
            DetectPlayers();

            // Handle the countdown if the timer is running
            if (timerRunning)
            {
                remainingTime -= Time.deltaTime; // Decrement the timer
                if (remainingTime <= 0)
                {
                    remainingTime = 0;
                    timerRunning = false;
                    photonView.RPC("ShowWinPanelForAll", RpcTarget.All); // Show win panel for all players
                }
                photonView.RPC("UpdateTimerDisplay", RpcTarget.All, remainingTime); // Update the timer for all players
            }
        }

        // Log and update the player count text
        if (playerCountText != null)
        {
            playerCountText.text = $"{playerCount}/{requiredPlayerPairs}";
        }

        // Trigger win condition if there are 2 players
        if (playerCount >= requiredPlayerPairs)
        {
            photonView.RPC("ShowWinPanelForAll", RpcTarget.All); // Show win panel for all players
        }
    }

    private void DetectPlayers()
    {
        if (detectionCenter == null)
        {
            Debug.LogError("Detection Center reference is null. Please assign it in the Inspector.");
            return;
        }

        // Perform a sphere cast to detect players within the radius
        Collider[] hitColliders = Physics.OverlapSphere(detectionCenter.transform.position, detectionRadius, playerLayer);

        int detectedPlayerCount = 0;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player")) // Check if the object is a player
            {
                detectedPlayerCount++;
            }
        }

        if (detectedPlayerCount != playerCount) // Log only if the count changes
        {
            playerCount = detectedPlayerCount;
            Debug.Log("Total players detected: " + playerCount);
        }
    }

    [PunRPC]
    private void UpdateTimerDisplay(float time)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds); // Format the time as MM:SS
        }
    }

    [PunRPC]
    private void ShowWinPanelForAll()
    {
        if (youWinPanel != null)
        {
            youWinPanel.SetActive(true); // Show the "You Win" panel
        }
        else
        {
            Debug.LogError("You Win Panel reference is not set.");
        }

        if (gamePanel != null)
        {
            gamePanel.SetActive(true); // Show the game panel
        }
        else
        {
            Debug.LogError("Game Panel reference is not set.");
        }

        if (winTimeText != null)
        {
            int minutes = Mathf.FloorToInt(countdownDuration / 60f);
            int seconds = Mathf.FloorToInt(countdownDuration % 60f);
            winTimeText.text = string.Format("Time: {0:D2}:{1:D2}", minutes, seconds); // Show time in Win Panel
        }
    }

    public void OnStartTimerButtonClick()
    {
        if (!timerRunning) // Only start the timer if it's not already running
        {
            timerRunning = true; // Start the timer
            remainingTime = countdownDuration; // Initialize the countdown timer
            Debug.Log("Timer started.");
        }
    }

    public void OnExitButtonClick()
    {
        PhotonNetwork.LeaveRoom(); // Leave the current room
        SceneManager.LoadScene("Main Screen"); // Load the lobby scene
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the sphere in the editor for visualization
        if (detectionCenter != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(detectionCenter.transform.position, detectionRadius);
        }
    }
}