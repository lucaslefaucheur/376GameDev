using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour {


    [SerializeField]
    private float timer = 5f;

    private float creationTime;

    // Use this for initialization
    void Start()
    {
        creationTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time > creationTime + timer)
        {
            Destroy(this.gameObject);
        }
    }

}
