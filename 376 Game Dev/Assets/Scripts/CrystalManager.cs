using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CrystalManager : NetworkBehaviour
{
    public static CrystalManager instance = null;
    private int crystalCount;

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

    void InitGame()
    {
        crystalCount = 0;
    }

        public void addCrystal()
    {
        crystalCount++;
    }

    public int getCrystalCount()
    {
        return crystalCount;
    }

    //for revival
    public void crystalExchange()
    {
        crystalCount -= 5;
    }
}
