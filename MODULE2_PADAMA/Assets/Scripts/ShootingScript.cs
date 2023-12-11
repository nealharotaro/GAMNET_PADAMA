using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class ShootingScript : MonoBehaviourPunCallbacks
{
    public Camera camera;

    public GameObject HitEffect_Prefab;

    [Header("HP Properties")] 
    public float startHP;
    private float currentHP;
    public UnityEngine.UI.Image hpBar;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        currentHP = startHP;
        hpBar.fillAmount = currentHP / startHP;
    }

    public void Fire()
    {
        //RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);
            
            photonView.RPC("CreateHitFX", RpcTarget.All, hit.point);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int dmg, PhotonMessageInfo info)
    {
        if(currentHP <= 0) return;
        
        currentHP -= dmg; 
        hpBar.fillAmount = currentHP / startHP;
        
        if (currentHP <= 0)
        {
            Die(info);
            Debug.Log($"{info.Sender.NickName} killed {info.photonView.Owner.NickName}");
        }
    }

    [PunRPC]
    public void CreateHitFX(Vector3 pos)
    {
        GameObject hitEffectObject = Instantiate(HitEffect_Prefab, pos, Quaternion.identity);
        Destroy(hitEffectObject, 0.2f);
    }

    void Die(PhotonMessageInfo info)
    {
        if (photonView.IsMine)
        {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
        
        GameManager.Instance.KillFeed.UpdateKillFeedSender(info.Sender.NickName,photonView.Owner.NickName);
        
        info.Sender.AddScore(1);
        
        if (info.Sender.GetScore() >= 2)
        {
            StopCoroutine(RespawnCountdown());

            StartCoroutine(EndGameCountDown(info.Sender.NickName));
        }
    }

    IEnumerator RespawnCountdown()
    {
        GameObject respawn_TXT = GameObject.Find("Respawn_TXT");
        
        float RespawnTime = 5f;

        while (RespawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            RespawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawn_TXT.GetComponent<UnityEngine.UI.Text>().text =
                $"You are killed. Respawning in {RespawnTime:.00}";
        }

        animator.SetBool("isDead", false);
        respawn_TXT.GetComponent<UnityEngine.UI.Text>().text = "";


        this.transform.position = GameManager.Instance.GetRandomLocation().position;
        transform.GetComponent<PlayerMovementController>().enabled = true;
        
        photonView.RPC("regainHP", RpcTarget.AllBuffered);
    }

    IEnumerator EndGameCountDown(string winnerName)
    {
        GameObject respawn_TXT = GameObject.Find("Respawn_TXT");
        
        
        float RespawnTime = 5f;

        while (RespawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            RespawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawn_TXT.GetComponent<UnityEngine.UI.Text>().text =
                $"{winnerName} wins! Disconnection in {RespawnTime}";
        }
        
        GameManager.Instance.ReturnToLobby();
    }

    [PunRPC]
    public void regainHP()
    {
        currentHP = startHP;
        hpBar.fillAmount = currentHP / startHP;
    }
}
