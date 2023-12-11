using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public InputField PlayerNameInput;

    [Header("Connecting Info Panel")]
    public GameObject ConnectingInfoUIPanel;

    [Header("Creating Room Info Panel")]
    public GameObject CreatingRoomInfoUIPanel;

    [Header("GameOptions  Panel")]
    public GameObject GameOptionsUIPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomUIPanel;
    public InputField RoomNameInputField;
    public string mapMode;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomUIPanel;

    public Text RoomInfo_TXT;
    public GameObject playerListPrefab;
    public GameObject playerListParent;
    public GameObject Start_BTN;
    public Text GameModeText;
   
    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomUIPanel;

    private Dictionary<int, GameObject> playerListGameObjects;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(LoginUIPanel.name);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI Callback Methods
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(ConnectingInfoUIPanel.name);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("PlayerName is invalid!");
        }
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }
    
    public void OnCreateRoomButtonClicked()
    {
        ActivatePanel(CreatingRoomInfoUIPanel.name);
        if(mapMode ==  null) return;
        
        string roomName = RoomNameInputField.text;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 3;  
        string[] roomPropertiesInLobby = {"gm"};

        
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", mapMode}};
        
        roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnJoinRandomRoomClicked(string gameMode)
    {
        mapMode = gameMode;
        
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties =
            new ExitGames.Client.Photon.Hashtable() {{"gm", gameMode}};

        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnBackBtnClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public void OnLeaveBtnClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameBtnClicked()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("void"))
            {
                Debug.Log("Map is void");
                PhotonNetwork.LoadLevel("Void");
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("white"))
            {
                Debug.Log("Map is white");
                PhotonNetwork.LoadLevel("Whiteout");
            }
        }
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+ " is connected to Photon");
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"{PhotonNetwork.CurrentRoom} has been created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} has joines the {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"Player Count: {PhotonNetwork.CurrentRoom.PlayerCount}");
        ActivatePanel(InsideRoomUIPanel.name);
        object gameModeName;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm", out gameModeName)) ;
        {
            if(string.IsNullOrEmpty(gameModeName.ToString())) return;
            Debug.Log(gameModeName.ToString());
            RoomInfo_TXT.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name} " +
                                $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("void"))
            {
                GameModeText.text = "MAP: VOID";
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("white"))
            {
                GameModeText.text = "MAP: WHITE OUT";
            }
        }
        
        if (playerListGameObjects is null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListItem = Instantiate(playerListPrefab);
            playerListItem.transform.SetParent(playerListParent.transform);
            playerListItem.transform.localScale = Vector3.one;
            playerListItem.GetComponent<PlayerListItemInitializer>().Initialize(player.ActorNumber,player.NickName);

            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                 playerListItem.GetComponent<PlayerListItemInitializer>().SetPlayerReady((bool) isPlayerReady);
            }
            playerListGameObjects.Add(player.ActorNumber,playerListItem);
        }
        Start_BTN.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        
        if(mapMode ==  null) return;
        
        string roomName = RoomNameInputField.text;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 3;
        string[] roomPropertiesInLobby = {"gm"};

        
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", mapMode}};
        
        roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
        roomOptions.CustomRoomProperties = customRoomProperties;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerListItem = Instantiate(playerListPrefab);                                                
        playerListItem.transform.SetParent(playerListParent.transform);                                           
        playerListItem.transform.localScale = Vector3.one;                                                        
        playerListItem.GetComponent<PlayerListItemInitializer>().Initialize(newPlayer.ActorNumber,newPlayer.NickName);  
                                                                                                                      
        playerListGameObjects.Add(newPlayer.ActorNumber,playerListItem);  
        
        RoomInfo_TXT.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name} " +                                   
                            $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        
        Start_BTN.SetActive(CheckAllPlayerReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
        
        RoomInfo_TXT.text = $"Room Name: {PhotonNetwork.CurrentRoom.Name} " +                                   
                            $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";  
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptionsUIPanel.name);

        foreach (GameObject playerListGameObject in playerListGameObjects.Values)
        {
            Destroy(playerListGameObject);
        }
        
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject playerListGameObject;
        if (playerListGameObjects.TryGetValue(targetPlayer.ActorNumber, out playerListGameObject))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerListGameObject.GetComponent<PlayerListItemInitializer>().SetPlayerReady((bool) isPlayerReady);
            }
        }
        
        //Debug.Log("Num of player: " + PhotonNetwork.PlayerList.Length);
        
        if(PhotonNetwork.PlayerList.Length > 1) Start_BTN.SetActive(CheckAllPlayerReady());
        else Start_BTN.SetActive(false);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            Start_BTN.SetActive(CheckAllPlayerReady());
        }
    }
    #endregion
    
    #region Public Methods
    public void ActivatePanel(string panelNameToBeActivated)
    {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
        ConnectingInfoUIPanel.SetActive(ConnectingInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreatingRoomInfoUIPanel.SetActive(CreatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelNameToBeActivated));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        JoinRandomRoomUIPanel.SetActive(JoinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));

    }

    public void SetMap(string gameMode)
    {
        mapMode = gameMode;
    }
    #endregion

    #region Private Methods

    private bool CheckAllPlayerReady()
    {
        if (!PhotonNetwork.IsMasterClient) return false;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;

            if (p.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool) isPlayerReady) return false;
            }
            else return false;
        }

        return true;
    }

    #endregion
}

