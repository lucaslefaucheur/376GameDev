using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
    public static GameController instance = null;           //Static instance of GameController which allows it to be accessed by other scripts
    public Text hpText;                                     //A reference to the UI text component that displays the player's HP
    private Text levelText;                                 //A reference to the UI text component that displays the current level number
    private GameObject levelTransition;                     //Background for levelText, while level is being set up
    private int level = 1;                                  //Current level number
    private bool settingUp = true;                          //Boolean to check if we're setting up game


    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //Set instance to this
            instance = this;

        //If instance already exists and it's not this
        else if (instance != this)

            //Destroy this. This enforces the singleton pattern.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    //Initializes the game for each level
    void InitGame()
    {
        //While setting up the game, player shouldn't be able to perform actions
        settingUp = true;

        //Get a reference to the background
        levelTransition = GameObject.Find("LevelImage");

        //Get a reference to the text LevelText's text component
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //Set the text of levelText
        levelText.text = "Level " + level;

        //Block the player's view of the game during setup.
        levelTransition.SetActive(true);

        //Call the HideLevelTransition function with a delay in seconds of levelStartDelay.
        Invoke("HideLevelTransition", 2f);

        //Setup level
    }

    //Hides black image used between levels
    void HideLevelTransition()
    {
        //Disable the levelImage gameObject.
        levelTransition.SetActive(false);

        //Let player to move again.
        settingUp = false;
    }


    //Update is called every frame
    void Update()
    {
        //make enemies move, etc.
    }

    //This is called each time a scene is loaded.
    void OnLevelWasLoaded(int index)
    {
        //Add one to current level
        level++;
        //Call InitGame to initialize the level
        InitGame();
    }
}