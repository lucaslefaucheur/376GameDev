using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapManagerScript : NetworkBehaviour
{

    public static MapManagerScript instance = null;
    public GameObject initialMapPrefab;
    public GameObject doorPrefab;
    private GameObject currentMap;
    private GameObject[] playerList;
    private GameObject[] objectList;

    //BOSS MAPS
    public List<GameObject> mapBossList = new List<GameObject>();
    private GameObject bossMapPick;
    //BOSS
    public List<GameObject> bossList = new List<GameObject>();
    private GameObject currentBoss;
    //ENEMY MAPS
    public List<GameObject> mapEnemyList = new List<GameObject>();
    private GameObject enemyMapPick;
    //ENEMY
    public List<GameObject> enemyList = new List<GameObject>();
    private GameObject currentEnemy;

    private bool hasMap = true;
    private bool start = true;
    private int mapPicker;

    public GameObject chest;
    public List<GameObject> items = new List<GameObject>();

    void Awake()
    {
        //Check if instance already exists, if not, set instance to this
        if (instance == null)
            instance = this;
        //If instance already exists and it's not this, destroy this, enforcing the singleton pattern
        else if (instance != this)
            Destroy(gameObject);
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
        InitGame();
    }

    // Use this for initialization
    void InitGame()
    {
        currentMap = Instantiate(initialMapPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        loadAllBossMap();
        loadAllBoss();
        loadAllEnemyMap();
        loadAllEnemy();
    }

	// Update is called once per frame
	void Update () {

        if (start)
        {
            start = false;
        }

        if (!hasMap){

            mapPicker = Random.Range(0, 5);
            if(mapPicker%4 != 0)
            {
                StartCoroutine(loadEnemyMap());
            }
            else
            {
                StartCoroutine(loadBossMap());
            }

            hasMap = true;
        }

        if(mapBossList.Count == 0)
        {
            loadAllBossMap();
        }

        if(bossList.Count == 0)
        {
            loadAllBoss();
        }

        if (mapEnemyList.Count == 0)
        {
            loadAllEnemyMap();
        }

        if (enemyList.Count == 0)
        {
            loadAllEnemy();
        }


	}

    //load all boss maps
    private void loadAllBossMap()
    {
        Object[] objList = Resources.LoadAll("BossMap", typeof(Object));

        // Add object to map list
        foreach (Object obj in objList)
        {
            GameObject mapBObject = (GameObject)obj;
            mapBossList.Add(mapBObject);
        }
    }

    //load all enemy maps
    private void loadAllEnemyMap()
    {
        Object[] objList = Resources.LoadAll("EnemyMap", typeof(Object));

        // Add object to map list
        foreach (Object obj in objList)
        {
            GameObject mapEObject = (GameObject)obj;
            mapEnemyList.Add(mapEObject);
        }
    }

    //load all bosses
    private void loadAllBoss()
    {
        //Get all object from Boss folder
        Object[] objList = Resources.LoadAll("Boss", typeof(Object));

        // Add object to boss list
        foreach (Object obj in objList)
        {
            GameObject bossObject = (GameObject)obj;
            bossList.Add(bossObject);
        }
    }

    //load all enemies
    private void loadAllEnemy()
    {
        //Get all object from Boss folder
        Object[] objList = Resources.LoadAll("Enemy", typeof(Object));

        // Add object to boss list
        foreach (Object obj in objList)
        {
            GameObject enemyObject = (GameObject)obj;
            enemyList.Add(enemyObject);
        }
    }

    //get random BOSS map
    IEnumerator loadBossMap()
    {
        if (isServer)
        {
            deleteAll();
            yield return new WaitForSeconds(1);
            //instantiate random map
            bossMapPick = mapBossList[Random.Range(0, mapBossList.Count)];
            currentMap = Instantiate(bossMapPick, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            NetworkServer.Spawn(currentMap);
            //remove map from list
            mapBossList.Remove(bossMapPick);
        }

    }

    //get random ENEMY map
    IEnumerator loadEnemyMap()
    {
        if (isServer)
        {
            deleteAll();
            yield return new WaitForSeconds(1);
            //instantiate random map
            enemyMapPick = mapEnemyList[Random.Range(0, mapEnemyList.Count)];
            currentMap = Instantiate(enemyMapPick, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            NetworkServer.Spawn(currentMap);
            //remove map from list
            mapEnemyList.Remove(enemyMapPick);
        }
    }

    //return random enemy
    public GameObject getRandomEnemy()
    {
        currentEnemy = enemyList[Random.Range(0, enemyList.Count -1)];
        enemyList.Remove(currentEnemy);
        return currentEnemy;
    }

    //return random boss
    public GameObject getRandomBoss()
    {
        currentBoss = bossList[Random.Range(0, bossList.Count)];
        bossList.Remove(currentBoss);
        return currentBoss;
    }

    public void notifyEntry()
    {
        Destroy(currentMap,1f);
        playerList = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i].GetComponent<PlayerController>().teleport();

        }
        GetComponent<GameController>().LevelUp();
        hasMap = false;
    }

    private void notifySpawn()
    {
        for(int i = 0; i<playerList.Length; i++)
        {
            playerList[i].GetComponent<PlayerController>().setRespawn();
        }
    }

    public void SpawnDoor()
    {
        if (isServer)
        {
            GameObject door = Instantiate(doorPrefab, new Vector3(3.6f, 0.1f, -0.5f), Quaternion.identity);
            NetworkServer.Spawn(door);
        }
    }

    public void spawnChest()
    {
        if (isServer)
        {
            GameObject spawnedChest = Instantiate(chest, new Vector3(3.6f, -5f, -1f), Quaternion.identity);
            NetworkServer.Spawn(spawnedChest);
        }
    }

    public void spawnWeapon()
    {
        if (isServer)
        {
            GameObject itemPick = items[Random.Range(0, items.Count)];
            GameObject itemDrop = Instantiate(itemPick, new Vector3(3.6f, -5f, -0.5f), Quaternion.identity);
            NetworkServer.Spawn(itemDrop);
        }
    }

    private void deleteAll()
    {
        if (isServer)
        {
            objectList = GameObject.FindObjectsOfType<GameObject>();
            for (int i = 0; i < objectList.Length; i++)
            {
                if (objectList[i].tag.Equals("crystal")|| objectList[i].tag.Equals("chest")|| objectList[i].tag.Equals("Sword")
                    || objectList[i].tag.Equals("Shield") || objectList[i].tag.Equals("Bow") || objectList[i].tag.Equals("Staff") || objectList[i].tag.Equals("fire"))
                {
                    Destroy(objectList[i].gameObject);
                }
            }
        }
    }

}
