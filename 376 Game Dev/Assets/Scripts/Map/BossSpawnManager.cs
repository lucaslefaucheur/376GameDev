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

    private GameController scaler;

    bool loot = false;

    // Use this for initialization
    void Start () {

        scaler = GameObject.Find("Manager").GetComponent<GameController>();

        //set a scale for the boss' health
        //scale = level * numPlayer;

        //multiply scale with base health
        //health = scale * health;

        //spawn random bossObject
        if (isServer)
        {
            for (int i = 0; i < (Mathf.Floor(scaler.getLevel() / 30) + 1); i++)
            {
                bossObject = GameObject.Find("Manager").GetComponent<MapManagerScript>().getRandomBoss();
                bossTemp = Instantiate(bossObject, spawnPoint, Quaternion.identity);
                // Health and Damage scaling
                bossTemp.GetComponent<Health>().setHealth(bossTemp.GetComponent<Health>().getStartingHealth() + (bossTemp.GetComponent<Health>().getStartingHealth() / 5 * (scaler.getLevel() - 1)));
                bossTemp.GetComponent<Health>().setAttackDamage(bossTemp.GetComponent<Health>().getStartingAttack() + (bossTemp.GetComponent<Health>().getStartingAttack() / 5 * (scaler.getLevel() - 1)));
                NetworkServer.Spawn(bossTemp);
            }
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

        if (isServer && bossTemp == null && !loot)
        {
            GameObject.Find("Manager").GetComponent<MapManagerScript>().SpawnDoor();
            GameObject.Find("Manager").GetComponent<MapManagerScript>().spawnChest(new Vector3(3.6f, -5f, -1f));
            loot = true;
        }
	}
}
