using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class HealthComponent : MonoBehaviourPunCallbacks
{
    [Header("HP Properties")] 
    public float Max_HP = 100f;
    private float current_HP;
    public GameObject HP_OBJ;
    public UnityEngine.UI.Image hpBar;

    private void Start()
    {
        current_HP = Max_HP;
        HP_OBJ.SetActive(true);
    }

    public override void OnDisable()
    {
        
    }

    [PunRPC]
    public void TakeDamage(int dmg, PhotonMessageInfo info)
    {
        //Debug.Log($"Sender: {info.Sender}");
        if(current_HP <= 0) return;
        
        current_HP -= dmg; 
        hpBar.fillAmount = current_HP / Max_HP;
        
        if (current_HP <= 0)
        {
            Die(info);
        }
    }
    
    void Die(PhotonMessageInfo info)
    {
        gameObject.transform.localScale = Vector3.zero;
        
        if(!photonView.IsMine) return;
        
        transform.GetComponent<VehicleCombat>().enabled = false;
        //transform.GetComponent<VehicleMovementScript>().enabled = false;
        transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetComponent<LapController>().enabled = false;
        
        GetComponent<PlayerSetup>().camera.transform.SetParent(null);
        
        
        // Send Event

        int viewID = photonView.ViewID;
        object[] data = new object[] {info.photonView.Owner, info.Sender, viewID};
        Debug.Log($"{info.Sender.NickName} killed {info.photonView.Owner.NickName}");
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };
        
        PhotonNetwork.RaiseEvent((byte) LapController.RaiseEventCode.WhoDiedEventCode, data, raiseEventOptions, sendOptions);
    }
}
