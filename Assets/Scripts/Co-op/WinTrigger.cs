using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class WinTrigger : MonoBehaviourPunCallbacks
{
    private int playerCount = 0;
    [SerializeField] private GameObject youWinPanel; // Reference to the "You Win" panel in the Canvas
    [SerializeField] private TextMeshProUGUI playerCountText; // Reference to the player count text
    [SerializeField] private TextMeshProUGUI timerText; // Reference to the timer text
    [SerializeField] private TextMeshProUGUI winTimeText; // Reference to the win time text
    [SerializeField] private Button startTimerButton; // Button to start the timer
    [SerializeField] private float detectionRadius = 5f; // Radius of the sphere for SphereCastAll
    [SerializeField] private LayerMask playerLayer; // Layer mask to detect only players

    private bool timerRunning = false; // Timer state
    private float elapsedTime = 0f; // Elapsed time for stopwatch

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

            // Handle the stopwatch if the timer is running
            if (timerRunning)
            {
                elapsedTime += Time.deltaTime; // Increment the timer
                photonView.RPC("UpdateTimerDisplay", RpcTarget.All, elapsedTime); // Update the timer for all players
            }
        }

        // Log and update the player count text
        if (playerCountText != null)
        {
            playerCountText.text = $"{playerCount}/{requiredPlayerPairs}";
        }
    }

    private void DetectPlayers()
    {
        // Perform a sphere cast to detect players within the radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

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

        if (winTimeText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            winTimeText.text = string.Format("Time: {0:D2}:{1:D2}", minutes, seconds); // Show time in Win Panel
        }
    }

    public void OnStartTimerButtonClick()
    {
        if (!timerRunning) // Only start the timer if it's not already running
        {
            timerRunning = true; // Start the timer
            Debug.Log("Timer started.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the sphere in the editor for visualization
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
