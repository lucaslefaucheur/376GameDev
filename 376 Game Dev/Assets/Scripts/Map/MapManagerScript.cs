using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManagerScript : MonoBehaviour {

    public static MapManagerScript instance = null;
    //BOSS MAPS
    public List<GameObject> mapBossList = new List<GameObject>();
    private GameObject currentBossMap;
    //BOSS
    public List<GameObject> bossList = new List<GameObject>();
    private GameObject currentBoss;
    //ENEMY MAPS
    public List<GameObject> mapEnemyList = new List<GameObject>();
    private GameObject currentEnemyMap;
    //ENEMY
    public List<GameObject> enemyList = new List<GameObject>();
    private GameObject currentEnemy;

    private bool hasMap = false;
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
        loadAllBossMap();
        loadAllBoss();
        loadAllEnemyMap();
        loadAllEnemy();
    }
	
	// Update is called once per frame
	void Update () {

        if (!hasMap){

            mapPicker = Random.Range(0, 2);
            Debug.Log(mapPicker);
            if(mapPicker%2 == 0)
            {
                loadEnemyMap();
            }
            else
            {
                loadBossMap();
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
        //instantiate random map
        currentBossMap = Instantiate(mapBossList[Random.Range(0, mapBossList.Count)], new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        
        //remove map from list
        mapBossList.Remove(currentBossMap);

    }

    //get random ENEMY map
    public void loadEnemyMap()
    {
        //instantiate random map
        currentEnemyMap = Instantiate(mapEnemyList[Random.Range(0, mapEnemyList.Count)], new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        //remove map from list
        mapEnemyList.Remove(currentEnemyMap);

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


}
