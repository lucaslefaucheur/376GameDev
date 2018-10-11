using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManagerScript : MonoBehaviour {

    public GameObject[] bossList;

    private GameObject currentBoss;
    private int level;
    private int players;

	void Start () {
        //get level and amount of players
        //level = this.GetComponent<GameManagerScript>().GetLevel();
        //players = this.GetComponent<GameManagerScript>().GeNumPlayer();

        //instantiate random boss
        currentBoss = Instantiate(bossList[Random.Range(0, bossList.Length)], transform.position, Quaternion.identity);
        
        //send information to boss created
        currentBoss.GetComponent<BossScaleScript>().SetScale(level, players);

    }
	
	void Update () {
		//check if boss is dead, if so notify game manager
        if(currentBoss == null)
        {
            //this.GetComponent<GameManagerScript>().BossDeath();
        }
	}
}
