using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour {

    //variables
    public int startingHealth;
    public int currentHealth;
    public RectTransform healthBar;
    public GameObject deathFX;
    private Transform Target;


    void Awake()
    {
        // Set the initial health of the enemy
        currentHealth = startingHealth;
    }

    void Start()
    {

    }

    public void resetColor() { gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1); }

    /***********************************
     *
     * Functions
     *
     ***********************************/


    /* TakeDamage: substracts a number to the enemy's health
     ******************************************************/

    public void TakeDamage(int damage) {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            CmdSpawnCrys();
            CmdDestroy(gameObject);
        }
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.75f, 0, 0, 1);
        Invoke("resetColor", 1.0f);
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    public void GainHealth(int heal)
    {
        if (currentHealth <= startingHealth)
        {
            currentHealth += heal;
        }
    }

    [Command]
    void CmdDestroy(GameObject state)
    {
        // make the change local on the server
        NetworkServer.Destroy(state);

    }

    [Command]
    void CmdSpawnCrys()
    {
        // make the change local on the server
        GameObject crys = Instantiate(deathFX, transform.position, transform.rotation);
        NetworkServer.Spawn(crys);

    }

}
