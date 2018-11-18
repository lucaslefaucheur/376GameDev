using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    private bool ScanForPlayer()
    {
        int playerInCol = 0;
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
        int numOfPlayers = GameObject.FindGameObjectsWithTag("Player").Length;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Player")
            {
                playerInCol++;
            }
        }

        return playerInCol == numOfPlayers;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.tag.Equals("Player"))
        {
            if (ScanForPlayer())
            {
                GameObject.Find("Manager").GetComponent<MapManagerScript>().notifyEntry();
            }
            else
            {
                //Display Waiting
                Debug.Log("Waiting for all players to be ready!");
            }
        }

    }
}
