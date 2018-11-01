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
        public GameObject enemy;
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

    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points referenced.");
        }

        waveCountdown = timeBetweenWaves;
        wavesCompleted = false;
    }

    void Update()
    {
        if (wavesCompleted)
        {
            if (!EnemyIsAlive())
            {
                //NO ENEMIES ARE REMAINING
                
            }
        }

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

    IEnumerator SpawnWave(Wave _wave)
    {
        state = SpawnState.spawning;

        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        WaveCompleted();
        yield break;
    }

    void WaveCompleted()
    {
        state = SpawnState.counting;
        waveCountdown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length - 1)
        {
            wavesCompleted = true;
            //WAVES HAVE BEEN COMPLETED
            Debug.Log("WAVES COMPLETED");
        }
        else
        {
            nextWave++;
        }
    }

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

    void SpawnEnemy(GameObject _enemy)
    {
        if (isServer)
        {
             Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
             enemy = Instantiate(_enemy, _sp.position, _sp.rotation);
             NetworkServer.Spawn(enemy);

        }

    }

}
