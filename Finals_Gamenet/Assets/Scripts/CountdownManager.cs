using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
public class CountdownManager : MonoBehaviourPunCallbacks
{
    public Text timerTXT;

    public float timeToStartGame = 3.00f;
    // Start is called before the first frame update
    void Start()
    {
        timerTXT = GameManager.instance.timer_TXT;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        timeToStartGame = 3.00f;
        GetComponent<PlayerMovement>().canMove = false;
        timerTXT = GameManager.instance.timer_TXT;
        if(!PhotonNetwork.IsMasterClient) return;
        StartCoroutine(OnStartRound());
    }

    private void Update()
    {
        // if (!PhotonNetwork.IsMasterClient) return;
        // timerTXT ??= GameManager.instance.timer_TXT;
        //
        // if (timeToStartGame > 0)
        // {
        //     timeToStartGame -= Time.deltaTime;
        //     photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartGame);
        // }
        // else if (timeToStartGame < 0)
        // {
        //     photonView.RPC("StartGame", RpcTarget.AllBuffered);
        // }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        timerTXT ??= GameManager.instance.timer_TXT;
        timerTXT.text = time > 0 ? time.ToString("F1") : "";
    }

    [PunRPC]
    public void StartGame()
    {
        GetComponent<PlayerMovement>().canMove = true;
        timerTXT.text = "";
        this.enabled = false;
    }

    [PunRPC]
    public void FightText()
    {
        timerTXT ??= GameManager.instance.timer_TXT;
        timerTXT.text = "FIGHT!";
    }
    IEnumerator OnStartRound()
    {
        timerTXT ??= GameManager.instance.timer_TXT;
        while (timeToStartGame > 0)
        {
            timeToStartGame -= Time.deltaTime;
            photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartGame);
            yield return new WaitForFixedUpdate();
        }
        
        photonView.RPC("FightText", RpcTarget.AllBuffered);
        
        yield return new WaitForSeconds(1f);
        
        photonView.RPC("StartGame", RpcTarget.AllBuffered);
    }
}
