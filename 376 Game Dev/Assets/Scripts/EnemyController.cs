using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    [SerializeField]
    Transform Target;
    [SerializeField]
    float FollowSpeed;
    [SerializeField]
    float FollowRange;

    void Update()
    {
        if (Target != null)
        {
            if (transform.position.x - Target.transform.position.x < FollowRange)
            {
                transform.LookAt(Target.position);
                transform.Rotate(new Vector3(0, 90, 180), Space.Self);
                transform.Translate(new Vector3(FollowSpeed * Time.deltaTime, 0, 0));

            }
        }
    }
}
