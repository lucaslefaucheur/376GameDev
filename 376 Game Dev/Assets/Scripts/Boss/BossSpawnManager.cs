using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnManager : MonoBehaviour {

    [SerializeField]
    private float scale;
    private float health;
    private Vector2 spawnPoint;
    private GameObject bossObject;

	// Use this for initialization
	void Start () {
        //set a scale for the boss' health
        //scale = level * numPlayer;

        //multiply scale with base health
        //health = scale * health;

        //get random bossObject and spawn
        bossObject = GetComponent<MapManagerScript>().getRandomBoss();
        Instantiate(bossObject, spawnPoint, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {

        //kill boss object when health is 0
        if(health <= 0)
        {
            Destroy(bossObject);
        }
		
	}
}
