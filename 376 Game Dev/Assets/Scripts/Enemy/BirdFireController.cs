using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdFireController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 0.417f);
	}

    //Colliding with the player will cause damage to the player
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer.Equals(8))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(5);
        }
    }

}
