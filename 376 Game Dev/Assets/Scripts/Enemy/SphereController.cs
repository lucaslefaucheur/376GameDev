using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SphereController : NetworkBehaviour {

    private Animator anim;
    private float counter; 

    private void Start()
    {
        counter = 5.75f; 
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (counter > 0.75)
            counter -= Time.deltaTime;
        else {
            counter = 5.75f;
            anim.SetBool("Explodes", true);
            NetworkServer.Destroy(gameObject);
        }
    }

    //Colliding with the player will cause damage to the player
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer.Equals(8))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(5);
            anim.SetBool("Explodes", true);
            if (counter > 5)
                counter -= Time.deltaTime;
            else
            {
                NetworkServer.Destroy(gameObject);
            }
            Destroy(gameObject, 0.75f);
        }
    }
}
