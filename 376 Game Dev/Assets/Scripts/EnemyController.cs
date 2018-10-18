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
    private Vector2 InitialPosition;
    private Vector2 direction;
    private int Health = 10;

    public Transform moveSpot;

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

        PatrolSpeed = 0.5f;
        FollowSpeed = 2;
        AttackSpeed = 3;

        moveSpot.position = new Vector2(Random.Range(InitialPosition.x - PatrolRange, InitialPosition.x + PatrolRange), Random.Range(InitialPosition.y - PatrolRange, InitialPosition.y + PatrolRange));
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
            else
                Attack(distance);
        }
        Orientation();
    }

    /***********************************
     *
     * Functions
     *
     ***********************************/
    
    /* SearchForTarget: looks for a player in the FollowRange
     *******************************************************/
    
    void SearchForTarget()
    {
        if (!isServer)
            return;
            
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
    
    /* TakeDamage: substracts a number to the enemy's health
     ******************************************************/
    
    void TakeDamage(int number) {
        Health -= number;
        if (Health <= 0) {
            // TODO: call a Die() method
        }
    }

    /* Orientation: determines which sprite to use
     ********************************************/
    
    void Orientation()
    {
        anim.SetBool("Move", true);

        Transform goTo;
        if (Target == null)
            goTo = moveSpot;
        else
            goTo = Target;

        if (Mathf.Abs(loc.position.x - goTo.position.x) > Mathf.Abs(loc.position.y - goTo.position.y))
        {
            anim.SetBool("Side", true);
            anim.SetBool("Up", false);
            anim.SetBool("Down", false);

            if ((loc.position.x - goTo.position.x) < 0)
            {
                rendy.flipX = false; // invoke the change on the Server as you already named the function
                CmdProvideFlipStateToServer(rendy.flipX);
            }
            else if ((loc.position.x - goTo.position.x) > 0)
            {
                rendy.flipX = true; // invoke the change on the Server as you already named the function
                CmdProvideFlipStateToServer(rendy.flipX);
            }
        }
        else if (Mathf.Abs((loc.position.x - goTo.position.x)) < Mathf.Abs((loc.position.y - goTo.position.y)))
        {
            anim.SetBool("Side", false);
            if ((loc.position.y - goTo.position.y) < 0)
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
    }

    /* Patrol: enemy walks towards random move spots
     **********************************************/

    void Patrol() 
    {
        // direction of the patrol: towards the 'move spot'
        direction = Vector2.MoveTowards(transform.position, moveSpot.position, PatrolSpeed * Time.deltaTime);
        transform.position = direction;

        // if the enemy reaches the 'move spot'
        if (Vector2.Distance(transform.position, moveSpot.position) < 0.2f) 
        {
            // create a new random 'move spot'
            moveSpot.position = new Vector2(Random.Range(InitialPosition.x - PatrolRange, InitialPosition.x + PatrolRange), Random.Range(InitialPosition.y - PatrolRange, InitialPosition.y + PatrolRange));
        }
    }

    /* Follow: enemy follows a player
     ********************************/
    
    void Follow()
    {
        if (Target != null && isServer)
        {
            // direction of the follow: towards the position of the player
            direction = Vector2.MoveTowards(new Vector2(loc.position.x, loc.position.y), Target.position, FollowSpeed * Time.deltaTime);
            transform.position = direction;
        }
    }

    /* Attack: enemy attacks a player
     *******************************/
    
    void Attack(float distance)
    {
        if (!test2) 
        {
            // direction of the attack: towards the position of the player
            direction.x = Target.transform.position.x - transform.position.x;
            direction.y = Target.transform.position.y - transform.position.y;
            direction.Normalize();

            // enemy moves towards the player
            if (distance >= 1.9)
            {
                front = 1;
                if (test1)
                {
                    test2 = true;
                    counter = 2.0f;
                }
            }

            // enemy moves away from the player
            if (distance <= 1)
            {
                front = -1;
                test1 = true;
            }
            
            transform.Translate(AttackSpeed * front * direction.x * Time.deltaTime, AttackSpeed * front * direction.y * Time.deltaTime, 0);
        }
        else 
        {
            counter -= Time.deltaTime; // counter between every attacks
            if (counter <= 0) 
            {
                test1 = false;
                test2 = false;
            }
        }
    }
    
    /***********************************
     *
     * Network
     *
     ***********************************/

    [Command]
    void CmdProvideFlipStateToServer(bool state)
    {
        rendy.flipX = state; // make the change local on the server
        RpcSendFlipState(state); // forward the change also to all clients
    }

    [ClientRpc]
    void RpcSendFlipState(bool state)
    {
        if (isLocalPlayer) return; // skip this function on the LocalPlayer because he is the one who originally invoked this
        rendy.flipX = state; // make the change local on all clients
    }
}
