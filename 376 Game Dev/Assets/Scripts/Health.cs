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
       // healthBar.sizeDelta = new Vector2((currentHealth / startingHealth) * 100, healthBar.sizeDelta.y);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Destroy(gameObject, 1.0f);
        }

        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    public void GainHealth(int heal)
    {
        if (currentHealth <= startingHealth)
        {
            currentHealth += heal;
        }
    }

}
