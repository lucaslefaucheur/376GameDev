using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NutController : NetworkBehaviour {


    int temp;
    Vector2 nutDirection;
    Vector3 playerPosition;
    Rigidbody2D mRigidBody2D;
    private int damage;


    // Use this for initialization
    public void Shoot (Transform tran) {
        mRigidBody2D = GetComponent<Rigidbody2D>();
        nutDirection = (tran.position - transform.position).normalized;
        mRigidBody2D.velocity = nutDirection * 5;
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.layer.Equals(8))
            other.gameObject.GetComponent<PlayerController>().TakeDamage(5);
        NetworkServer.Destroy(gameObject);

    }

    public void setTemp(int val)
    {
        temp = val;
    }

    public void setDamage(int newDamage)
    {
        damage = newDamage;
    }
}
