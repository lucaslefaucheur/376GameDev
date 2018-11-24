using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RhinoController : NetworkBehaviour
{
    private Transform Target;
    private Transform loc;
    private LayerMask caster;
    private readonly float FollowRange = 10;
    private readonly float PatrolRange = 3;
    public Transform moveSpot;

    private Animator anim;
    private SpriteRenderer renderer;

    private Vector2 InitialPosition;
    private Vector2 direction;

    private float PatrolSpeed, FollowSpeed, AttackSpeed;
    public bool knockedBack;

    private float counter;
    public GameObject EnemyHitParticle;

    void Start()
    {
        loc = transform;
        renderer = GetComponent<SpriteRenderer>();
        caster = 1 << LayerMask.NameToLayer("Player");
        anim = GetComponent<Animator>();

        InitialPosition.x = transform.position.x;
        InitialPosition.y = transform.position.y;

        PatrolSpeed = 0.5f;
        FollowSpeed = 1;
        AttackSpeed = 1;
        moveSpot.position = new Vector2(Random.Range(InitialPosition.x - PatrolRange, InitialPosition.x + PatrolRange), Random.Range(InitialPosition.y - PatrolRange, InitialPosition.y + PatrolRange));

    }

    void FixedUpdate()
    {
        SearchForTarget();
        if (Target == null)
        {
            
        }
        else
        {
            float distance = Vector3.Distance(gameObject.transform.position, Target.transform.position);
            if (distance >= 5)
                Follow(FollowSpeed);
            else if (distance < 5 && distance > 2)
                Follow(5);
            else
            {
                Attack(distance);
            }
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
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(loc.position, FollowRange, caster);

            if (hitColliders.Length > 0)
            {
                int randomint = Random.Range(0, hitColliders.Length);
                Target = hitColliders[randomint].transform;
            }
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
                renderer.flipX = false; // invoke the change on the Server as you already named the function
                CmdProvideFlipStateToServer(renderer.flipX);
            }
            else if ((loc.position.x - goTo.position.x) > 0)
            {
                renderer.flipX = true; // invoke the change on the Server as you already named the function
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

    /* Follow: enemy follows a player
     ********************************/

    void Follow(float fSpeed)
    {
        // wait before following 
        if (counter > 0)
        {
            counter -= Time.deltaTime;
        }
        else if (isServer)
        {
            // direction of the follow: towards the position of the player
            direction = Vector2.MoveTowards(new Vector2(loc.position.x, loc.position.y), Target.position, fSpeed * Time.deltaTime);
            transform.position = direction;
        }
    }

    /* Attack: enemy attacks a player
     *******************************/

    void Attack(float distance)
    {
        // direction of the attack: towards the position of the player
        direction.x = Target.transform.position.x - transform.position.x;
        direction.y = Target.transform.position.y - transform.position.y;
        direction.Normalize();
        transform.Translate(AttackSpeed * direction.x * Time.deltaTime, AttackSpeed * direction.y * Time.deltaTime, 0);
        counter = 0.2f;
    }

    //Colliding with the player will cause damage to the player
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer.Equals(8))
        {
            Debug.Log("Collision with player");
            other.gameObject.GetComponent<PlayerController>().TakeDamage(GetComponent<Health>().currentAttackDamage);
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
        renderer.flipX = state; // make the change local on the server
        RpcSendFlipState(state); // forward the change also to all clients
    }

    [ClientRpc]
    void RpcSendFlipState(bool state)
    {
        if (isLocalPlayer) return; // skip this function on the LocalPlayer because he is the one who originally invoked this
        renderer.flipX = state; //make the change local on all clients
    }
}
