using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bubble : NetworkBehaviour
{
    float lifeTime = 30;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void setLife(int time)
    {
        lifeTime = time;
        Debug.Log("Shield Time = " + time);
    }
}