using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class HealthComponent : MonoBehaviourPunCallbacks
{
    [SerializeField] private float maxHealth  = 100;

    private float currentHealth;

    public float MaxHP => maxHealth;
    public float currentHP => currentHealth;
    public PlayerUI PlayerUI=> GetComponent<PlayerSetup>()._playerUI;
    private void Start()
    {
        currentHealth = maxHealth;
    }

    [PunRPC]
    public void TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;
        
        PlayerUI.UpdateHealthBar(currentHealth/maxHealth);

        if (currentHealth <= 0)
        {
            Die(info);
        }
    }

    public void Die(PhotonMessageInfo info)
    {
        GetComponent<Animator>().SetTrigger("Death");
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Collider2D>().enabled = false;
        
        if(!photonView.IsMine) return;
        
        int viewID = photonView.ViewID;
        object[] data = new object[] {info.photonView.Owner, info.Sender, viewID};
        
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };
        
        PhotonNetwork.RaiseEvent((byte) Constants.RaiseEventCode.PlayerDeath, data, raiseEventOptions, sendOptions);
    }

    public void RefreshHealth()
    {
        currentHealth = maxHealth;
        PlayerUI.UpdateHealthBar(currentHealth/maxHealth);
    }
}
