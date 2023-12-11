using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject fps_Model;
    public GameObject nonFPS_Model;

    public GameObject playerUI_Prefab;

    public PlayerMovementController controller;
    public Camera fpsCamera;

    private Animator animator;
    public Avatar fps_Avatar;
    public Avatar thirdPerson_Avatar;
    public GameObject playerUI;
    private ShootingScript shooting;
    public TMPro.TMP_Text playerName_TXT;
    
    void Start()
    {
        controller = this.GetComponent<PlayerMovementController>();
        shooting = GetComponent<ShootingScript>();
        fps_Model.SetActive(photonView.IsMine);
        nonFPS_Model.SetActive(!photonView.IsMine);

        if (photonView.IsMine)
        {
            playerUI = Instantiate(playerUI_Prefab);
            controller.touchField = playerUI.transform.Find("RotationTouchField").GetComponent<FixedTouchField>();
            controller.Joystick = playerUI.transform.Find("Fixed Joystick").GetComponent<Joystick>();
            fpsCamera.enabled = true;

            //GameManager.Instance.KillFeed = playerUI.transform.Find("KillFeed").GetComponent<KillFeed>();
            playerUI.transform.Find("Fire_Btn").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => shooting.Fire());
        }
        else
        {
            controller.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            fpsCamera.enabled = false;
        }
        
        animator = GetComponent<Animator>();
        animator.SetBool("isLocalPlayer", photonView.IsMine);

        animator.avatar = (photonView.IsMine) ? fps_Avatar : thirdPerson_Avatar;

        playerName_TXT.text = photonView.Owner.NickName;
    }
    
    void Update()
    {
        
    }
}
