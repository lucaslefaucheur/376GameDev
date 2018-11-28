using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
    public static GameController instance = null;           //Static instance of GameController which allows it to be accessed by other scripts          
    private GameObject levelTransition;                      //Background for levelText, while level is being set up
    public int level = 30;                                  //Current level number
    private bool settingUp = true;                          //Boolean to check if we're currently setting up game
    private int numOfPlayers = 1;                           //Keep track of the number of players spawn
    private GameObject player;
    public float loadTime = 5.0f;
    public Transform[] PlayerSpawn;
    private int tempNum;
    private GameObject[] playersList;

    //Awake is always called before any Start functions
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

    //Initializes the game for each level
    void InitGame()
    {
        //While setting up the game, player shouldn't be able to perform actions
        settingUp = true;
        levelTransition = GameObject.Find("LevelImage");
        //Call the HideLevelTransition function with a delay
        Invoke("HideLevelTransition", 2f);
        //Setup level
    }

    //Update is called every frame
    void Update()
    {
        
    }

    
    public void AddPlayer()
    {
        numOfPlayers = FindPlayerNumber();
        Debug.Log("Num of players: " + numOfPlayers);
        if(numOfPlayers == 1)
        {
            GameObject.Find("Manager").GetComponent<MapManagerScript>().spawnChest(new Vector3(-6.94f, -5.61f, -1f));
        }
        if (numOfPlayers == 4)
        {
            GameObject.Find("Manager").GetComponent<MapManagerScript>().spawnChest(new Vector3(0.83f, -5.61f, -1f));
        }
    }

    private int FindPlayerNumber()
    {

        for(int j = 1; j <= 4; j++)
        {
            if (!InList(j))
            {
                tempNum = j;
                break;
            }
        }

        return tempNum;
    }

    private bool InList(int n)
    {
        playersList = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playersList.Length; i++)
        {
            if (n == playersList[i].GetComponent<PlayerController>().getNumber())
            {
                return true;
            }
        }
        return false;
    }

    public int getPlayerCount()
    {
        playersList = GameObject.FindGameObjectsWithTag("Player");
        return playersList.Length;
    }

    public int getNumOfPLayers()
    {
        return numOfPlayers;
    }

    public int getLevel()
    {
        return level;
    }

    //Hides black image used between levels
    void HideLevelTransition()
    {
        //levelTransition.SetActive(false);
        settingUp = false;
    }

    public void LevelUp()
    {
        level++;
        //InitGame();
    }

    //GameOver when the player reaches 0 HP
    public void GameOver()
    {
        // levelText.text = "GAME OVER";
        //levelTransition.SetActive(true);
        //Disable GameManager
        enabled = false;
    }
}