using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BossDrop : NetworkBehaviour
{

    private List<GameObject> items = new List<GameObject>();

    // Use this for initialization
    void Start () {
        Object[] objList = Resources.LoadAll("Item", typeof(Object));

        // Add object to map list
        foreach (Object obj in objList)
        {
            GameObject item = (GameObject)obj;
            items.Add(item);
        }
    }
	
    public void OnDestroy()
    {
        if (isServer)
        {
            //instantiate a single random item
            GameObject itemPick = items[Random.Range(0, items.Count)];
            GameObject itemDrop = Instantiate(itemPick, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            NetworkServer.Spawn(itemDrop);
        }
    }
}
