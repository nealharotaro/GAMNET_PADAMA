using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    private Rigidbody2D rb;

    public float moveSpeed = 10;
    public float jumpForce = 200;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    public LandingComponent LandingComponent;

    public Animator animator;

    private SpriteRenderer spriteRenderer;

    private bool isAttacking = false;

    public Transform[] dir;

    public float damage = 20;

    public LayerMask layers;

    public bool canMove = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!canMove) return;
        
        float x = Input.GetAxis("Horizontal");
        rb.velocity = (isAttacking) ? new Vector2(0,0) : new Vector2(x * moveSpeed, rb.velocity.y);
        
        if(isAttacking || !LandingComponent.isLanded) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += Vector2.up * jumpForce;
        }
        
        // attack1
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(AtkCooldown("Attack1"));
            GetComponent<PhotonView>().RPC("AttackAnim", RpcTarget.AllBuffered, 1);
            //GetComponent<AnimationScript>().AttackAnim(1);
        }
        //attack2
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(AtkCooldown("Attack2"));
            //GetComponent<AnimationScript>().AttackAnim(2);
            GetComponent<PhotonView>().RPC("AttackAnim", RpcTarget.AllBuffered, 2);
        }
    }

    private void FixedUpdate()
    {
        BetterJumping();
    }

    void BetterJumping()
    {
        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }else if(rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    IEnumerator AtkCooldown(string animName)
    {
        //Wait until we enter the current state
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            yield return null;
        }
 
        isAttacking = true;
        //Now, Wait until the current state is done playing
        while ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) % 1 < 0.99f)
        {
            yield return null;
        }
        
        //Debug.Log("Done with " + animName);
        isAttacking = false;
    }

    public void DamageOpponent(int i)
    {
        if(!photonView.IsMine) return;
        
        float dmg = i == 0 ? damage : damage * 1.5f;
        float rad = i == 0 ? 0.5f : 0.8f;
        Transform origin = !spriteRenderer.flipX ? dir[0] : dir[1];

        Collider2D[] cols = Physics2D.OverlapCircleAll(origin.position, rad);

        foreach (Collider2D c in cols)
        {
            if(c.gameObject==this.gameObject) continue;

            if (c.gameObject.TryGetComponent(out HealthComponent healthComponent))
            {
                if (c.gameObject.TryGetComponent(out PhotonView view))
                {
                    view.RPC("TakeDamage", RpcTarget.AllBuffered, dmg);
                }
                //healthComponent.TakeDamage(dmg);
            }
        }
    }
    
}
