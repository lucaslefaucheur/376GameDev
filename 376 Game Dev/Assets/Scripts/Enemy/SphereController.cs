using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour {
    public int damage;

    void Update()
    {
        Destroy(gameObject, 5);
    }

    //Colliding with the player will cause damage to the player
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer.Equals(8))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
}
