using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Photon server in the Asia region...");
        PhotonNetwork.NickName = playerData.playerName;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia"; // Specify Asia region
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to the Master server in Asia.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby.");
        Debug.Log("Player Name: " + playerData.playerName);
        SceneManager.LoadScene("Lobby");
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon. Reason: " + cause);
    }
}
