using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyController : NetworkBehaviour {


    //for internal referencing
    private Transform Target;
    private Transform loc;
    private LayerMask caster;
    private Animator anim;
    private SpriteRenderer rendy;
    public Transform moveSpot; 

    //variables
    private readonly float FollowRange = 10;
    private readonly float PatrolRange = 3;
    private int front = 1; 
    private bool test1, test2 = false;
    private float counter = 2.0f;
    private Vector2 InitialPosition;
    private Vector2 direction;
    public int Health = 10; // TODO: put it on the network 
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
        NetworkServer.Spawn(gameObject);
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
            else
                Attack(distance);
        }
        Orientation();
    }

    public void TakeDamage() {
        Health--;
        if (Health <= 0)
            Destroy(gameObject);
    }

    //Colliding with the player will cause damage to the player
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer.Equals(8))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(5);
        }
    }

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
                rendy.flipX = false;
                // invoke the change on the Server as you already named the function
                CmdProvideFlipStateToServer(rendy.flipX);
            }
            else if ((loc.position.x - goTo.position.x) > 0)
            {
                rendy.flipX = true;
                // invoke the change on the Server as you already named the function
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

    void Patrol() 
    {
        direction = Vector2.MoveTowards(transform.position, moveSpot.position, PatrolSpeed * Time.deltaTime);
        transform.position = direction;

        if (Vector2.Distance(transform.position, moveSpot.position) < 0.2f) 
        {
            moveSpot.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        }
    }

    void Follow()
    {
        if (Target != null && isServer)
        {
            direction = Vector2.MoveTowards(new Vector2(loc.position.x, loc.position.y), Target.position, FollowSpeed * Time.deltaTime);
            transform.position = direction;
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
