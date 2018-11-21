using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour {

    private Animator anim;
    private float counter; 

    private void Start()
    {
        counter = 5.0f; 
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (counter > 0)
            counter -= Time.deltaTime;
        else {
            counter = 5.0f;
            anim.SetBool("Explodes", true);
            Destroy(gameObject, 0.75f);
        }
    }

    //Colliding with the player will cause damage to the player
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer.Equals(8))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(5);
            anim.SetBool("Explodes", true);
            Destroy(gameObject, 0.75f);
        }
    }
}
