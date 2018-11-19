using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bowProjectile : MonoBehaviour {

   
    Rigidbody2D mRigidBody2D;
    Vector2 attack;
    Quaternion rotation;
    Vector2 angle;

    private void Start()
    {
        mRigidBody2D = GetComponent<Rigidbody2D>();
        rotation = gameObject.transform.rotation;
        angle = rotation * new Vector2(1,0);
        mRigidBody2D.velocity = angle * 1;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
