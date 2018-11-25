using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireController : MonoBehaviour {

    private float timer;
    private float freezeTimer;
    private float speed = 5f;
    private float radius = 2f;
    private float angle;
    private Vector2 centre;
    private Vector2 fireDirection;
    Vector3 playerPosition;


    // Use this for initialization
    void Start () {

        //playerPosition = gameObject.GetComponent<BearController>().getTarget();
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        fireDirection = (playerPosition - transform.position).normalized;

        timer = Time.time + 8;
        freezeTimer = Time.time + 1;
        centre = transform.position;

    }
	
	// Update is called once per frame
	void Update () {

        if(Time.time > timer)
        {
            Destroy(this.gameObject);
        }

        if(Time.time > freezeTimer)
        {
            speed = 0;
        }

        this.transform.Translate(fireDirection * Time.deltaTime * speed);

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        speed = 0;
        if (other.gameObject.layer.Equals(8))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(5);
        }
        
    }
}
