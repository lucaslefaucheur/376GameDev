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
    public int startingAttackDamage;
    public int currentAttackDamage;


    void Awake()
    {
        // Set the initial health of the enemy
        currentHealth = startingHealth;
    }

    void Start()
    {

    }


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
            Instantiate(deathFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.75f, 0, 0, 1);
        Invoke("resetColor", 1.0f);
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
        // pushed back
      /*  Vector2 pushbackdirection = Target.transform.position - gameObject.transform.position;
        pushbackdirection.Normalize();
        rb.AddForce(-pushbackdirection * 5, ForceMode2D.Impulse);*/
    }

    public void GainHealth(int heal)
    {
        if (currentHealth <= startingHealth)
        {
            currentHealth += heal;
        }
    }

}
