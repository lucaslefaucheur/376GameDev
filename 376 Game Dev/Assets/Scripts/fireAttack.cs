using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class fireAttack : MonoBehaviour {
    public GameObject center;
    float moveSpeed = 0.5f;
   // private Transform loc;
    // Use this for initialization
    void Start () {
        destory();
	}
	
	// Update is called once per frame
	void Update () {
        move();
	}
    //void OnTriggerEnter2D(Collider2D collider)
    //{
    //    if (collider.tag == "Player")
    //    {

    //        Destroy(this.gameObject);



    //    }
    //}

    private void move()
    {
        Vector2 direction = Vector2.MoveTowards(transform.position, center.transform.position, 1 * Time.deltaTime);
        transform.position = direction;
    }
    public void destory()
    {

        Destroy(this.gameObject, 3f);

    }
}
