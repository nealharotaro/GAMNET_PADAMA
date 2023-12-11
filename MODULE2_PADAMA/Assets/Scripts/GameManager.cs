using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager Instance;
    public GameObject player_Prefab;
    public List<GameObject> spawnList;
    public List<GameObject> staticSpawnList = new List<GameObject>();
    public KillFeed KillFeed;

    void Photon.Pun.IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
    
    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else Instance = this;
    }

    void Start()
    {
        spawnList = GameObject.FindGameObjectsWithTag("SpawnPoints").ToList();
        staticSpawnList.AddRange(spawnList);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int i = UnityEngine.Random.Range(0, spawnList.Count - 1);

            Vector3 loc = spawnList[i].transform.position;
            
            GameObject player = PhotonNetwork.Instantiate(player_Prefab.name, loc, Quaternion.identity);
            
            photonView.RPC("RemoveLocation", RpcTarget.AllBuffered, i);
        }
    }

    public Transform GetRandomLocation()
    {
        int i = UnityEngine.Random.Range(0, spawnList.Count - 1);

        return staticSpawnList[i].transform;
    }

    Vector3 GetRandomSpawn()
    {
        int i = UnityEngine.Random.Range(0, spawnList.Count - 1);

        Vector3 loc = spawnList[i].transform.position;
        
        photonView.RPC("RemoveLocation", RpcTarget.AllBuffered, i);

        return loc;
    }

    [PunRPC]
    void RemoveLocation(int i)
    {
        spawnList.RemoveAt(i);
    }
    
    public void ReturnToLobby()
    {
        StartCoroutine (DoSwitchLevel("LobbyScene"));
    }

    IEnumerator DoSwitchLevel(string level)
    {
        PhotonNetwork.Disconnect ();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        Application.LoadLevel(level);
    }
}
