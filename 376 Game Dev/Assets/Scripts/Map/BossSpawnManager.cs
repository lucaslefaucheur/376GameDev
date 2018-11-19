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

    private List<GameObject> items = new List<GameObject>();
    bool loot = false;

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

        Object[] objList = Resources.LoadAll("Item", typeof(Object));

        // Add object to map list
        foreach (Object obj in objList)
        {
            GameObject item = (GameObject)obj;
            items.Add(item);
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

        if (isServer && bossTemp && !loot)
        {
            //instantiate a single random item
            int numOfPlayers = GameObject.FindGameObjectsWithTag("Player").Length;

            for (int i = 0; i < numOfPlayers; i++)
            {
                Debug.Log("Spawning Item");
                GameObject itemPick = items[Random.Range(0, items.Count)];
                GameObject itemDrop = Instantiate(itemPick, transform.position, Quaternion.identity);
                NetworkServer.Spawn(itemDrop);
            }
        }
	}
}
