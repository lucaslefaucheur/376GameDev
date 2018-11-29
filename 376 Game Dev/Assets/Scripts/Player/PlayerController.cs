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
    [SyncVar]
    private bool alive = true;

    //Base Stats -- To be used to calculate attacks and damage, and to be changed with getters and setters
    [SyncVar]
    private float armourVar = 0f;
    private int startingAttack = 5;
    [SyncVar]
    private int attack = 5;
    [SyncVar]
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
    private int durInit = 50;
    private float durCur = 0;

    //UI
    public Text Weapon;
    public RectTransform durBar;
    public Text cc;
    public Text dung;
    public GameObject UIcam;

    //spawn point
    private bool respawn = false;
    private Vector3[] playerInitialSpawn = { new Vector3(-11.2f, 0.8f, 0.0f), new Vector3(5.3f, 0.8f, 0.0f), new Vector3(-11.2f, -9.3f, 0.0f), new Vector3(5.3f, -9.3f, 0.0f) };

    //Crystal and Revive
    [SyncVar(hook = "OnChangeCrystals")]
    private int crystalCount = 0;
    [SyncVar]
    private bool reviving = false;
    public GameObject reviveAnim;
    private GameObject revive;

    GameObject gameManager;

    //Teleport
    private bool teleporting = false;
    public GameObject teleAnim;
    private GameObject tele;

    //Death
    public GameObject ghostAnim;
    private GameObject ghost;
    [SyncVar]
    private bool dying = false;

    //color
    private Color playerColor;
    private Color[] colorString = { new Color(1f, 0.8f, 0.5f, 1f), new Color(0.5f, 0.5f, 1f, 1f), new Color(0.5f, 1f, 0.5f, 1f), new Color(0.8f, 0.5f, 1f, 1f) };


    private void Start()
    {

        if (isLocalPlayer)
        {
            UIcam.GetComponent<Canvas>().enabled = true;
        }
        gameManager = GameObject.Find("Manager");
        gameManager.GetComponent<GameController>().AddPlayer();
        playerNumber = gameManager.GetComponent<GameController>().getNumOfPLayers();

        this.transform.position = playerInitialSpawn[playerNumber - 1];
        // set local components
        playerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        netAnim = GetComponent<NetworkAnimator>();
        rendy = GetComponent<SpriteRenderer>();

        // set initial health
        currentHealth = maxHealth;
        durBar.sizeDelta = new Vector2((durCur / durInit) * 100, durBar.sizeDelta.y);
        playerColor = gameObject.GetComponent<SpriteRenderer>().color = colorString[playerNumber-1];
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
            Debug.LogError(facing);


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
                    swordHit();
                }
                else if (GetComponent<Staff>() != null)
                {
                    netAnim.SetTrigger("attacking");
                    netAnim.animator.ResetTrigger("attacking");
                    staffHit();
                }
                else if (GetComponent<bow>() != null)
                {
                    netAnim.SetTrigger("attacking");
                    netAnim.animator.ResetTrigger("attacking");
                    bowHit();
                }
                else if (GetComponent<Shield>() != null)
                {
                    netAnim.SetTrigger("attacking");
                    netAnim.animator.ResetTrigger("attacking");
                    shieldHit();
                }
            }


            if (Input.GetButtonDown("Pickup"))
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, facing, 1.5f);
                if (hit.collider != null && hit.collider.gameObject.layer.Equals(10))
                {
                    if (hit.collider.tag.Equals("chest"))
                    {
                        Vector3 chestPos = hit.collider.gameObject.transform.position;
                        gameObject.GetComponent<AudioSource>().clip = chestOpenSound;
                        gameObject.GetComponent<AudioSource>().Play();
                        CmdDestroy(hit.collider.gameObject);
                        CmdChest(chestPos);
                    }

                    else if (hit.collider.tag.Equals("Sword"))
                    {
                        unequip();
                        Weapon.text = "Sword";
                        gameObject.GetComponent<AudioSource>().clip = pickWeaponSound;
                        gameObject.GetComponent<AudioSource>().Play();
                        moveVar = -0.25f;
                        armourVar = 0.25f;
                        CmdDestroy(hit.collider.gameObject);
                        gameObject.AddComponent<Sword>();
                        setAnimation("hasSword");
                    }

                    else if (hit.collider.tag.Equals("Bow"))
                    {
                        unequip();
                        Weapon.text = "Bow";
                        gameObject.GetComponent<AudioSource>().clip = pickWeaponSound;
                        gameObject.GetComponent<AudioSource>().Play();
                        moveVar = 0.3f;
                        armourVar = -0.25f;
                        CmdDestroy(hit.collider.gameObject);
                        gameObject.AddComponent<bow>();
                        Debug.Log("has bow");
                        setAnimation("hasBow");
                    }

                    else if (hit.collider.tag.Equals("Shield"))
                    {
                        unequip();
                        Weapon.text = "Shield";
                        gameObject.GetComponent<AudioSource>().clip = pickWeaponSound;
                        gameObject.GetComponent<AudioSource>().Play();
                        moveVar = -0.5f;
                        armourVar = 0.5f;
                        CmdDestroy(hit.collider.gameObject);
                        gameObject.AddComponent<Shield>();
                        Debug.Log("has shield");
                        setAnimation("hasShield");
                    }

                    else if (hit.collider.tag.Equals("Staff"))
                    {
                        unequip();
                        Weapon.text = "Staff";
                        gameObject.GetComponent<AudioSource>().clip = pickWeaponSound;
                        gameObject.GetComponent<AudioSource>().Play();
                        moveVar = -0.25f;
                        armourVar = -0.25f;
                        CmdDestroy(hit.collider.gameObject);
                        gameObject.AddComponent<Staff>();
                        Debug.Log("has staff");
                        setAnimation("hasStaff");
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

    //set the animation for the selected weapon
    private void setAnimation(string type)
    {
      anim.SetBool("hasBow", false);
      anim.SetBool("hasStaff", false);
      anim.SetBool("hasSword", false);
      anim.SetBool("hasShield", false);
      anim.SetBool(type, true);
      StartCoroutine(changingWeapon());
    }


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
        }
        else if (hit.collider != null && hit.collider.gameObject.tag == "RhinoBoss")
        {
            CmdDealDamage(hit.collider.gameObject, smallAttack());
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
        CmdSpawnArrow(temp, facing);
    }

    private void shieldHit()
    {
        gameObject.GetComponent<AudioSource>().clip = sheildSound;
        gameObject.GetComponent<AudioSource>().Play();

        float temp = (GetComponent<Shield>().weaponAttack(attackVar, attack));
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
                }
                else if (hit[i] != null && hit[i].gameObject.layer.Equals(8))
                {
                    //heal
                    CmdHeal(hit[i].gameObject, temp);

                    
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

            //Death
            currentHealth = 0;
            alive = false;
            if (crystalCount >= 5 && !reviving)
            {
                StartCoroutine(Revive());
            }
            else if (crystalCount < 5 && !dying)
            {
                StartCoroutine(Death(gameObject));
            }
            else
            {
                currentHealth = 0;
            }
        }
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.75f, 0, 0, 1);
        Invoke("resetColor", 0.5f);
    }

    //reset color of player
    public void resetColor()
    {
      gameObject.GetComponent<SpriteRenderer>().color = playerColor;
    }


    //teleporting animation
    public void teleport()
    {
        if (!isServer)
        {
            return;
        }
        if (!teleporting)
        {
            StartCoroutine(TeleportAnimation());
        }
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
        Weapon.text = "None";
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

    public void setDurInit(int dur)
    {
        durInit = dur;
    }

    public void setDurCur(int dur)
    {
        durCur = dur;
        durBar.sizeDelta = new Vector2((durCur / durInit) * 100, durBar.sizeDelta.y);
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

    private void OnChangeCrystals(int crystalCount)
    {
        Debug.Log("before Health = " + currentHealth);
        attack = (int)Mathf.Floor(startingAttack + crystalCount^(1/2));
        //takes note of the players health percentage
        float temp = (currentHealth / maxHealth);
        Debug.Log("temp = " + temp);
        //scales the health base up wih the level up
        maxHealth = (int)Mathf.Floor(50 + crystalCount );
        //scales the current health of the player by using the presisting the helth precentage
        currentHealth = maxHealth * temp;
        Debug.Log("after Health = " + currentHealth);
        cc.text = crystalCount.ToString();
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
        Color playerInitialColor = colorString[hit.gameObject.GetComponent<PlayerController>().getNumber() - 1];
        hit.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 1.0f, 1);
        StartCoroutine(resetColorStaff(hit, playerInitialColor, 0.5f));
    }

    [Command]
    void CmdChest(Vector3 vec)
    {
        // make the change local on the server
        GameObject.Find("Manager").GetComponent<MapManagerScript>().spawnWeapon(vec);

    }

    [Command]
    void CmdDestroy(GameObject state)
    {
        // make the change local on the server
        NetworkServer.Destroy(state);

    }

    [Command]
    void CmdSpawnArrow(int temp, Vector2 pos)
    {
        // make the change local on the server
        GameObject arrowSpawn = Instantiate(arrow, transform.position, Quaternion.FromToRotation(Vector2.up, pos));
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
        tele = Instantiate(teleAnim, new Vector3(transform.position.x, transform.position.y, -1f), Quaternion.identity);
        NetworkServer.Spawn(tele);
    }

    [Command]
    void CmdGhost()
    {
        ghost = Instantiate(ghostAnim, new Vector3(transform.position.x, transform.position.y, -1f), Quaternion.identity);
        NetworkServer.Spawn(ghost);
    }

    [Command]
    void CmdProvideFlipStateToServer(bool state)
    {
        // make the change local on the server
        rendy.flipX = state;

        // forward the change also to all clients
        RpcSendFlipState(state);
    }

    [Command]
    void CmdGetLevel()
    {
        gameManager.GetComponent<GameController>().getLevel();
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
        alive = true;
        setHealth((int)maxHealth);
        reviving = false;

    }

    IEnumerator Death(GameObject player)
    {
        dying = true;
        StartCoroutine(FloatingGhost());
        yield return new WaitForSeconds(5);
        CmdDestroy(ghost);
        CmdDestroy(player);
    }

    IEnumerator TeleportAnimation()
    {
        teleporting = true;
        alive = false;
        CmdTeleport();
        yield return new WaitForSeconds(1);
        alive = true;
        CmdDestroy(tele);
        dung.text = (gameManager.GetComponent<GameController>().getLevel() - 1).ToString();
        teleporting = false;
    }

    IEnumerator FloatingGhost()
    {
        CmdGhost();
        yield return new WaitForSeconds(0);
    }

    IEnumerator resetColorStaff(Collider2D hit, Color oldColor, float delayTime)
    {
       yield return new WaitForSeconds(delayTime);
       hit.gameObject.GetComponent<SpriteRenderer>().color = oldColor;
    }

    IEnumerator changingWeapon()
    {
       alive = false;
       anim.SetBool("isMoving", false);
       yield return new WaitForSeconds(0.1f);
       alive = true;
    }

    IEnumerator resetColorStaff(GameObject hit, Color oldColor, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        hit.GetComponent<SpriteRenderer>().color = oldColor;
    }


}
