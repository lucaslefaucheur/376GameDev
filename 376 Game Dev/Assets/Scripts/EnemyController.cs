using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyController : NetworkBehaviour {
    
    private Transform Target;
    private Transform loc;
    private LayerMask caster;
    private readonly float FollowRange = 10;
    private readonly float PatrolRange = 3;

    private Animator anim;
    private SpriteRenderer rendy;

    private int front = 1; 
    private bool test1, test2 = false;
    private float counter = 2.0f;
    Vector2 InitialPosition;
    private Vector2 direction;
    int Health = 10; // TODO: put it on the network 

    public Transform moveSpot; 
    private float minX, maxX, minY, maxY;

    private float PatrolSpeed, FollowSpeed, AttackSpeed;

    void Start()
    {
        loc = transform;
        rendy = GetComponent<SpriteRenderer>();
        caster = 1 << LayerMask.NameToLayer("Player");
        NetworkServer.Spawn(gameObject);
        anim = GetComponent<Animator>();

        InitialPosition.x = transform.position.x;
        InitialPosition.y = transform.position.y;
        minX = InitialPosition.x - PatrolRange;
        maxX = InitialPosition.x + PatrolRange;
        minY = InitialPosition.y - PatrolRange;
        maxY = InitialPosition.y + PatrolRange;

        PatrolSpeed = 0.5f;
        FollowSpeed = 2;
        AttackSpeed = 3;
        direction = new Vector2(0, 0);
        direction.Normalize();
        anim.SetBool("Move", true);
        moveSpot.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    void FixedUpdate()
    {
        SearchForTarget();
        if (Target == null) 
        {
            Patrol();
        }
        else
        {
            float distance = Vector3.Distance(gameObject.transform.position, Target.transform.position);
            if (distance > 2)
                Follow();
            else {
                Attack(distance);
            }
        }
        Orientation();
    }

    void TakeDamage() {
        Health--;
        if (Health <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") 
        {
            // TODO: if player is attacking -> TakeDamage() 
        }
    }

    void Orientation()
    {
        /*
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x) && direction.y > 0)
        {
            anim.SetBool("Side", false);

            if (direction.y > 0)
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
        else
        {
            anim.SetBool("Side", true);
            anim.SetBool("Up", false);
            anim.SetBool("Down", false);

            if (direction.x > 0)
            {
                rendy.flipX = false;
                CmdProvideFlipStateToServer(rendy.flipX);
            }
            else 
            {
                rendy.flipX = true;
                CmdProvideFlipStateToServer(rendy.flipX);
            }
        }
        */
    }

    void Patrol() 
    {
        direction = Vector2.MoveTowards(transform.position, moveSpot.position, PatrolSpeed * Time.deltaTime);
        transform.position = direction;

        if (Vector2.Distance(transform.position, moveSpot.position) < 0.2f) 
        {
            moveSpot.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        }
    }

    void Attack(float distance)
    {
        if (!test2) 
        {
            direction.x = Target.transform.position.x - transform.position.x;
            direction.y = Target.transform.position.y - transform.position.y;
            direction.Normalize();

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

            transform.Translate(AttackSpeed * front * direction.x * Time.deltaTime, AttackSpeed * front * direction.y * Time.deltaTime, 0);
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
            direction = Vector2.MoveTowards(new Vector2(loc.position.x, loc.position.y), Target.position, FollowSpeed * Time.deltaTime);
            transform.position = direction;
            /*
        if (Mathf.Abs(loc.position.x - Target.position.x) > Mathf.Abs(loc.position.y - Target.position.y))
        {
            anim.SetBool("Side", true);
            anim.SetBool("Up", false);
            anim.SetBool("Down", false);

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
        */
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
