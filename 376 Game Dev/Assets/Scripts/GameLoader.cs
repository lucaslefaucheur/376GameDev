using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameLoader : NetworkBehaviour
{
    public GameObject gameManager;          //GameManager prefab to instantiate

    void Awake()
    {
        //Check if a GameManager has already been assigned to static variable GameManager.instance 
        if (GameController.instance == null)

            //Instantiate prefab
            Instantiate(gameManager);
    }
}