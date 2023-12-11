using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] characterPrefabs;
    public Transform[] playerPos;

    public List<Player> playerList = new List<Player>();
    public List<GameObject> playerObjectList = new List<GameObject>();
    public Text timer_TXT;

    private List<string> feed_String = new List<string>();
    public Text feed_TXT;

    public PlayerUI[] UI_Scripts;

    public static GameManager instance = null;
    protected void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    protected void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            StartRound();
        }
    }

    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnPlayerDeath;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnPlayerDeath;
    }
    
    public void AddPlayer(Player playerToAdd, GameObject playerGameObject)
    {
        playerList.Add(playerToAdd);
        playerObjectList.Add(playerGameObject);
        UI_Scripts[playerToAdd.ActorNumber - 1].gameObject.SetActive(true);
    }

    public void StartRound()
    {
        object playerSelectionNumber;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER,
                out playerSelectionNumber))
        {

            int actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
            Vector3 pos = playerPos[actorNum - 1].position;
            pos.z = 1f;
            PhotonNetwork.Instantiate(characterPrefabs[(int) playerSelectionNumber].name, pos, Quaternion.identity);
        }
    }

    void OnPlayerDeath(EventData photonEvent)
    {
        if (photonEvent.Code != (byte) Constants.RaiseEventCode.PlayerDeath) return;
        
        object[] data = (object[]) photonEvent.CustomData;

        Player player = (Player) data[0];
        Player killer = (Player) data[1];
        int viewID = (int) data[2];

        UpdateKillFeed(killer.NickName, player.NickName);

        playerList.Remove(player);

        if (playerList.Count != 1) return;
        
        Player winner = playerList[0];
        winner.AddScore(1);
        
        if (winner.GetScore() >= 2)
        {
            Debug.Log($"{winner.NickName} wins!");
            StartCoroutine(EndGame(winner));
        }
        else
        {
            StartCoroutine(waitForNextRound());
        }
    }

    IEnumerator waitForNextRound()
    {
        
        playerList = new List<Player>();
        playerObjectList = new List<GameObject>();
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(1f);
        }
        
        if(PhotonNetwork.IsMasterClient)
        {
            object[] data = new object[] {};
        
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = false
            };
        
            PhotonNetwork.RaiseEvent((byte) Constants.RaiseEventCode.PlayerRespawn, data, raiseEventOptions, sendOptions);
        }
        
        //if(PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("Whiteout");
        playerList = new List<Player>();
        playerObjectList = new List<GameObject>();
        
        feed_String.Clear();
        feed_TXT.text = "";
        //StartRound();
    }

    public Vector3 GetRespectivePosition(int actorNum)
    {
        Vector3 pos = playerPos[actorNum - 1].position;
        pos.z = 1f;

        return pos;
    }
    
    IEnumerator EndGame(Player WinnerPlayer)
    {
        UI_Scripts[WinnerPlayer.ActorNumber - 1].UpdateWinsIndicator(3);
        timer_TXT.gameObject.SetActive(true);
        float timer = 5f;
        while (timer >= 0)
        {
            timer_TXT.text =  $"{WinnerPlayer.NickName} Wins!";
            timer_TXT.text +=  "\n Returning to lobby in " + timer;
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
    
    public void UpdateKillFeed(string killerName, string victimName)
    {
        feed_TXT.text = "";
        if (feed_String.Count >= 3)
        {
            feed_String.RemoveAt(0);
        }

        feed_String.Add(killerName == victimName
            ? $"{victimName} committed suicide"
            : $"{killerName} killed {victimName}");

        foreach (var feed in feed_String)
        {
            feed_TXT.text += feed + "\n";
        }
    }
}
