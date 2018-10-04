using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyController : NetworkBehaviour {
    
    private Transform Target;
    private Transform loc;
    private LayerMask caster;
    private float FollowRange = 100;

    float FollowSpeed;

    void Start()
    {
        loc = transform;
        caster = 1 << LayerMask.NameToLayer("Player");
    }

    void FixedUpdate()
    {
        SearchForTarget();
        Follow();
    }

    void SearchForTarget()
    {
        if (!isServer)
        {
            return;
        }
        if (Target == null)
        {
            Collider2D [] hitColliders = Physics2D.OverlapCircleAll(loc.position, FollowRange, caster);
            if (hitColliders.Length > 0)
            {

                int randomint = Random.Range(0, hitColliders.Length);
                Target = hitColliders[randomint].transform;
            }
        }
        
    }

    void Follow()
    {
        if (Target!= null && isServer)
        {
            transform.position = Vector2.MoveTowards(new Vector2(loc.position.x, loc.position.y), Target.position, 1 * Time.deltaTime);
        }
       
        

    }

}
