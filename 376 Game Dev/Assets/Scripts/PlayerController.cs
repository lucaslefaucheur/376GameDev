using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    //imported objects
    public Camera cam;

    //variable set up
    public float speed;
    private Vector2 facing;

    //Health 
    private float maxHealth = 30f;
    //sych health over network to know when your teammates are dead
    [SyncVar(hook = "OnChangeHealth")]
    public float currentHealth;

    //Base Stats -- To be used to calculate attacks and damage, and to be changed with getters and setters
    private float armourVar = 0f;
    private int attack = 10;
    private float attackVar = 0f;
    private float healthVar = 0f;

    //for internal referencing
    private Rigidbody2D playerRB;
    private Animator anim;
    private SpriteRenderer rendy;
    public RectTransform healthBar;

    //item stuff
    private GameObject equipped;


    private void Start()
    {
        // set local components
        playerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rendy = GetComponent<SpriteRenderer>();

        // set initial health
        currentHealth = maxHealth;
    }

    //On local player start only
    public override void OnStartLocalPlayer()
    {

    }

    void Update()
    {
        //don't touch
        if (!isLocalPlayer)
        {
            cam.enabled = false;
            return;
        }

        Move();
        
        if (Input.GetButtonDown("Melee"))
        {
            melee();
        }

        if (Input.GetButtonDown("Weapon"))
        {
            if (GetComponent<Sword>() != null)
            {
                weaponHit();
            }
            else
                Debug.Log("weapon attack");
        }


        if (Input.GetButtonDown("Pickup"))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, facing, 1.5f);
            if (hit.collider != null && hit.collider.gameObject.layer.Equals(10))           
            {
                if (hit.collider.tag.Equals("Sword"))
                {
                    Destroy(hit.collider.gameObject);
                gameObject.AddComponent<Sword>();
                }

                if (hit.collider.tag.Equals("Bow"))
                {
                    //gameObject.AddComponent<Bow>();
                }

                if (hit.collider.tag.Equals("Shield"))
                {
                    //gameObject.AddComponent<Shield>();
                }

                if (hit.collider.tag.Equals("Staff"))
                {
                    //gameObject.AddComponent<Staff>();
                }
            }

        }

     }




    /***********************************************************
     * 
     * 
     *  Functions
     * 
     * 
     * 
     * ********************************************************/

    //attack function
    private void melee()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facing, 1.5f);
        if (hit.collider != null && hit.collider.gameObject.layer.Equals(9))
        {
            hit.collider.gameObject.GetComponent<EnemyController>().TakeDamage(smallAttack());

            //to remove
            Debug.Log("melee attack hit for: " + smallAttack());
        }
    }

    //weapon attack function
    private void weaponHit()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facing, 1.5f);
        if (hit.collider != null && hit.collider.gameObject.layer.Equals(9))
        {
            hit.collider.gameObject.GetComponent<EnemyController>().TakeDamage(bigAttack());

            //to remove
            Debug.Log("melee attack hit for: " + bigAttack());
        }
    }

    // to be called by the collision detector
    public void TakeDamage(int amount)
    {
        if (!isServer)
        {
            return;
        }
        currentHealth -= amount * (1 - armourVar);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Destroy(gameObject);
            Debug.Log("Dead!");
        }
    }

    //to be used to cast an attack
    public int smallAttack()
    {
        return (int)Mathf.Floor(attack * (1 + attackVar));
    }

    //to be used to cast a big attack
    public int bigAttack()
    {
        return(GetComponent<Sword>().weaponAttack(attackVar, attack));
    }

    //to move player + animations
    private void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal") * Time.deltaTime * 5.000001f; ;
        float moveVertical = Input.GetAxis("Vertical") * Time.deltaTime * 5.0f; ;
        if (Mathf.Abs(moveHorizontal) > Mathf.Abs(moveVertical))
        {
            anim.SetBool("moveSide", true);
            anim.SetBool("moveUp", false);
            anim.SetBool("moveDown", false);
            anim.SetBool("isMoving", true);
            if (moveHorizontal < 0)
            {
                rendy.flipX = true;
                facing = Vector2.left;
                // invoke the change on the Server as you already named the function
                CmdProvideFlipStateToServer(rendy.flipX);
            }
            else if (moveHorizontal > 0)
            {
                rendy.flipX = false;
                facing = Vector2.right;
                // invoke the change on the Server as you already named the function
                CmdProvideFlipStateToServer(rendy.flipX);
            }
        }
        else if (Mathf.Abs(moveHorizontal) < Mathf.Abs(moveVertical))
        {
            anim.SetBool("moveSide", false);
            anim.SetBool("isMoving", true);
            if (moveVertical > 0)
            {
                facing = Vector2.up;
                anim.SetBool("moveUp", true);
                anim.SetBool("moveDown", false);
            }

            else
            {
                facing = Vector2.down;
                anim.SetBool("moveUp", false);
                anim.SetBool("moveDown", true);
            }

        }
        else if (moveVertical == 0 && moveHorizontal == 0)
        {
            anim.SetBool("isMoving", false);
        }

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
        playerRB.transform.Translate(movement);

        if (Mathf.Abs(transform.position.x) > 9.8f)
        {
            playerRB.transform.Translate(new Vector3(-moveHorizontal, 0));
        }
        if (Mathf.Abs(transform.position.y) > 5.8f)
        {
            playerRB.transform.Translate(new Vector3(0, -moveVertical));
        }


    }

    //to be called when level finishes, to up base stats
    public void levelUp(int level)
    {
        //scales the attack base up with level up
        attack = (int)Mathf.Floor(attack + (0.325f * level));
        //takes note of the players health percentage
        float temp = (currentHealth / maxHealth)*100;
        //scales the health base up wih the level up
        maxHealth = (int)Mathf.Floor(maxHealth + (0.25f * level));
        //scales the current health of the player by using the presisting the helth precentage
        currentHealth = maxHealth * temp;
    }

    private void OnChangeHealth(float currentHealth)
    {
        //sets the size of the green healthbar in relaiton to the percentage of health left
        healthBar.sizeDelta = new Vector2((currentHealth / maxHealth) * 100, healthBar.sizeDelta.y);
    }

    /***********************************************************
     * 
     * 
     *  Getters and Setters
     * 
     * 
     * 
     * ********************************************************/

    public float getHealth()
    {
        return currentHealth;
    }

    public int getAttack()
    {
        return attack;
    }

    public void setArmour(float armour)
    {
        armourVar = armour;
    }

    public void setAttack (float attack)
    {
        attackVar = attack;
    }

    /***********************************************************
     * 
     * 
     *  Network Methods
     * 
     * 
     * 
     * ********************************************************/


    [Command]
    void CmdProvideFlipStateToServer(bool state)
    {
        // make the change local on the server
        rendy.flipX = state;

        // forward the change also to all clients
        RpcSendFlipState(state);
    }

    // invoked by the server only but executed on ALL clients
    [ClientRpc]
    void RpcSendFlipState(bool state)
    {
        // skip this function on the LocalPlayer 
        // because he is the one who originally invoked this
        if (isLocalPlayer) return;

        //make the change local on all clients
        rendy.flipX = state;
    }


    /***********************************************************
    * 
    *  Cooroutines 
    * 
    * 
    * 
    * 
    * ********************************************************/


}
