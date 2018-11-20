using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GiraffeController : NetworkBehaviour {
    
    private Transform Target;
    private Transform loc;
    private LayerMask caster;
    private readonly float FollowRange = 10;
    private readonly float PatrolRange = 3;

    private Animator anim;
    private SpriteRenderer rendy;

    private Vector2 InitialPosition;
    private Vector2 direction;
    int Health = 10;

    public Transform moveSpot;

    private float PatrolSpeed, FollowSpeed, AttackSpeed;
    
    private float counter;
    //public GameObject EnemyHitParticle;
    private int attackCounter = 0;

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
            if (distance > 2) {
                Follow();
            }
                
            else if (distance > 1) {
                Attack(distance);
                attackCounter++;
            }
                
            else {
                Teleport();
            }

            if(attackCounter == 3)
            {
                Target = null;
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
            Collider2D [] hitColliders = Physics2D.OverlapCircleAll(loc.position, FollowRange, caster);
            
            if (hitColliders.Length > 0)
            {
                int randomint = FindRandomTarget(hitColliders);
                Target = hitColliders[randomint].transform;
            }
        }
    }

    private int FindRandomTarget(Collider2D[] hitList)
    {
        int targetnumber = -1;
        int temp;
        while (targetnumber < 0)
        {
            temp = Random.Range(0, hitList.Length);
            if (hitList[temp].GetComponent<PlayerController>().getHealth() <= 0)
            {
                targetnumber = temp;
            }
        }
        return targetnumber;
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
                
                /*
                 * COMMENTED OUT BECAUSE OF ERROR 
                 */
                //CmdProvideFlipStateToServer(rendy.flipX);
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
        // wait before following 
        if (counter > 0)
        {
            anim.SetBool("Appear", true);
            counter -= Time.deltaTime;
        }
        else if (isServer)
        {
            anim.SetBool("Appear", false);
            // direction of the follow: towards the position of the player
            direction = Vector2.MoveTowards(new Vector2(loc.position.x, loc.position.y), Target.position, FollowSpeed * Time.deltaTime);
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
        counter = 0.917f;
    }

    /* Teleport: enemy teleports to a random position around the player
     *****************************************************************/
    
    void Teleport() {
        // wait before exploding
        if (counter > 0)
        {
            anim.SetBool("Explode", true);
            counter -= Time.deltaTime;
        }
        else
        {
            anim.SetBool("Explode", false);
            //Instantiate(EnemyHitParticle, gameObject.transform.position, gameObject.transform.rotation); // emit a particle effect
            Vector2 teleportPosition = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            teleportPosition.Normalize();
            teleportPosition.x *= 2;
            teleportPosition.y *= 2;
            transform.position += new Vector3(3 * teleportPosition.x, 3 * teleportPosition.y, 0); // modify the position of the enemy
            counter = 0.583f;
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
        rendy.flipX = state; //make the change local on all clients
    }
}
