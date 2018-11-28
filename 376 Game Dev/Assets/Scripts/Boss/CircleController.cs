using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleController : MonoBehaviour {

    public int damage;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer.Equals(8))
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
}
