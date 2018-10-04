using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    //imported objects
    public Camera cam;

    //variable set up
    public float speed;

    //for internal referencing
    private Rigidbody2D playerRB;
    private Animator anim;
    private SpriteRenderer rendy;

    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rendy = GetComponent<SpriteRenderer>();
    }

    //On local player start only
    public override void OnStartLocalPlayer()
    {
        
    }

    void FixedUpdate()
    {
        //don't touch
        if (!isLocalPlayer)
        {
            cam.enabled = false;
            return;
        }

        Move();


    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal") * Time.deltaTime * 5.000001f; ;
        float moveVertical = Input.GetAxis("Vertical") * Time.deltaTime * 5.0f; ;
        if(Mathf.Abs(moveHorizontal)> Mathf.Abs(moveVertical))
        {
            anim.SetBool("moveSide", true);
            anim.SetBool("moveUp", false);
            anim.SetBool("moveDown", false);
            anim.SetBool("isMoving", true);
            if (moveHorizontal < 0)
            {
                rendy.flipX = true;
                // invoke the change on the Server as you already named the function
                CmdProvideFlipStateToServer(rendy.flipX);
            }
            else if (moveHorizontal > 0)
            {
                rendy.flipX = false;
                // invoke the change on the Server as you already named the function
                CmdProvideFlipStateToServer(rendy.flipX);
            }
        }
        else if(Mathf.Abs(moveHorizontal) < Mathf.Abs(moveVertical))
        {
            anim.SetBool("moveSide", false);
            anim.SetBool("isMoving", true);
            if (moveVertical > 0)
            {
            anim.SetBool("moveUp", true);
            anim.SetBool("moveDown", false);
            }

            else
            {
                anim.SetBool("moveUp", false);
                anim.SetBool("moveDown", true);
            }

        }
        else if(moveVertical == 0 && moveHorizontal == 0)
        {
            anim.SetBool("isMoving", false);
        }

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
        playerRB.transform.Translate(movement);
    }

    [Command]
    void CmdProvideFlipStateToServer(bool state)
    {
        // make the change local on the server
        rendy.flipX = state;

        // forward the change also to all clients
        RpcSendFlipState(state);
    }

    // invoked by the server only but executed on ALL clients
    [ClientRpc]
    void RpcSendFlipState(bool state)
    {
        // skip this function on the LocalPlayer 
        // because he is the one who originally invoked this
        if (isLocalPlayer) return;

        //make the change local on all clients
        rendy.flipX = state;
    }
}
