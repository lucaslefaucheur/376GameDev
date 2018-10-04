using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyController : NetworkBehaviour {

    [SerializeField]
    private Transform Target;
    [SerializeField]
    float FollowSpeed;
    [SerializeField]
    float FollowRange;

    GameObject[] players;

    void Start()
    {
    }

    void Update()
    {

        players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length != 0)
        {
            Debug.Log(players.Length);
            CmdFollow();
        }
    }

    [Command]
    void CmdFollow()
    {
        gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);

        var closestPlayer = players[0];
        var dist = Vector3.Distance(transform.position, players[0].transform.position);
        for (var i = 0; i < players.Length; i++)
        {
            var tempDist = Vector3.Distance(transform.position, players[i].transform.position);
            if (tempDist > dist)
            {
                closestPlayer = players[i];

            }
        }
        //do something with the closestEnemy 
        transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), closestPlayer.transform.position, 1 * Time.deltaTime);
        gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);

    }

}
