using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManagerScript : MonoBehaviour {

    public List<GameObject> bossList = new List<GameObject>();

    private GameObject currentBoss;
    private int level;
    private int players;

	void Start () {
        //get level and amount of players
        //level = this.GetComponent<GameManagerScript>().GetLevel();
        //players = this.GetComponent<GameManagerScript>().GeNumPlayer();

        //Get all object from Boss folder
        Object[] objList = Resources.LoadAll("Boss", typeof(Object));
        // Add object to boss list
        foreach (Object obj in objList)
        {
            GameObject bossObject = (GameObject) obj;
            bossList.Add(bossObject);
        }
        //instantiate random boss
        currentBoss = Instantiate(bossList[Random.Range(0, bossList.Count)], transform.position, Quaternion.identity);
        
        //send information to boss created
        currentBoss.GetComponent<BossScaleScript>().SetScale(level, players);

    }
	
	void Update () {
		//check if boss is dead, if so notify game manager
        if(currentBoss == null)
        {
            bossList.Remove(currentBoss);
            //this.GetComponent<GameManagerScript>().BossDeath();
        }
	}
}
