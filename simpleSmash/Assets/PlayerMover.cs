using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerMover : MonoBehaviour
{
    float jumpHeight = 20;
    float speed = 8;
    float speedstandard = 10;

    int jumpNum = 2;
    int currentJump = 0;

    float grav = -1f;
    public bool grounded;

    public Vector3 moveDirection;

    public int layerMask;
    public int layerMaskEXCLD;

    int playerID;
    Player player;

    public Transform otherGuy;
    AttackInputs attackData;

    public bool HIT;

    // Start is called before the first frame update
    void Start()
    {
        HIT = false;    
        attackData = GetComponent<AttackInputs>();
        playerID = int.Parse(this.gameObject.name[0].ToString());
        player = ReInput.players.GetPlayer(playerID);
        maskSelect();
        moveDirection = new Vector3(0, 0, 0);
        GetOtherPlayer();
    }

    void GetOtherPlayer()
    {
        if (playerID == 1)
        {
            otherGuy = GameObject.Find("0Player").transform;
        }
        else
        {
            otherGuy = GameObject.Find("1Player").transform;
        }
    }

    void maskSelect()
    {
        if (playerID == 0)
        {
            layerMask = 1 << 8;
            layerMaskEXCLD = 1 << 9;
        }
        else if (playerID == 1)
        {
            layerMask = 1 << 9;
            layerMaskEXCLD = 1 << 8;
        }

        layerMask = ~layerMask;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        GroundCheck();
        if (otherGuy.position.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        //if (moveDirection.x > 0)
        //{
        //    transform.rotation = Quaternion.Euler(0, 0, 0);
        //}
        //else if (moveDirection.x < 0)
        //{
        //    transform.rotation = Quaternion.Euler(0, 180, 0);
        //}
    }

    void GroundCheck()
    {
        grounded = Physics2D.Raycast(transform.position, Vector2.down, 1.25f, layerMask);
    }

    void Movement()
    {
        moveDirection = new Vector3(moveDirection.x, moveDirection.y, 0);
        if (!attackData.crouching && grounded && !attackData.punching && !HIT)
        {
            moveDirection.x = player.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
        }
        else if (attackData.crouching || attackData.punching)
        {
            moveDirection.x = 0;
        }
        else if (HIT)
        {
            if (transform.rotation.eulerAngles.y == 0)
            {
                moveDirection.x = -2f * Time.deltaTime * speed;
            }
            else
            {
                moveDirection.x = 2f * Time.deltaTime * speed;
            }
        }
        moveDirection.y += grav * Time.deltaTime;

        if (Physics2D.OverlapCapsule(transform.position + new Vector3(0, moveDirection.y, 0), new Vector2(transform.localScale.x, transform.localScale.y), CapsuleDirection2D.Vertical, 0, layerMask))//yColl
        {
            currentJump = 0;
            if (moveDirection.y < 0)
            {
                moveDirection.y = 0;
            }
            else if (moveDirection.y > 0)
            {
                moveDirection.y = -.1f;
            }
            if (moveDirection.y >= 0 && player.GetButton("Jump") &&
                !Physics2D.OverlapCapsule(transform.position + new Vector3(0, moveDirection.y, 0), new Vector2(transform.localScale.x, transform.localScale.y), CapsuleDirection2D.Vertical, 0, layerMask))
            {
                moveDirection.y = jumpHeight * Time.deltaTime;
            }
        }

        //x wall

        if (Physics2D.OverlapCapsule(transform.position + new Vector3(moveDirection.x, 0, 0), new Vector2(transform.localScale.x, transform.localScale.y), CapsuleDirection2D.Vertical, 0, layerMask))//XColl
        {
            if (!Physics2D.OverlapCapsule(transform.position + new Vector3(moveDirection.x, .2f, 0), new Vector2(transform.localScale.x, transform.localScale.y), CapsuleDirection2D.Vertical, 0, layerMask))
            {
                moveDirection.x -= Time.deltaTime * 2;
            }
            else
            {
                if (!Physics2D.OverlapCapsule(transform.position + new Vector3(moveDirection.x / speed, 0, 0), new Vector2(transform.localScale.x, transform.localScale.y), CapsuleDirection2D.Vertical, 0, layerMask))
                {
                    moveDirection.x = moveDirection.x * Time.deltaTime;
                }
                else
                {
                    moveDirection.x = 0;
                }
            }
            if (attackData.jumping)
            {
                moveDirection.x *= -1;
            }
        }

        if (HIT)
        {
            moveDirection.y = Time.deltaTime * 2.5f;
        }
        //Debug.Log(moveDirection.x);
        moveDirection.y = Mathf.Clamp(moveDirection.y,-.5f, .33f);
        moveDirection.x = Mathf.Clamp(moveDirection.x,-.15f,.15f);
        transform.position += moveDirection;
    }
}
