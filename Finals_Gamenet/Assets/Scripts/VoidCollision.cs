using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class VoidCollision : MonoBehaviourPunCallbacks
{
    public bool hasEntered = false;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!photonView.IsMine) return;
        if(hasEntered) return;
        if(col.gameObject.CompareTag("Void"))
        {
            Debug.Log(photonView.Owner.NickName + " SUICIDE!");
            photonView.RPC("TakeDamage", RpcTarget.AllBuffered, 999.0f);
            hasEntered = true;
        }
    }
}
