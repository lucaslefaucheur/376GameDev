using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BossSpawnManager : NetworkBehaviour {

    public float scale;
    public float health;
    public Vector2 spawnPoint;

    private GameObject bossObject;
    private GameObject bossTemp;

    // Use this for initialization
    void Start () {
        //set a scale for the boss' health
        //scale = level * numPlayer;

        //multiply scale with base health
        //health = scale * health;

        //spawn random bossObject
        if (isServer)
        {
            bossObject = GameObject.Find("Manager").GetComponent<MapManagerScript>().getRandomBoss();
            bossTemp = Instantiate(bossObject, spawnPoint, Quaternion.identity);
            NetworkServer.Spawn(bossTemp);

        }
	}
	
	// Update is called once per frame
	void Update () {

        //kill boss object when health is 0
        /*
        if(health <= 0)
        {
            Destroy(bossObject);
        }
        */
		
	}
}
