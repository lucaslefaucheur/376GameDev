using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BirdFireController : NetworkBehaviour {
    
    private float counter;
    private int damage;

    private void Start()
    {
        counter = 2f;
    }

    void Update()
    {
        if (counter > 0)
            counter -= Time.deltaTime;
        else
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    //Colliding with the player will cause damage to the player
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer.Equals(8))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            NetworkServer.Destroy(gameObject);
        }
    }

    public void setDamage(int newDamage)
    {
        damage = newDamage;
    }

}
