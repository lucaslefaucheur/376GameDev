using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapManagerScript : NetworkBehaviour
{

    public static MapManagerScript instance = null;
    public GameObject initialMapPrefab;
    private GameObject currentMap;
    private GameObject[] playerList;
    private Vector3[] playerInitialSpawn = { new Vector3(-11.2f, 0.8f, 0.0f), new Vector3(5.3f, 0.8f, 0.0f), new Vector3(-11.2f, -9.3f, 0.0f), new Vector3(5.3f, -9.3f, 0.0f) };
    private Vector3[] playerSpawnPoint = {new Vector3(-6.0f, -3.0f, 0.0f), new Vector3(-7.0f, -5.0f, 0.0f), new Vector3(-6.0f, -5.0f, 0.0f), new Vector3(-7.0f, -3.0f, 0.0f) };

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
            playerList = GameObject.FindGameObjectsWithTag("Player");
            //spawnPlayers(playerInitialSpawn);
            start = false;
        }

        if (!hasMap){

            mapPicker = Random.Range(0, 2);
            Debug.Log(mapPicker);
            if(mapPicker%2 == 0)
            {
                playerList = GameObject.FindGameObjectsWithTag("Player");
                loadEnemyMap();
                //spawnPlayers(playerSpawnPoint);
            }
            else
            {
                playerList = GameObject.FindGameObjectsWithTag("Player");
                loadBossMap();
                //spawnPlayers(playerSpawnPoint);
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
            loadAllBossMap();
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
    public void loadBossMap()
    {
        if (isServer)
        {
            //instantiate random map
            bossMapPick = mapBossList[Random.Range(0, mapBossList.Count)];
            currentMap = Instantiate(bossMapPick, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            NetworkServer.Spawn(currentMap);
            //remove map from list
            mapBossList.Remove(bossMapPick);
        }

    }

    //get random ENEMY map
    public void loadEnemyMap()
    {
        if (isServer)
        {
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
        currentEnemy = enemyList[Random.Range(0, enemyList.Count)];
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
        Destroy(currentMap);
        hasMap = false;
    }

    public void spawnPlayers(Vector3[] spawnPoints)
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i].transform.position = spawnPoints[i];
        }
    }

}
