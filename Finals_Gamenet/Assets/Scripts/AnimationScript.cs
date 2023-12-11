using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AnimationScript : MonoBehaviourPunCallbacks
{
    private Rigidbody2D rb;
    private Animator animator;
    private LandingComponent LandingComponent;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        LandingComponent = GetComponentInChildren<LandingComponent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        int air = 0;
        if (rb.velocity.y > 0.05f && !LandingComponent.isLanded) air = 1;
        else if(rb.velocity.y <= 0f && !LandingComponent.isLanded) air = -1;
        else if (LandingComponent.isLanded) air = 0;
        
        animator.SetInteger("airState",air);
        
        bool isRunning = rb.velocity.x > 0.8 || rb.velocity.x < -0.8f;
        
        animator.SetBool("isRunning", isRunning);
        
        if (rb.velocity.x > 0f) spriteRenderer.flipX = false;
        else if (rb.velocity.x < 0f) spriteRenderer.flipX = true;
    }

    [PunRPC]
    public void AttackAnim(int num)
    {
        //if (!photonView.IsMine) return;
        
        if(num == 1) animator.SetTrigger("Attack1");
        else if(num == 2) animator.SetTrigger("Attack2");
    }
}
