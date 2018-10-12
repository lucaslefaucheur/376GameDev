using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScaleScript : MonoBehaviour {

    private int level;
    private int players;

    public void SetScale(int l, int p)
    {
        level = l;
        players = p;
    }

    public int GetLevel()
    {
        return level;
    }
    public int GetNumPlayers()
    {
        return players;
    }
}
