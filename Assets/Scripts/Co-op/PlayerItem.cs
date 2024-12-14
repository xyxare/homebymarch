using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro
using Photon.Realtime; // Ensure the correct Player class is used

public class PlayerItem : MonoBehaviour
{
    public TextMeshProUGUI playerName; // Use TextMeshProUGUI for TextMeshPro

    public void SetPlayerInfo(Photon.Realtime.Player _Player)
    {
        playerName.text = _Player.NickName; // Access the NickName property
    }
}
