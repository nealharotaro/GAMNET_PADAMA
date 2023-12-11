using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class DeathRaceManager : RacingGameManager
{
    private List<string> feed_String = new List<string>();
    public Text feed_TXT;
    public Text winner_TXT;
    public List<Photon.Realtime.Player> players  = new List<Player>();

    void Awake()
    {
        base.Awake();

    }
    
    void Start()
    {
        base.Start();
        winner_TXT.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnPlayerDeath;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnPlayerDeath;
    }


    public void UpdateKillFeed(string killerName, string victimName)
    {
        feed_TXT.text = "";
        if (feed_String.Count >= 3)
        {
            feed_String.RemoveAt(0);
        }

        feed_String.Add($"{killerName} killed {victimName}");

        foreach (var feed in feed_String)
        {
            feed_TXT.text += feed + "\n";
        }
    }

    void OnPlayerDeath(EventData photonEvent)
    {
        if (photonEvent.Code == (byte) LapController.RaiseEventCode.WhoDiedEventCode)
        {
            object[] data = (object[]) photonEvent.CustomData;

            Player player = (Player)data[0];
            Player killer = (Player)data[1];
            int viewID = (int) data[2];
            
            UpdateKillFeed(killer.NickName, player.NickName);
            Debug.Log($"{killer.NickName} killed {player.NickName}");

            players.Remove(player);
            if (players.Count == 1)
            {
                if (players[0] == null)
                {
                    Debug.LogError("Something is wrong");
                    return;
                }
                //EndGameLastManStanding(players[0]);
                StartCoroutine(EndGameLastManStanding(players[0].NickName));
            }
        }
    }

    IEnumerator EndGameLastManStanding(string winnerName)
    {
        winner_TXT.gameObject.SetActive(true);
        float timer = 5f;
        while (timer >= 0)
        {
            winner_TXT.text =  $"{winnerName} Wins!";
            winner_TXT.text +=  "\n Returning to lobby in " + timer;
            yield return new WaitForSeconds(1.0f);
            timer--;
        }
        
        PhotonNetwork.Disconnect ();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        Application.LoadLevel("LobbyScene");
    }
}
