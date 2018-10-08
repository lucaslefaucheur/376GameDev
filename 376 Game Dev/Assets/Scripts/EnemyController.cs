using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyController : NetworkBehaviour {
    
    private Transform Target;
    private Transform loc;
    private LayerMask caster;
    private float FollowRange = 10;

    private Animator anim;
    private SpriteRenderer rendy;

    float FollowSpeed;

    private float distance;
    private int front = 1;

    bool test1 = false;
    bool test2 = false;
    float counter = 2.0f;

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
        if (Target != null)
        {
            distance = Vector3.Distance(gameObject.transform.position, Target.transform.position);
            if (distance > 2)
                Follow();
            else {
                Attack(distance);

            }
                
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") 
        {
            // TODO: TakeDamage(); add: only if player was attacking 
            // Destroy(gameObject, 2.0f);
            // dying animation 
        }
    }

    void Attack(float distance)
    {
        if (!test2) 
        {
            float xdirection = Target.transform.position.x - transform.position.x;
            float ydirection = Target.transform.position.y - transform.position.y;

            if (distance >= 1.9)
            {
                front = 1;
                if (test1)
                {
                    test2 = true;
                    counter = 2.0f;
                }
            }

            if (distance <= 1)
            {
                front = -1;
                test1 = true;
            }

            transform.Translate(4 * front * xdirection * Time.deltaTime, 4 * front * ydirection * Time.deltaTime, 0);
        }
        else 
        {
            counter -= Time.deltaTime;
            if (counter <= 0) 
            {
                test1 = false;
                test2 = false;
            }
        }

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
