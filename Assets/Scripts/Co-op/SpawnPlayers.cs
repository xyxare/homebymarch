using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using HomeByMarch;


public class SpawnPlayers : MonoBehaviour
{
    public FixedJoystick joystickPrefab; // Reference to the joystick prefab
    public GameObject playerPrefab; // Player prefab to spawn
    public Camera mainCamera; // Reference to the main camera
    public CinemachineVirtualCamera cinemachineCamera; // Reference to the Cinemachine camera
    public Canvas uiCanvas; // Reference to the canvas where the joystick should be instantiated
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    private FixedJoystick instantiatedJoystick;

    // Start is called before the first frame update

    void Start()
    {
        // Generate a random position within the defined bounds
        Vector3 randomPosition = new Vector3(
            Random.Range(minX, maxX),
            41.24628f, // Assuming ground level
            Random.Range(minZ, maxZ)
        );

        // Instantiate the player using PhotonNetwork
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);

        // Ensure the PlayerController is found
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Enable the PlayerController if it's disabled
            if (!playerController.isActiveAndEnabled)
            {
                playerController.enabled = true; // Enable the PlayerController component
            }

            // Instantiate the joystick prefab inside the UI Canvas
            if (joystickPrefab != null && uiCanvas != null)
            {
                instantiatedJoystick = Instantiate(joystickPrefab, uiCanvas.transform);
                RectTransform joystickRect = instantiatedJoystick.GetComponent<RectTransform>();
                if (joystickRect != null)
                {
                    joystickRect.anchoredPosition = new Vector2(245, 246); // Adjust based on your UI layout
                }
                else
                {
                    Debug.LogError("RectTransform not found on joystick prefab.");
                }
                playerController.joystick = instantiatedJoystick; // Assign joystick to PlayerController
            }
            else
            {
                Debug.LogError("Joystick prefab or UI Canvas reference not set.");
            }

            // Assign the Cinemachine camera follow
            if (cinemachineCamera != null)
            {
                cinemachineCamera.Follow = player.transform;
                cinemachineCamera.LookAt = player.transform;
            }
            else
            {
                Debug.LogError("CinemachineVirtualCamera reference not set.");
            }
        }
        else
        {
            Debug.LogError("PlayerController component not found on the instantiated player.");
        }
    }

}
