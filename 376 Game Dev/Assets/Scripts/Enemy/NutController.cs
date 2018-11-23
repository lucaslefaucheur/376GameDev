using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutController : MonoBehaviour {

    private float counter;
    Vector2 nutDirection;
    Vector3 playerPosition;

    // Use this for initialization
    void Start () {
        counter = 2f;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        nutDirection = (playerPosition - transform.position).normalized;
    }
	
	// Update is called once per frame
	void Update () {
        if (counter > 0) 
        {
            this.transform.Translate(nutDirection * Time.deltaTime * 3f);
            counter -= Time.deltaTime;
        }
        else
            Destroy(gameObject, 1f);

	}

    void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject, 1f);
        other.gameObject.GetComponent<PlayerController>().TakeDamage(5);
    }
}
