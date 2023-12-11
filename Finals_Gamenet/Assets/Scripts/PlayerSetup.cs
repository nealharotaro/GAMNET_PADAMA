using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public PlayerUI _playerUI;

    public string characterName;
    public GameObject playerIndicator;
    private void Awake()
    {
        _playerUI = GameManager.instance.UI_Scripts[photonView.Owner.ActorNumber - 1];
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.AddPlayer(photonView.Owner, this.gameObject);
        GetComponent<PlayerMovement>().enabled = photonView.IsMine;
        
        _playerUI = GameManager.instance.UI_Scripts[photonView.Owner.ActorNumber - 1];
        _playerUI.SetName(photonView.Owner.NickName, characterName);
        playerIndicator.SetActive(photonView.IsMine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += OnRestartRound;
    }

    public void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnRestartRound;
    }

    void OnRestartRound(EventData photonEvent)
    {
        if (photonEvent.Code != (byte) Constants.RaiseEventCode.PlayerRespawn) return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.gravityScale = 1;
        rb.velocity = Vector2.zero;
        rb.MovePosition(GameManager.instance.GetRespectivePosition(photonView.Owner.ActorNumber));
        transform.position = GameManager.instance.GetRespectivePosition(photonView.Owner.ActorNumber);
        
        
        GetComponent<Animator>().SetTrigger("RestartRound");
        GetComponent<PlayerMovement>().enabled = photonView.IsMine;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<HealthComponent>().RefreshHealth();
        
        if (GetComponent<VoidCollision>().enabled)
        {
            GetComponent<VoidCollision>().hasEntered = false;
        }

        int score = photonView.Owner.GetScore();
        
        _playerUI.UpdateWinsIndicator(score);

        GameManager.instance.AddPlayer(photonView.Owner, this.gameObject);
        GetComponent<CountdownManager>().enabled = true;
    }
}
