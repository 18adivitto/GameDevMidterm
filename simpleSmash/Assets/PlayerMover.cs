using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerMover : MonoBehaviour
{
    float jumpHeight = 17;
    float speed = 4;
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

    bool bumpAway;

    public bool onRight;

    float speedmod;
    float groundspeed;
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
            layerMask = (1 << 10) | (1 << 12); //bitshift means exclude all except for 8
            layerMaskEXCLD = 1 << 9;
        }
        else if (playerID == 1)
        {
            layerMask = (1 << 11)|(1 << 12);//used for checking the player movement collisions
            layerMaskEXCLD = 1 << 8;//used for hit detection
        }

        //layerMask = ~layerMask;//eveything but 8
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        GroundCheck();
        if (otherGuy.position.x < transform.position.x)
        {
            onRight = true;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            onRight = false;
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
        if (!attackData.crouching  &&grounded&& !attackData.punching && !HIT)
        {
            moveDirection.x = player.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
            groundspeed = player.GetAxisRaw("Horizontal")*Time.deltaTime*speed;
        }
        else if (attackData.crouching || attackData.punching)
        {
            moveDirection.x = 0;
        }

        moveDirection.y += grav * Time.deltaTime;

        if (HIT)
        {
            moveDirection.y = Time.deltaTime * 4f;

                if (transform.rotation.eulerAngles.y == 0)
                {
                    moveDirection.x = -3f * Time.deltaTime * speed;
                }
                else
                {
                    moveDirection.x = 3f * Time.deltaTime * speed;
                }
        }

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
            if (moveDirection.y >= 0 && player.GetButton("Jump")&& transform.position.y<1 &&
                !Physics2D.OverlapCapsule(transform.position + new Vector3(0, moveDirection.y, 0), new Vector2(transform.localScale.x, transform.localScale.y), CapsuleDirection2D.Vertical, 0, layerMask))
            {
                moveDirection.y = jumpHeight * Time.deltaTime;
            }
        }

        //x wall
        if (!grounded && moveDirection.y == 0)
        {
            if (otherGuy.position.x < transform.position.x)
            {
                moveDirection.x = Time.deltaTime * 3;
                if (otherGuy.position.x > -4.23f)
                {
                    otherGuy.position += new Vector3(-Time.deltaTime * 10, 0);
                }
            }
            else
            {
                moveDirection.x = -Time.deltaTime * 3;
                if (otherGuy.position.x < 4.23f)
                {
                    otherGuy.position += new Vector3(Time.deltaTime * 10, 0);
                }
            }
        }
        else if (grounded && moveDirection.y==0 && transform.position.y > 1)
        {
            if (otherGuy.position.x < transform.position.x)
            {
                moveDirection.x = Time.deltaTime * 3;
            }
            else
            {
                moveDirection.x = -Time.deltaTime * 3;
            }
        }

        if (!grounded)
        {
            speedmod = 1.5f;
        }
        else
        {
            speedmod = 1f;
        }

        if (!HIT && !grounded)
        {
            moveDirection.x = groundspeed * speedmod;
        }

        if (Physics2D.OverlapCapsule(transform.position + new Vector3(moveDirection.x, 0, 0), new Vector2(transform.localScale.x, transform.localScale.y), CapsuleDirection2D.Vertical, 0, layerMask))//XColl
        {

            if (!Physics2D.OverlapCapsule(transform.position + new Vector3(moveDirection.x, .1f, 0), new Vector2(transform.localScale.x, transform.localScale.y), CapsuleDirection2D.Vertical, 0, layerMask))
            {
                moveDirection.x -= Time.deltaTime * 2;
            }
            else
            {
                if (!Physics2D.OverlapCapsule(transform.position + new Vector3(moveDirection.x / speed, 0, 0), new Vector2(transform.localScale.x, transform.localScale.y), CapsuleDirection2D.Vertical, 0, layerMask))
                {
                    //moveDirection.x = moveDirection.x * Time.deltaTime;
                    moveDirection.x = 0;
                }
                else
                {
                    moveDirection.x = 0;
                }
            }
        }
        if (Mathf.Abs(transform.position.x - otherGuy.position.x)<.7f && grounded)
        {
                if (!onRight)
                {                    
                    if (player.GetAxisRaw("Horizontal") > 0 && otherGuy.position.x< 4.23f)
                    {
                        otherGuy.transform.position += new Vector3(player.GetAxisRaw("Horizontal") * Time.deltaTime * speed, 0);
                    }
                    //Debug.Log("pushing");
                }
                else
                {
                    if (player.GetAxisRaw("Horizontal") < 0&& otherGuy.position.x > -4.23f)
                    {
                        otherGuy.transform.position += new Vector3(player.GetAxisRaw("Horizontal") * Time.deltaTime * speed, 0);
                    }
                    //Debug.Log("pushing");
                }
            
        }

        moveDirection.y = Mathf.Clamp(moveDirection.y,-.5f, .33f);
        moveDirection.x = Mathf.Clamp(moveDirection.x,-.15f,.15f);
        transform.position += moveDirection;
    }
}
