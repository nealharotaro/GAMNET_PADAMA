using System;
using System.Collections;
using System.Collections.Generic;
using MiscUtil.Extensions.TimeRelated;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
public class VehicleCombat : MonoBehaviourPunCallbacks
{
    public Transform turretMuzzle;
    public Transform rocketMuzzle;

    public GameObject laser_prefab;
    public GameObject missile_Prefab;

    public float turretFireRate = 0.5f;
    public float missileFireRate = 1f;
    private bool canFireLaser = false;
    private bool canFireMissile = false;
    public bool enableCombat = false;
    void Start()
    {
        canFireLaser = true;
        canFireMissile = true;
    }

    private void Update()
    {
        if(!enableCombat) return;
        
        if (Input.GetButtonDown("Fire1"))
        {
            if(canFireLaser) StartCoroutine(FireLaser());
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (canFireMissile) StartCoroutine(FireMissile());
            //photonView.RPC("FireMissileImplementation", RpcTarget.AllBuffered);
        }
    }

    IEnumerator FireLaser()
    {
        if (!canFireLaser) yield break; 
        
        FireLaserImplementation();
        
        canFireLaser = false;
        float timer = turretFireRate;
        while (timer > 0)
        {
            canFireLaser = false;
            yield return new WaitForSeconds(0.1f);
            timer--;
        }
        canFireLaser = true;
    }
    
    void FireLaserImplementation()
    {
        Ray ray = new Ray(turretMuzzle.position, turretMuzzle.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10);
            }
            
            photonView.RPC("CreateLaserFX", RpcTarget.AllBuffered, hit.point);
        }
        else
        {
            photonView.RPC("CreateLaserFX", RpcTarget.AllBuffered, turretMuzzle.forward * 200);
        }
        canFireLaser = false;
    }

    IEnumerator FireMissile()
    {
        if (!canFireMissile) yield break; 
        
        photonView.RPC("FireMissileImplementation", RpcTarget.AllBuffered);
        
        canFireMissile = false;
        float timer = missileFireRate;
        while (timer > 0)
        {
            canFireMissile = false;
            yield return new WaitForSeconds(0.1f);
            timer--;
        }
        canFireMissile = true;
    }
    
    [PunRPC]
    void FireMissileImplementation()
    {
        //if(!canFireMissile) return;
        GameObject missile = Instantiate(missile_Prefab, rocketMuzzle.position, gameObject.transform.rotation);
        Projectile proj = missile.GetComponent<Projectile>();
        float additionalSpeed = (GetComponent<VehicleMovementScript>().currentSpeed/.2f) * 1500;
        proj.Initialize(this.gameObject, additionalSpeed);

        if(!photonView.IsMine) return;
        proj.OnHit += o =>
        {
            if (o.CompareTag("Player"))
            {
                o.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 20);
            }
        };

    }

    [PunRPC]
    void CreateLaserFX(Vector3 endPos)
    {
        GameObject laser = Instantiate(laser_prefab);
        
        LineRenderer line = laser_prefab.GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.SetPosition(0,turretMuzzle.position);
        line.SetPosition(1,endPos);
        Destroy(laser, 0.1f);
    }
}
