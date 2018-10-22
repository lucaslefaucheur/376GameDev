using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BossController : NetworkBehaviour
{

    private Transform Target;
    private Transform loc;
    private LayerMask caster;
    private float FollowRange = 100;
    public float bossHp = 50.0f;
    private float statsBossHp;
    public int ASPD = 25;
    public int ATK2Spd = 8;//dash range 
    private bool isInAttackRange = false;
    private bool attacking = false;
    private bool attack1Final = false;
    private float timeStap = 3;
    private float castFire = 2;
    private float attackRange = 3;
    private int step = 1;
    private Vector3 robackLoc;
    private Vector3 AttackLoc;
    public GameObject bossHPObj;
    private float bossHPLength;
    private Animator anim;
    private SpriteRenderer rendy;
    private float slowRate = 2f;
    public GameObject player;
    public GameObject fireAttack;
    public GameObject firePos;
    GameObject fireTemp;
    public bool flag = true; //fire attack flag
    private float timer = 0;

    private float timerMax = 0;
    //public float atkDamage;
    //public float dashDamage;


    float FollowSpeed;

    void Start()
    {
        loc = transform;
        rendy = GetComponent<SpriteRenderer>();
        caster = 1 << LayerMask.NameToLayer("Player");
        NetworkServer.Spawn(gameObject);
        anim = GetComponent<Animator>();
        GetComponent<CircleCollider2D>().radius = attackRange;//currently set the radius of the boss collider to attackRange, it means the boss awake range. once we have the animation this will be changed 
        SearchForTarget();
        bossHPLength = bossHPObj.transform.localScale.x;
        statsBossHp = bossHp;



    }

    void FixedUpdate()
    {
        if (SearchForTarget())
        {
            if (!isInAttackRange && !attacking)
            {
                move();
            }
            else
            {

                bossAtkLoop();
                //在这等两秒
                generateFireAtk();


            }
        }
    }

    /**
     * TODO
     * 
     * */
    bool SearchForTarget()
    {
        if (!isServer)
        {
            return false;
        }

        if (Target == null)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(loc.position, FollowRange, caster);
            if (hitColliders.Length > 0)
            {
                int randomint = Random.Range(0, hitColliders.Length);
                Target = hitColliders[randomint].transform;
                return true;
            }
            return false;
        }
        else
        {
            return true;
        }
    }




    void move()//let boss moves & dashes towards the character facing position
    {
        Follow();
        if (Mathf.Abs(loc.position.y - Target.position.y) < Mathf.Abs(loc.position.x - Target.position.x))
        {
            anim.SetBool("Up", false);
            anim.SetBool("Down", false);
            anim.SetBool("Side", true);
            anim.SetBool("Move", true);
            if (Target.position.x > loc.position.x)
            {
                rendy.flipX = false;
            }
            else
            {
                rendy.flipX = true;
            }

            // invoke the change on the Server as you already named the function
            CmdProvideFlipStateToServer(rendy.flipX);
            return;
        }

        if (loc.position.y - Target.position.y < 0)
        {
            anim.SetBool("Move", true);
            anim.SetBool("Up", true);
            anim.SetBool("Down", false);
            anim.SetBool("Side", false);
        }
        else
        {
            anim.SetBool("Move", true);
            anim.SetBool("Up", false);
            anim.SetBool("Down", true);
            anim.SetBool("Side", false);
        }
    }

    void Follow()
    {

        if (Target != null && isServer)
        {

            Vector2 direction = Vector2.MoveTowards(new Vector2(loc.position.x, loc.position.y), (new Vector2(Target.position.x, Target.position.y)), 1 * Time.deltaTime);

            float distance = (loc.position - Target.position).sqrMagnitude;

            if (distance > attackRange) //算攻击距离如果大于他
            {
                transform.position = direction;
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

    /**
     *  TODO
     **/
    public void reduceHp(float num)//boss 收到 num的伤害 然后检查是否死亡 player can use this function outside class
    {
        bossHp = bossHp - num;
        bossHPObj.transform.localScale = new Vector3(bossHPLength * (bossHp / statsBossHp), bossHPObj.transform.localScale.y, bossHPObj.transform.localScale.z);
        inspectHp();
    }

    /**
     *  TODO
     **/
    private void inspectHp()
    {

        if (bossHp <= 0)
        {
            dead();
        }
    }

    /**
     *  TODO 
     **/
    private void dead()
    {
        Destroy(this.gameObject);
    }

    void bossAtkLoop()
    {
        if ((isInAttackRange || attacking) && !attack1Final)
        {
            attacking = true;
            timeStap -= Time.deltaTime;
            if (timeStap - ASPD * 0.1f <= 0 && isInAttackRange)//if the player didn't walk out the range to dodge it, the boss atk will take effect
            {
                timeStap = 3;// attack time stap
                bossNormalAtk();
                attack1Final = true;
                robackLoc = new Vector3(loc.position.x, loc.position.y);
                AttackLoc = new Vector3(Target.position.x, Target.position.y);
            }
            else
            {
                attacking = false;
            }
        }
        else if ((isInAttackRange || attacking) && attack1Final && fireTemp == null)
        {

            dashAtk();

        }
        else
        {
            timeStap = 3;
        }
    }



    void bossNormalAtk()//implement boss attack damage here
    {
        //TODO use player API to reduce HP 减血//boss 
        //if (isInAttackRange)
        //{
        //    //player.hp -= atkDamage;
        //}
        Debug.Log("Roar!");

    }

    void dashAtk()//dash to player, deal a certan amount of dash damage
    {

        switch (step)
        {
            case 1:
                Vector2 direction = Vector2.MoveTowards(new Vector2(loc.position.x, loc.position.y), (new Vector2(AttackLoc.x, AttackLoc.y)), ATK2Spd * Time.deltaTime);
                transform.position = direction;
                float distance = (loc.position - AttackLoc).sqrMagnitude;//returns the length of the vector
                                                                         //right now i think it is because of network issue, the actual number is changed in player but he is not slowed
                                                                         //bossRoar();
                if (distance <= 0.5)
                {
                    step = 2;
                }

                //TODO use player API to reduce HP
                //if (isInAttackRange)
                //{
                //    //player.hp -= dashDamage;
                //}
                break;
            case 2:
                Vector2 direction2 = Vector2.MoveTowards(new Vector2(loc.position.x, loc.position.y), (new Vector2(robackLoc.x, robackLoc.y)), ATK2Spd * Time.deltaTime);
                transform.position = direction2;
                float distance2 = (loc.position - robackLoc).sqrMagnitude;//returns the length of the vector

                if (distance2 <= 0)
                {
                    attack1Final = false;
                    attacking = false;
                    step = 1;
                    timeStap = 3;
                }
                break;
        }
    }


    void fireAtk()
    {

        if (isInAttackRange == true && flag == true)
        {


            Vector2 pos = new Vector2(Target.transform.position.x + 0.5f, Target.transform.position.y + 0.5f);
            fireTemp = Instantiate(fireAttack, pos, Quaternion.identity);
            flag = false;
        }


    }

    void generateFireAtk()
    {
        if (attacking == false)
        {
            Invoke("fireAtk", 5f);
            if (fireTemp == null)
            {
                flag = true;
            }
        }

    }


    //void bossRoar() // this will slow down enemy movement speed #it's not useable now due to the network problem
    //{
    //    //if (player.GetComponent<PlayerController>().playerMoveSpeed > 3&& isInAttackRange =)
    //    //{
    //    player.GetComponent<PlayerController>().playerMoveSpeed = 1;
    //    //}
    //}

    //private bool Waited(float seconds)
    //{
    //    timerMax = seconds;

    //    timer += Time.deltaTime;

    //    if (timer >= timerMax)
    //    {
    //        return true; //max reached - waited x - seconds
    //    }

    //    return false;
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isInAttackRange = true;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)//if player walks out then the attack will be dodged
    {
        if (collision.tag == "Player")
        {
            isInAttackRange = false;
        }
    }
}






















