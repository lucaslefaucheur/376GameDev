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

    //Player Number
    private int playerNumber;

    //Health 
    private float maxHealth = 50f;
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
    public bool grounded;
    public GameObject arrow;

    //spawn point
    private bool respawn = false;
    private Vector3[] playerInitialSpawn = { new Vector3(-11.2f, 0.8f, 0.0f), new Vector3(5.3f, 0.8f, 0.0f), new Vector3(-11.2f, -9.3f, 0.0f), new Vector3(5.3f, -9.3f, 0.0f) };


    private void Start()
    {

        GameObject gameManager = GameObject.Find("Manager");
        gameManager.GetComponent<GameController>().AddPlayer();
        playerNumber = gameManager.GetComponent<GameController>().getNumOfPLayers();
        Debug.Log("Player number " + gameManager.GetComponent<GameController>().getNumOfPLayers() + " has joined the game.");

        this.transform.position = playerInitialSpawn[playerNumber-1];
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
            anim.SetTrigger("attacking");
            melee();
        }

        if (Input.GetButtonDown("Weapon"))
        {           
            if (GetComponent<Sword>() != null)
            {
                anim.SetTrigger("attacking");
                swordHit();
            }
            else if (GetComponent<Staff>() != null)
            {
                staffHit();
            }
            else if (GetComponent<bow>() != null)
            {
                bowHit();
            }
            else if (GetComponent<Shield>() != null)
            {
                shieldHit();
            }
            else
                Debug.Log("no weapon attached");
        }


        if (Input.GetButtonDown("Pickup"))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, facing, 1.5f);
            if (hit.collider != null && hit.collider.gameObject.layer.Equals(10))           
            {
                if (hit.collider.tag.Equals("Sword"))
                {
                    Destroy(hit.collider.gameObject);
                    NetworkServer.UnSpawn(hit.collider.gameObject);
                    unequip();
                    gameObject.AddComponent<Sword>();
                    Debug.Log("has sword");
                    anim.SetBool("hasSword", true);
                }

                if (hit.collider.tag.Equals("Bow"))
                {
                    unequip();
                    gameObject.AddComponent<bow>();
                    Debug.Log("has bow");
                }

                if (hit.collider.tag.Equals("Shield"))
                {
                    unequip();
                    gameObject.AddComponent<Shield>();
                    Debug.Log("has shield");
                }

                if (hit.collider.tag.Equals("Staff"))
                {
                    unequip();
                    gameObject.AddComponent<Staff>();
                    Debug.Log("has staff");
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facing , 1.5f);
        Debug.DrawRay(transform.position, facing* 1.5f, Color.green, 5.5f);
        if (hit.collider != null && hit.collider.gameObject.layer.Equals(9))
        {
            hit.collider.gameObject.GetComponent<Health>().TakeDamage(smallAttack());

            //to remove
            Debug.Log("melee attack hit for: " + smallAttack());
        }
        else if (hit.collider != null && hit.collider.gameObject.tag == "RhinoBoss")
        {
            hit.collider.gameObject.GetComponent<Health>().TakeDamage(smallAttack());

            //to remove
            Debug.Log("melee attack hit for: " + smallAttack());
        }
    }

    //weapon attack function
    private void swordHit()
    {
        RaycastHit2D[] hit = new RaycastHit2D[] { Physics2D.Raycast(transform.position, facing, 1.5f), Physics2D.Raycast(transform.position, facing + new Vector2(transform.right.x, transform.right.y), .75f), Physics2D.Raycast(transform.position, facing + new Vector2(-transform.right.x, -transform.right.y), .75f) };
        Debug.DrawRay(transform.position, facing * 1.5f, Color.green, 5.5f);
        Debug.DrawRay(transform.position, facing * Vector2FromAngle(90) * 1.5f, Color.green, 5.5f);
        Debug.DrawRay(transform.position, facing *Vector2FromAngle(-90) * 1.5f, Color.green, 5.5f);
        int temp = GetComponent<Sword>().weaponAttack(attackVar, attack);
        if (hit != null)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider != null && hit[i].collider.gameObject.layer.Equals(9))
                {
                    hit[i].collider.gameObject.GetComponent<Health>().TakeDamage(temp);

                    //to remove
                    Debug.Log("sword attack hit for: " + temp);
                }
            }
        }
    }

    public Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }

    private void bowHit()
    {
       
        Debug.DrawRay(transform.position, facing * 1.5f, Color.green, 5.5f);
        int temp = (GetComponent<bow>().weaponAttack(attackVar, attack)); ;
        if (hit != null)
        {
            GameObject arrow = Instantiate(arrow, transform.position, Quaternion.LookRotation(transform.position, facing));
            NetworkServer.Spawn(arrow);
        }
    }

    private void shieldHit()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facing, 1.5f);
        if (hit.collider != null && hit.collider.gameObject.layer.Equals(9))
        {
            //hit.collider.gameObject.GetComponent<Health>().TakeDamage(bigAttack());

            //to remove
            Debug.Log("sword attack hit for: " + bigAttack());
        }
    }

    private void staffHit()
    {
        Collider2D [] hit = Physics2D.OverlapCircleAll(transform.position, 2f);
        Debug.DrawRay(transform.position, Vector2.up * 2f, Color.green, 5.5f);
        Debug.DrawRay(transform.position, Vector2.right * 2f, Color.green, 5.5f);
        Debug.DrawRay(transform.position, Vector2.left * 2f, Color.green, 5.5f);
        Debug.DrawRay(transform.position, Vector2.down * 2f, Color.green, 5.5f);

        int temp = GetComponent<Staff>().weaponAttack(attackVar, attack); ;
        if (hit != null)
        {
            for (int i = 0; i< hit.Length; i++)
            {

                if (hit[i] != null && hit[i].gameObject.layer.Equals(9))
                {
                    hit[i].gameObject.GetComponent<Health>().TakeDamage(temp);

                    //to remove
                    Debug.Log("staff attack hit for: " + temp);
                }
                else if (hit[i] != null && hit[i].gameObject.layer.Equals(8))
                {
                    //heal
                    hit[i].gameObject.GetComponent<PlayerController>().setHealth(temp);
                    //to remove
                    Debug.Log("staff attack heal for: " + temp);

                }
            }
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

    public void unequip()
    {
        if (GetComponent<Sword>() != null)
        {
            Destroy(gameObject.GetComponent<Sword>());
            anim.SetBool("hasSword", false);
        }
        else if (GetComponent<bow>() != null)
            Destroy(gameObject.GetComponent<bow>());
        else if (GetComponent<Staff>() != null)
            Destroy(gameObject.GetComponent<Staff>());
        else if (GetComponent<Shield>() != null)
            Destroy(gameObject.GetComponent<Shield>());

        Debug.Log("unequipped");
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

    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        //If contact with RhinoBoss
        if (collision.gameObject.tag == "RhinoBoss")
        {
            if (collision.gameObject.transform.position.x <= transform.position.x)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(20, 0) * 1500);
            }
            else if (collision.gameObject.transform.position.y <= transform.position.y)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 20) * 1500);
            }
            else if (collision.gameObject.transform.position.y > transform.position.y)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -20) * 1500);
            }
            else if (collision.gameObject.transform.position.x > transform.position.x)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(-20, 0) * 1500);
            }

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

    public void setRespawn()
    {
        respawn = true;
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

    public void setHealth( int hp)
    {
        currentHealth += hp;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
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
