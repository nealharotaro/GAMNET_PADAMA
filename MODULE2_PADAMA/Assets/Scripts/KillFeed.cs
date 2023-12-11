using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;

public class KillFeed : MonoBehaviourPunCallbacks,IPunObservable
{
    private List<string> feed_String = new List<string>();
    public Text feed_TXT;
    public Text Respawn_TXT;

    void Photon.Pun.IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
    
    
    [PunRPC]
    public void UpdateKillFeedSender(string killerName, string victimName)
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

    [PunRPC]
    public void EndGame(Player winner)
    {
        Respawn_TXT.text = $"{winner.NickName} wins!!";
    }
}
