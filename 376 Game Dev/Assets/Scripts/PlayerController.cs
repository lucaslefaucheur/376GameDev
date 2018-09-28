using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour 
{
    public float speed;
    public float xMin, xMax, yMin, yMax;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
        rb.velocity = movement * speed;

        rb.position = new Vector3
            (
                Mathf.Clamp(rb.position.x, xMin, xMax),
                Mathf.Clamp(rb.position.y, yMin, yMax),
                0.0f
            );


    }
}
