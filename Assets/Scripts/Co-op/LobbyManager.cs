using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public GameObject loadingPanel;

    public TextMeshProUGUI roomNameText;

    public RoomItem roomItemPrefab;

    List<RoomItem> roomItems = new List<RoomItem>();

    public Transform contentObject;

    public float timeBetweenRoomUpdates = 1.5f;

    float nextUpdateTime;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>();

    public PlayerItem playerItemPrefab;

    public Transform playerItemParent;

    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            StartCoroutine(CheckAndJoinRoom(roomName));
        }
    }

    private IEnumerator CheckAndJoinRoom(string roomName)
    {
        loadingPanel.SetActive(true);

        while (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Waiting for client to be connected and ready...");
            yield return null;
        }

        PhotonNetwork.JoinRoom(roomName);
        loadingPanel.SetActive(false);
    }

    public void OnClickCreate()
    {
        string roomName = GenerateRandomRoomName(6);
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime)
        {
            Debug.Log("OnRoomListUpdate called with " + roomList.Count + " rooms.");
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenRoomUpdates;
        }
    }

    void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomItem item in roomItems)
        {
            Destroy(item.gameObject);
        }
        roomItems.Clear();

        foreach (RoomInfo info in roomList)
        {
            Debug.Log("Adding room: " + info.Name);
            RoomItem item = Instantiate(roomItemPrefab, contentObject);
            item.SetRoomName(info.Name);
            roomItems.Add(item);
        }
    }

    private string GenerateRandomRoomName(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        StringBuilder result = new StringBuilder(length);
        System.Random random = new System.Random();
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }
        return result.ToString();
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
    }

    public void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    void UpdatePlayerList()
{
    foreach (PlayerItem item in playerItemsList)
    {
        Destroy(item.gameObject);
    }
    playerItemsList.Clear();

    if (PhotonNetwork.CurrentRoom == null)
    {
        return;
    }

    foreach (KeyValuePair<int, Photon.Realtime.Player> player in PhotonNetwork.CurrentRoom.Players)
    {
        PlayerItem item = Instantiate(playerItemPrefab, playerItemParent);
        playerItemsList.Add(item);
    }
}

}