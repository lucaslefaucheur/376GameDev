using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bowProjectile : MonoBehaviour {

   
    Rigidbody2D mRigidBody2D;
    int temp;
    Quaternion rotation;
    Vector2 angle;

    private void Start()
    {
        mRigidBody2D = GetComponent<Rigidbody2D>();
        rotation = gameObject.transform.rotation;
        angle = rotation * Vector2.up;
        mRigidBody2D.velocity = angle * 5;

    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        if ( hit.collider.gameObject.layer.Equals(9))
        {
            hit.collider.gameObject.GetComponent<Health>().TakeDamage(temp);

            //to remove
            Debug.Log("sword attack hit for: " + temp);
        }
        Destroy(gameObject);
    }

    public void setTemp(int val)
    {
        temp = val;
    }

}
