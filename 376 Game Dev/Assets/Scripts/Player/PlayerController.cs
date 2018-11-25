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
    [SyncVar]
    private float maxHealth = 50f;
    //sych health over network to know when your teammates are dead
    [SyncVar(hook = "OnChangeHealth")]
    public float currentHealth;
    private bool alive = true;

    //Base Stats -- To be used to calculate attacks and damage, and to be changed with getters and setters
    [SyncVar]
    private float armourVar = 0f;
    private int attack = 10;
    private float attackVar = 0f;
    [SyncVar]
    private float moveVar = 0f;

    //for internal referencing
    private Rigidbody2D playerRB;
    private Animator anim;
    private NetworkAnimator netAnim;
    private SpriteRenderer rendy;
    public RectTransform healthBar;

    //audio clips
    public AudioClip swingSound;
    public AudioClip meleeSound;
    public AudioClip sheildSound;
    public AudioClip speerSound;
    public AudioClip arrowSound;
    public AudioClip pickWeaponSound;
    public AudioClip pickCrystalSound;
    public AudioClip levelUpSound;
    public AudioClip dieSound;
    public AudioClip e1Sound;
    public AudioClip e2Sound;
    public AudioClip e3Sound;
    public AudioClip weaponBreakSound;
    public AudioClip chestOpenSound;


    //item stuff
    private GameObject equipped;
    public bool grounded;
    public GameObject arrow;
    public GameObject bubble;

    //spawn point
    private bool respawn = false;
    private Vector3[] playerInitialSpawn = { new Vector3(-11.2f, 0.8f, 0.0f), new Vector3(5.3f, 0.8f, 0.0f), new Vector3(-11.2f, -9.3f, 0.0f), new Vector3(5.3f, -9.3f, 0.0f) };

    //Crystal and Revive
    private int crystalCount = 0;
    [SyncVar]
    private bool reviving = false;
    public GameObject reviveAnim;
    private GameObject revive;

    //Teleport
    public GameObject teleAnim;
    private GameObject tele;

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
        netAnim = GetComponent<NetworkAnimator>();
        rendy = GetComponent<SpriteRenderer>();

        // set initial health
        currentHealth = maxHealth;
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
                netAnim.SetTrigger("melee");
                netAnim.animator.ResetTrigger("melee");
                melee();
            }

            if (Input.GetButtonDown("Weapon"))
            {
                if (GetComponent<Sword>() != null)
                {
                    netAnim.SetTrigger("attacking");
                    netAnim.animator.ResetTrigger("attacking");
                    //anim.SetTrigger("attacking");
                    swordHit();
                }
                else if (GetComponent<Staff>() != null)
                {
                    netAnim.SetTrigger("attacking");
                    netAnim.animator.ResetTrigger("attacking");
                    //anim.SetTrigger("attacking");
                    staffHit();
                }
                else if (GetComponent<bow>() != null)
                {
                    netAnim.SetTrigger("attacking");
                    netAnim.animator.ResetTrigger("attacking");
                    //anim.SetTrigger("attacking");
                    bowHit();
                }
                else if (GetComponent<Shield>() != null)
                {
                    netAnim.SetTrigger("attacking");
                    netAnim.animator.ResetTrigger("attacking");
                    //anim.SetTrigger("attacking");
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
                        gameObject.GetComponent<AudioSource>().clip = chestOpenSound;
                        gameObject.GetComponent<AudioSource>().Play();
                        CmdDestroy(hit.collider.gameObject);
                        CmdChest();
                    }

                    else if (hit.collider.tag.Equals("Sword"))
                    {
                        gameObject.GetComponent<AudioSource>().clip = pickWeaponSound;
                        gameObject.GetComponent<AudioSource>().Play();
                        moveVar = -0.25f;
                        armourVar = 0.25f;
                        CmdDestroy(hit.collider.gameObject);
                        unequip();
                        gameObject.AddComponent<Sword>();
                        Debug.Log("has sword");

                        anim.SetBool("hasSword", true);
                        anim.SetBool("hasStaff", false);
                        anim.SetBool("hasShield", false);
                        anim.SetBool("hasBow", false);
                    }

                    else if (hit.collider.tag.Equals("Bow"))
                    {
                        gameObject.GetComponent<AudioSource>().clip = pickWeaponSound;
                        gameObject.GetComponent<AudioSource>().Play();
                        moveVar = 0.3f;
                        armourVar = -0.25f;
                        CmdDestroy(hit.collider.gameObject);
                        unequip();
                        gameObject.AddComponent<bow>();
                        Debug.Log("has bow");
                        anim.SetBool("hasBow", true);
                        anim.SetBool("hasStaff", false);
                        anim.SetBool("hasSword", false);
                        anim.SetBool("hasShield", false);
                    }

                    else if (hit.collider.tag.Equals("Shield"))
                    {
                        gameObject.GetComponent<AudioSource>().clip = pickWeaponSound;
                        gameObject.GetComponent<AudioSource>().Play();
                        moveVar = -0.5f;
                        armourVar = 0.5f;
                        CmdDestroy(hit.collider.gameObject);
                        unequip();
                        gameObject.AddComponent<Shield>();
                        Debug.Log("has shield");
                        anim.SetBool("hasBow", false);
                        anim.SetBool("hasShield", true);
                        anim.SetBool("hasSword", false);
                        anim.SetBool("hasStaff", false);
                    }

                    else if (hit.collider.tag.Equals("Staff"))
                    {
                        gameObject.GetComponent<AudioSource>().clip = pickWeaponSound;
                        gameObject.GetComponent<AudioSource>().Play();
                        moveVar = -0.25f;
                        armourVar = -0.25f;
                        CmdDestroy(hit.collider.gameObject);
                        unequip();
                        gameObject.AddComponent<Staff>();
                        Debug.Log("has staff");
                        anim.SetBool("hasBow", false);
                        anim.SetBool("hasStaff", true);
                        anim.SetBool("hasSword", false);
                        anim.SetBool("hasShield", false);
                    }
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
        gameObject.GetComponent<AudioSource>().clip = meleeSound;
        gameObject.GetComponent<AudioSource>().Play();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facing, 1.5f);
        Debug.DrawRay(transform.position, facing * 1.5f, Color.green, 5.5f);
        
        if (hit.collider != null && hit.collider.gameObject.layer.Equals(9))
        {
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

        gameObject.GetComponent<AudioSource>().clip = swingSound;
        gameObject.GetComponent<AudioSource>().Play();
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
        gameObject.GetComponent<AudioSource>().clip = arrowSound;
        gameObject.GetComponent<AudioSource>().Play();

        Debug.DrawRay(transform.position, facing * 1.5f, Color.green, 5.5f);
        int temp = (GetComponent<bow>().weaponAttack(attackVar, attack));
        CmdSpawnArrow(temp);


    }

    private void shieldHit()
    {
        gameObject.GetComponent<AudioSource>().clip = sheildSound;
        gameObject.GetComponent<AudioSource>().Play();

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

        gameObject.GetComponent<AudioSource>().clip = speerSound;
        gameObject.GetComponent<AudioSource>().Play();
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
        playDifferentEnemyAttackSound();
        gameObject.GetComponent<AudioSource>().Play();
        if (!isServer)
        {
            return;
        }
        currentHealth -= amount * (1 - armourVar);
        if (currentHealth <= 0)
        {
            gameObject.GetComponent<AudioSource>().clip = dieSound;
            GetComponent<AudioSource>().Play();
            //Death
            currentHealth = 0;
            alive = false;
            if (crystalCount >= 5 && !reviving)
            {
                StartCoroutine(Revive());
                Debug.Log("Reviving...");
            }
            else if (crystalCount < 5)
            {
                StartCoroutine(Death(gameObject));
            }
            else
            {
                currentHealth = 0;
            }
        }
    }

    //teleporting animation
    public void teleport()
    {
        StartCoroutine(TeleportAnimation());
    }


    public void playDifferentEnemyAttackSound()
    {
        int randomSound = Random.Range(0, 3);

        switch (randomSound)
        {
            case 0:
                gameObject.GetComponent<AudioSource>().clip = e1Sound;
                break;
            case 1:
                gameObject.GetComponent<AudioSource>().clip = e2Sound; ;
                break;
            case 2:
                gameObject.GetComponent<AudioSource>().clip = e3Sound;
                break;
        }

       
    }

    //to be used to cast an attack
    public int smallAttack()
    {
        return (int)Mathf.Floor(attack * (1 + attackVar));
    }

    public void unequip()
    {
        gameObject.GetComponent<AudioSource>().clip =weaponBreakSound;
        gameObject.GetComponent<AudioSource>().Play();
        moveVar = 0;
        armourVar = 0;

        if (GetComponent<Sword>() != null)
        {
            Destroy(gameObject.GetComponent<Sword>());
            anim.SetBool("hasSword", false);
        }
        else if (GetComponent<bow>() != null){
            Destroy(gameObject.GetComponent<bow>());
            anim.SetBool("hasBow", false);
          }
        else if (GetComponent<Staff>() != null){
            Destroy(gameObject.GetComponent<Staff>());
            anim.SetBool("hasStaff", false);
          }
        else if (GetComponent<Shield>() != null){
            Destroy(gameObject.GetComponent<Shield>());
            anim.SetBool("hasShield", false);
          }

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
            crystalCount++;
            Debug.Log("Crystal Count " + crystalCount);
            gameObject.GetComponent<AudioSource>().clip = pickCrystalSound;
            gameObject.GetComponent<AudioSource>().Play();
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
        gameObject.GetComponent<AudioSource>().clip = levelUpSound;
        gameObject.GetComponent<AudioSource>().Play();
        //scales the attack base up with level up
        attack = (int)Mathf.Floor(attack + (0.325f * level));
        //takes note of the players health percentage
        float temp = (currentHealth / maxHealth) * 100;
        //scales the health base up wih the level up
        maxHealth = (int)Mathf.Floor(maxHealth + (0.25f * level));
        //scales the current health of the player by using the presisting the helth precentage
        currentHealth = maxHealth * temp;
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
  
    private void OnChangeHealth(float currentHealth)
    {
        //sets the size of the green healthbar in relaiton to the percentage of health left
        healthBar.sizeDelta = new Vector2((currentHealth / maxHealth) * 100, healthBar.sizeDelta.y);
    }

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
        Debug.Log("object destroyed command");

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
    void CmdSpawnRevive()
    {
        // make the change local on the server
        revive = Instantiate(reviveAnim, new Vector3(transform.position.x, transform.position.y, -1f), Quaternion.identity);
        NetworkServer.Spawn(revive);
        

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
    void CmdTeleport()
    {
        tele = Instantiate(teleAnim, transform.position, Quaternion.identity);
        NetworkServer.Spawn(tele);
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
    IEnumerator Revive()
    {
        reviving = true;
        CmdSpawnRevive();
        yield return new WaitForSeconds(5);
        CmdDestroy(revive);
        crystalCount -= 5;
        Debug.Log("Crystal Count " + crystalCount);
        alive = true;
        setHealth((int)maxHealth);
        reviving = false;

    }

    IEnumerator Death(GameObject player)
    {
        yield return new WaitForSeconds(5);
        CmdDestroy(player);
    }

    IEnumerator TeleportAnimation()
    {
        alive = false;
        CmdTeleport();
        yield return new WaitForSeconds(1);
        alive = true;
        Destroy(tele);
    }


}
