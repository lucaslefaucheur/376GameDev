using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
    public static GameController instance = null;           //Static instance of GameController which allows it to be accessed by other scripts
    public Text hpText;                                     //A reference to the UI text component that displays the player's HP
    public Text levelText;                                  //A reference to the UI text component that displays the current level number
    public GameObject levelTransition;                      //Background for levelText, while level is being set up
    private int level = 1;                                  //Current level number
    private bool settingUp = true;                          //Boolean to check if we're currently setting up game
    private int numOfPlayers = 0;                           //Keep track of the number of players spawn
    private GameObject player;
    public float loadTime = 5.0f;


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
       // levelText = GameObject.Find("LevelText").GetComponent<Text>();
       // levelText.text = "Level " + level;
       // levelTransition.SetActive(true);
        //Call the HideLevelTransition function with a delay
        Invoke("HideLevelTransition", 2f);


        //Setup level
    }

    public void AddPlayer()
    {
        numOfPlayers++;
        Debug.Log("Num of players: " + numOfPlayers);
    }

    public int getNumOfPLayers()
    {
        return numOfPlayers;
    }

    private int getLevel()
    {
        return level;
    }

    //Hides black image used between levels
    void HideLevelTransition()
    {
        //levelTransition.SetActive(false);
        settingUp = false;
    }

    //Update is called every frame
    void Update()
    {
        //make enemies move, attack, etc
    }

    //This is called each time a scene is loaded
    void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
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