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
        Debug.Log("Connecting to Photon server...");
        PhotonNetwork.NickName = playerData.playerName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master server.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby.");
        Debug.Log("Player Name: " + playerData.playerName);
        SceneManager.LoadScene("Lobby");
    }

    // Update is called once per frame
}