using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManagerScript : MonoBehaviour {

    //load all the maps
    public List<GameObject> mapList = new List<GameObject>();

    private GameObject currentMap;


    // Use this for initialization
    void Start () {
        
        loadAllMap();
    }
	
	// Update is called once per frame
	void Update () {

        if(mapList.Count == 0)
        {
            loadAllMap();
        }
		
	}

    //load all maps
    private void loadAllMap()
    {
        Object[] objList = Resources.LoadAll("Map", typeof(Object));

        // Add object to map list
        foreach (Object obj in objList)
        {
            GameObject mapObject = (GameObject)obj;
            mapList.Add(mapObject);
        }
    }
    //get random map
    public void loadMap()
    {
        //instantiate random map
        currentMap = Instantiate(mapList[Random.Range(0, mapList.Count)], new Vector3(0.0f, 0.0f), Quaternion.identity);
        //remove map from list
        mapList.Remove(currentMap);

    }

}
