using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour 
{
    public float speed;
    public float xMin, xMax, yMin, yMax;
    public GameObject sprite;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!isLocalPlayer)
        {
            sprite.GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    public override void OnStartLocalPlayer()
    {
       
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            sprite.GetComponent<SpriteRenderer>().color = Color.red;
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
