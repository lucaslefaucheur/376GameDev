using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyController : NetworkBehaviour {
    
    private Transform Target;
    private Transform loc;
    private LayerMask caster;
    private float FollowRange = 100;

    private Animator anim;
    private SpriteRenderer rendy;

    float FollowSpeed;

    void Start()
    {
        loc = transform;
        rendy = GetComponent<SpriteRenderer>();
        caster = 1 << LayerMask.NameToLayer("Player");
        NetworkServer.Spawn(gameObject);
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        SearchForTarget();
        Follow();
    }

    void SearchForTarget()
    {
        if (!isServer)
        {
            return;
        }
        if (Target == null)
        {
            Collider2D [] hitColliders = Physics2D.OverlapCircleAll(loc.position, FollowRange, caster);
            if (hitColliders.Length > 0)
            {

                int randomint = Random.Range(0, hitColliders.Length);
                Target = hitColliders[randomint].transform;
            }
        }
        
    }

    void Follow()
    {
        if (Target!= null && isServer)
        {

            Vector2 direction = Vector2.MoveTowards(new Vector2(loc.position.x, loc.position.y), Target.position, 1 * Time.deltaTime);
            if (Mathf.Abs(loc.position.x - Target.position.x) > Mathf.Abs(loc.position.y - Target.position.y))
            {
                anim.SetBool("Side", true);
                anim.SetBool("Up", false);
                anim.SetBool("Down", false);
                anim.SetBool("Move", true);
                if ((loc.position.x - Target.position.x) < 0)
                {
                    rendy.flipX = false;
                    // invoke the change on the Server as you already named the function
                    CmdProvideFlipStateToServer(rendy.flipX);
                }
                else if ((loc.position.x - Target.position.x) > 0)
                {
                    rendy.flipX = true;
                    // invoke the change on the Server as you already named the function
                    CmdProvideFlipStateToServer(rendy.flipX);
                }
            }
            else if (Mathf.Abs((loc.position.x - Target.position.x)) < Mathf.Abs((loc.position.y - Target.position.y)))
            {
                anim.SetBool("Side", false);
                anim.SetBool("Move", true);
                if ((loc.position.y - Target.position.y) < 0)
                {
                    anim.SetBool("Up", true);
                    anim.SetBool("Down", false);
                }

                else
                {
                    anim.SetBool("Up", false);
                    anim.SetBool("Down", true);
                }

            }
            else if ((loc.position.y - Target.position.y) == 0 && (loc.position.x - Target.position.x) == 0)
            {
                anim.SetBool("Move", false);
            }

            transform.position = direction;
        }
    }


    [Command]
    void CmdProvideFlipStateToServer(bool state)
    {
        // make the change local on the server
        rendy.flipX = state;

        // forward the change also to all clients
        RpcSendFlipState(state);
    }


    [ClientRpc]
    void RpcSendFlipState(bool state)
    {
        // skip this function on the LocalPlayer 
        // because he is the one who originally invoked this
        if (isLocalPlayer) return;

        //make the change local on all clients
        rendy.flipX = state;
    }





}
