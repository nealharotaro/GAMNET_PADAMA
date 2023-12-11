using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInputManager : MonoBehaviour
{
    public void SetPlayerName(string playerName)
    {
        if(string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Player name is empty!");
            return;
        }

        PhotonNetwork.NickName = playerName;
    }
}
