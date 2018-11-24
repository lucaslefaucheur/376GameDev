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
    private bool alive = true;

    //Base Stats -- To be used to calculate attacks and damage, and to be changed with getters and setters
    private float armourVar = 0f;
    private int attack = 10;
    private float attackVar = 0f;
    private float healthVar = 0f;
    private float moveVar = 0f;

    //for internal referencing
    private Rigidbody2D playerRB;
    private Animator anim;
    private SpriteRenderer rendy;
    public RectTransform healthBar;

    //item stuff
    private GameObject equipped;
    public bool grounded;
    public GameObject arrow;
    public GameObject bubble;

    //spawn point
    private bool respawn = false;
    private Vector3[] playerInitialSpawn = { new Vector3(-11.2f, 0.8f, 0.0f), new Vector3(5.3f, 0.8f, 0.0f), new Vector3(-11.2f, -9.3f, 0.0f), new Vector3(5.3f, -9.3f, 0.0f) };

    
    private void Start()
    {

        GameObject gameManager = GameObject.Find("Manager");
        gameManager.GetComponent<GameController>().AddPlayer();
        playerNumber = gameManager.GetComponent<GameController>().getNumOfPLayers();
        Debug.Log("Player number " + gameManager.GetComponent<GameController>().getNumOfPLayers() + " has joined the game.");

        this.transform.position = playerInitialSpawn[playerNumber - 1];
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
        NetworkAnimator netAnim = GetComponent<NetworkAnimator>();

        netAnim.SetParameterAutoSend(0, true);
        netAnim.SetParameterAutoSend(1, true);
        netAnim.SetParameterAutoSend(2, true);
        netAnim.SetParameterAutoSend(3, true);
        netAnim.SetParameterAutoSend(4, true);
        netAnim.SetParameterAutoSend(5, true);
    }

    public override void PreStartClient()
    {
        NetworkAnimator netAnim = GetComponent<NetworkAnimator>();
        netAnim.SetParameterAutoSend(0, true);
        netAnim.SetParameterAutoSend(1, true);
        netAnim.SetParameterAutoSend(2, true);
        netAnim.SetParameterAutoSend(3, true);
        netAnim.SetParameterAutoSend(4, true);
        netAnim.SetParameterAutoSend(5, true);
    }

    void Update()
    {
        //don't touch
        if (!isLocalPlayer)
        {
            cam.enabled = false;
            return;
        }

        if (alive)
        {

            Move();

            if (Input.GetButtonDown("Melee"))
            {
                //Should be different animation anim.SetTrigger("attacking");
                melee();
            }

            if (Input.GetButtonDown("Weapon"))
            {
                if (GetComponent<Sword>() != null)
                {
                    GetComponent<NetworkAnimator>().SetTrigger("attacking");
                    anim.SetTrigger("attacking");
                    swordHit();
                }
                else if (GetComponent<Staff>() != null)
                {
                    GetComponent<NetworkAnimator>().SetTrigger("attacking");
                    anim.SetTrigger("attacking");
                    staffHit();
                }
                else if (GetComponent<bow>() != null)
                {
                    GetComponent<NetworkAnimator>().SetTrigger("attacking");
                    anim.SetTrigger("attacking");
                    bowHit();
                }
                else if (GetComponent<Shield>() != null)
                {
                    GetComponent<NetworkAnimator>().SetTrigger("attacking");
                    anim.SetTrigger("attacking");
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
                    if (hit.collider.tag.Equals("chest"))
                    {
                        CmdDestroy(hit.collider.gameObject);
                        CmdChest();
                    }

                    else if (hit.collider.tag.Equals("Sword"))
                    {
                        moveVar = -0.25f;
                        armourVar = 0.25f;
                        CmdDestroy(hit.collider.gameObject);
                        unequip();
                        gameObject.AddComponent<Sword>();
                        Debug.Log("has sword");

                        anim.SetBool("hasSword", true);
                        anim.SetBool("hasStaff", false);
                        anim.SetBool("hasShield", false);
                        anim.SetLayerWeight(1, 1f);
                        anim.SetLayerWeight(2, 0f);
                        anim.SetLayerWeight(3, 0f);
                    }

                    else if (hit.collider.tag.Equals("Bow"))
                    {
                        moveVar = 0.3f;
                        armourVar = -0.25f;
                        CmdDestroy(hit.collider.gameObject);
                        unequip();
                        gameObject.AddComponent<bow>();
                        Debug.Log("has bow");
                        anim.SetBool("hasStaff", false);
                        anim.SetBool("hasSword", false);
                        anim.SetBool("hasShield", false);
                        anim.SetLayerWeight(1, 0f);
                        anim.SetLayerWeight(2, 0f);
                        anim.SetLayerWeight(3, 0f);
                    }

                    else if (hit.collider.tag.Equals("Shield"))
                    {
                        moveVar = -0.5f;
                        armourVar = 0.5f;
                        CmdDestroy(hit.collider.gameObject);
                        unequip();
                        gameObject.AddComponent<Shield>();
                        Debug.Log("has shield");
                        anim.SetBool("hasShield", true);
                        anim.SetBool("hasSword", false);
                        anim.SetBool("hasStaff", false);
                        anim.SetLayerWeight(3, 1f);
                        anim.SetLayerWeight(1, 0f);
                        anim.SetLayerWeight(2, 0f);
                    }

                    else if (hit.collider.tag.Equals("Staff"))
                    {
                        moveVar = -0.25f;
                        armourVar = -0.25f;
                        CmdDestroy(hit.collider.gameObject);
                        unequip();
                        gameObject.AddComponent<Staff>();
                        Debug.Log("has staff");
                        anim.SetBool("hasStaff", true);
                        anim.SetBool("hasSword", false);
                        anim.SetBool("hasShield", false);
                        anim.SetLayerWeight(2, 1f);
                        anim.SetLayerWeight(1, 0f);
                        anim.SetLayerWeight(3, 0f);
                    }
                }
                else if (hit.collider != null && hit.collider.gameObject.layer.Equals(8))
                {
                    //revive
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
        Debug.DrawRay(transform.position, facing * 1.5f, Color.green, 5.5f);
        if (hit.collider != null && hit.collider.gameObject.layer.Equals(9))
        {
            
            //hit.collider.gameObject.GetComponent<EnemyController>().PushedBack();
            CmdDealDamage(hit.collider.gameObject, smallAttack());
            //to remove
            Debug.Log("melee attack hit for: " + smallAttack());
        }
        else if (hit.collider != null && hit.collider.gameObject.tag == "RhinoBoss")
        {
            CmdDealDamage(hit.collider.gameObject, smallAttack());

            //to remove
            Debug.Log("melee attack hit for: " + smallAttack());
        }
    }

    //weapon attack function
    private void swordHit()
    {
        int temp = GetComponent<Sword>().weaponAttack(attackVar, attack);

        Vector2 startPos = transform.position; // umm, start position !
        Vector2 targetPos = (new Vector2(transform.position.x, transform.position.y) + facing); // variable for calculated end position


        float angle = Vector2.Angle(startPos, targetPos) + 90;

        int startAngle = (int)(-angle * 0.5); // half the angle to the Left of the forward
        int finishAngle = (int)(angle * 0.5); // half the angle to the Right of the forward

        // the gap between each ray (increment)
        int inc = (int)(90 / 5);


        // step through and find each target point
        for (int i = startAngle; i < finishAngle; i += inc) // Angle from forward
        {
            targetPos = (Quaternion.Euler(0, 0, i) * facing).normalized * 1.5f + transform.position;

            RaycastHit2D hit = Physics2D.Linecast(startPos, targetPos);
            if (hit.collider != null && hit.collider.gameObject.layer.Equals(9))
            {
                CmdDealDamage(hit.collider.gameObject, temp);

                //to remove
                Debug.Log("sword attack hit for: " + temp);
            }


            // to show ray just for testing
            Debug.DrawLine(startPos, targetPos, Color.green, 5.5f);
        }
    }


    private void bowHit()
    {

        Debug.DrawRay(transform.position, facing * 1.5f, Color.green, 5.5f);
        int temp = (GetComponent<bow>().weaponAttack(attackVar, attack));
        CmdSpawnArrow(temp);


    }

    private void shieldHit()
    {
        float temp = (GetComponent<Shield>().weaponAttack(attackVar, attack));
        Debug.Log("radius = " + temp);
        CmdSpawnBubble(temp);
        Debug.DrawRay(transform.position, Vector2.up * 2f, Color.green, 5.5f);
        Debug.DrawRay(transform.position, Vector2.right * 2f, Color.green, 5.5f);
        Debug.DrawRay(transform.position, Vector2.left * 2f, Color.green, 5.5f);
        Debug.DrawRay(transform.position, Vector2.down * 2f, Color.green, 5.5f);
    }

    private void staffHit()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, 2f);
        Debug.DrawRay(transform.position, Vector2.up * 2f, Color.green, 5.5f);
        Debug.DrawRay(transform.position, Vector2.right * 2f, Color.green, 5.5f);
        Debug.DrawRay(transform.position, Vector2.left * 2f, Color.green, 5.5f);
        Debug.DrawRay(transform.position, Vector2.down * 2f, Color.green, 5.5f);

        int temp = GetComponent<Staff>().weaponAttack(attackVar, attack); ;
        if (hit != null)
        {
            for (int i = 0; i < hit.Length; i++)
            {

                if (hit[i] != null && hit[i].gameObject.layer.Equals(9))
                {
                    CmdDealDamage(hit[i].gameObject, temp);

                    //to remove
                    Debug.Log("staff attack hit for: " + temp);
                }
                else if (hit[i] != null && hit[i].gameObject.layer.Equals(8))
                {
                    //heal
                    CmdHeal(hit[i].gameObject, temp);
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
            //Death
            currentHealth = 0;
            alive = false;
            Destroy(gameObject, 10);
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
        moveVar = 0;
        armourVar = 0;
        
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
        float moveHorizontal = Input.GetAxis("Horizontal") * Time.deltaTime * 5.000001f * (1 + moveVar); ;
        float moveVertical = Input.GetAxis("Vertical") * Time.deltaTime * 5.0f * (1 + moveVar); ;
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

        if (collision.gameObject.tag == "crystal")
        {
            GameObject.Find("Manager").GetComponent<CrystalManager>().addCrystal();
            Debug.Log("Crystal" + GameObject.Find("Manager").GetComponent<CrystalManager>().getCrystalCount());
            Destroy(collision.gameObject);
        }
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
        float temp = (currentHealth / maxHealth) * 100;
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

    public void setHealth(int hp)
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

    public void setAttack(float attack)
    {
        attackVar = attack;
    }

    public int getNumber()
    {
        return playerNumber;
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
    void CmdDealDamage(GameObject hit, int dmg)
    {
        // make the change local on the server
        hit.GetComponent<Health>().TakeDamage(dmg);

    }

    [Command]
    void CmdHeal(GameObject hit, int dmg)
    {
        // make the change local on the server
        hit.GetComponent<PlayerController>().setHealth(dmg);

    }

    [Command]
    void CmdChest()
    {
        // make the change local on the server
        GameObject.Find("Manager").GetComponent<MapManagerScript>().spawnWeapon();

    }

    [Command]
    void CmdDestroy(GameObject state)
    {
        // make the change local on the server
        NetworkServer.Destroy(state);

    }

    [Command]
    void CmdSpawnArrow(int temp)
    {
        // make the change local on the server
        GameObject arrowSpawn = Instantiate(arrow, transform.position, Quaternion.FromToRotation(Vector2.up, facing));
        arrowSpawn.GetComponent<bowProjectile>().setTemp(temp);
        NetworkServer.Spawn(arrowSpawn);

    }

    [Command]
    void CmdSpawnBubble(float temp)
    {
        // make the change local on the server
        GameObject shieldBubble = Instantiate(bubble, transform.position, Quaternion.FromToRotation(Vector2.up, facing));
        shieldBubble.GetComponent<Bubble>().setLife(Random.Range(5, 10));
        shieldBubble.transform.localScale = shieldBubble.transform.localScale * temp;
        NetworkServer.Spawn(shieldBubble);

    }

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