using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : NetworkBehaviour
{
    public static GameController instance = null;           //Static instance of GameController which allows it to be accessed by other scripts
    private int level = 1;                                  //Current level number
    public Text hpText;                                     //A reference to the UI text component that displays the player's HP

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
        //Load current level

    }

    //Update is called every frame
    void Update()
    {

    }
}