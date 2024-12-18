using UnityEngine;
using Photon.Pun;

public class WinTrigger : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public GameObject youWinPanel; // Reference to the "You Win" panel in the Canvas


    void Start()
    {

        youWinPanel.SetActive(false);
        if (youWinPanel != null)
        {
            youWinPanel.SetActive(false); // Hide the "You Win" panel at the start
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player enters the trigger
        {
            Debug.Log("Player entered the trigger.");
            if (youWinPanel == null)
            {
                Debug.LogError("You Win Panel reference is null. Please assign it in the Inspector.");
                return;
            }

            youWinPanel.SetActive(true); // Show the "You Win" panel
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
    }
}