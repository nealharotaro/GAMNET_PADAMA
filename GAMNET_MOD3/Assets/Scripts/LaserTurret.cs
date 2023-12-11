using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LaserTurret : Shooting
{
    LineRenderer laserLineRenderer;

    public float laserWidth = 0.1f;
    public float laserMaxLength = 25f;

    public GameObject laserPrefab;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //For Laser
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.SetWidth(laserWidth, laserWidth);


    }

    // Update is called once per frame
    void Update()
    {

    }

   /* protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isControlEnabled)
        {
            if (fireTimer < fireRate)
            {
                fireTimer += Time.deltaTime;
            }

            if (Input.GetButton("Fire1") && fireTimer > fireRate)
            {
                Debug.Log("Firing");
                Fire();
                fireTimer = 0;
                laserLineRenderer.enabled = photonView.IsMine;
            }
            else
            {
                laserLineRenderer.enabled = false;
            }
        }
    }*/

   /* public override void Fire()
    {
        base.Fire();

        //Put this line of code to an inherited script
        Ray ray = new Ray(turretNozzle.position, transform.forward);
        RaycastHit hit;
        Vector3 endPosition = turretNozzle.position + (laserMaxLength * transform.forward);

        if (photonView.IsMine)
        {
            photonView.RPC("DisplayLaser", RpcTarget.All, turretNozzle.position, endPosition);
        }

        if (Physics.Raycast(ray, out hit, laserMaxLength))
        {
            Debug.Log(hit.collider.gameObject.name);
            endPosition = hit.point;


            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 2);

            }
        }

        laserLineRenderer.SetPosition(0, turretNozzle.position);
        laserLineRenderer.SetPosition(1, endPosition);

        //photonView.RPC("createFireEffects", RpcTarget.AllBuffered, turretNozzle.position, endPosition);

    }*/

    [PunRPC]
    public void DisplayLaser(Vector3 turretNozzle, Vector3 laserEndPosition)
    {

        GameObject laserGameObject = Instantiate(laserPrefab, turretNozzle, Quaternion.identity);
        laserGameObject.gameObject.GetComponent<LineRenderer>().SetPosition(0, turretNozzle);
        laserGameObject.gameObject.GetComponent<LineRenderer>().SetPosition(1, laserEndPosition);
        Destroy(laserGameObject, 0.1f);

    }

}
