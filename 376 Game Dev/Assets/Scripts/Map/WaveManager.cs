using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class WaveManager : NetworkBehaviour {

    public enum SpawnState { spawning, counting };
    private GameObject enemy;

    [System.Serializable]
    public class Wave
    {
        public string name;
        public GameObject[] enemy;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    private int nextWave = 0;

    public Transform[] spawnPoints;

    public float timeBetweenWaves = 5f;
    private float waveCountdown;

    private float searchCountdown = 1f;
    private bool wavesCompleted;

    private SpawnState state = SpawnState.counting;

    private int numberOfPlayer = 1;
    private MapManagerScript mapManager;
    private GameController scaler;
    private bool doorSpawned = false;

    void Start()
    {
        // Check spawn points exist
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points referenced.");
        }

        // Get map manager
        GameObject managerObject = GameObject.Find("Manager");
        if (managerObject != null)
        {
            mapManager = managerObject.GetComponent<MapManagerScript>();
            scaler = managerObject.GetComponent<GameController>();
        }
        if (managerObject == null)
        {
            Debug.Log("Cannot find 'Manager' script");
        }

        setPlayer(scaler.getNumOfPLayers());
        waveCountdown = timeBetweenWaves;
        wavesCompleted = false;
    }

    void Update()
    {
        //Waves are done
        if (wavesCompleted)
        {
            if (!EnemyIsAlive())
            {
                //NO ENEMIES ARE REMAINING
                if (!doorSpawned)
                {
                    mapManager.SpawnDoor();
                    doorSpawned = true;
                }
            }
        }

        //Next Wave
        if (waveCountdown <= 0 && !wavesCompleted)
        {
            if (state != SpawnState.spawning)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    }

    //Spawn monsters
    IEnumerator SpawnWave(Wave _wave)
    {
        state = SpawnState.spawning;

        for (int i = 0; i < _wave.count; i++)
        {
            //Multiply number of spawn by number of players
            for (int j = 0; j < numberOfPlayer; j++)
            {
                // If enemies in array then spawn them from array
                if (_wave.enemy.Length > 0)
                {
                    SpawnEnemy(_wave.enemy[Random.Range(0, _wave.enemy.Length)]);
                }
                // Else spawn a random enemy from the map manager script
                else
                {
                    Debug.Log("Random Enemy Spawned");
                    SpawnEnemy(mapManager.getRandomEnemy());
                }
            }
            yield return new WaitForSeconds(1f / _wave.rate);
            Debug.Log("Next");
        }

        WaveCompleted();
        yield break;
    }

    //Select next waves
    void WaveCompleted()
    {
        state = SpawnState.counting;
        waveCountdown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length - 1)
        {
            wavesCompleted = true;
            Debug.Log("WAVES COMPLETED");
        }
        else
        {
            nextWave++;
        }
    }

    //Check if enemies are alive with 1 second wait times
    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown < 0f)
        {
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }

        return true;
    }

    //Spawn enemy at random position
    void SpawnEnemy(GameObject _enemy)
    {
        if (isServer)
        {
            Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemy = Instantiate(_enemy, _sp.position, _sp.rotation);
            //Set new enemy attack and health
            enemy.GetComponent<Health>().currentAttackDamage = enemy.GetComponent<Health>().startingAttackDamage + (scaler.getLevel() - 1) * 5;
            enemy.GetComponent<Health>().startingHealth = enemy.GetComponent<Health>().startingHealth + (scaler.getLevel() - 1) * 5;
            NetworkServer.Spawn(enemy);
        }

    }

    //Set wave multiplier for number of players
    public void setPlayer(int numOfPlayer)
    {
        numberOfPlayer = numOfPlayer;
    }

}
