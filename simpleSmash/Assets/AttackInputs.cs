using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class AttackInputs : MonoBehaviour
{
    Player player;
    int playerID;

    PlayerMover movementData;

    public bool punching;
    public bool idling;
    public bool walking;
    public bool crouching;
    public bool jumping;
    public bool hit;
    public bool block;

    AnimatorScript animData;

    public HurtBox headBox;
    public HurtBox armBox;
    public HurtBox bodyBox;
    public HurtBox legBox;

    public HitBox hitBox;

    public bool currentState;

    public bool hitActive;

    public GameObject head;
    public GameObject arms;
    public GameObject body;
    public GameObject legs;

    BoxCollider2D[] colliders;

    // Start is called before the first frame update
    void Start()
    {
        //hitActive = true;//debug
        headBox = new HurtBox(.2f, new Vector3(0,1,0), new Vector3(.35f,.35f,.01f));
        armBox = new HurtBox(.2f, new Vector3(0, .6f, 0), new Vector3(1f, .5f, .01f));
        bodyBox = new HurtBox(-.06f, new Vector3(0, .35f, 0), new Vector3(.35f, 1f, .01f));
        legBox = new HurtBox(.04f, new Vector3(0, -.5f, 0), new Vector3(1f, 1f, .01f));


        head.AddComponent<BoxCollider2D>();
        arms.AddComponent<BoxCollider2D>();
        legs.AddComponent<BoxCollider2D>();
        body.AddComponent<BoxCollider2D>();

        movementData = GetComponent<PlayerMover>();
        animData = GetComponentInChildren<AnimatorScript>();
        playerID = int.Parse(this.gameObject.name[0].ToString());
        player = ReInput.players.GetPlayer(playerID);
    }

    void Update()
    {
        stateDetection();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HurtBoxAssigner();
        DrawColliders();
        HitDetection();
        //Debug.Log(idling);
    }

    void stateDetection()
    {
        idling = movementData.moveDirection.magnitude == 0 && !jumping && !punching;
        walking = movementData.moveDirection.x != 0 && !jumping && !crouching && !punching;
        jumping = !movementData.grounded && !punching;
        crouching = movementData.grounded && player.GetButton("Crouch") && !punching;
        if (player.GetButtonDown("Punch") && !punching && !crouching && !jumping)
        {
            punching = true;
        }
        hit = movementData.HIT;
        if (transform.rotation.eulerAngles.y == 0 && !hit)//facing right
        {
            block = (player.GetAxisRaw("Horizontal") < 0);
        }
        else
        {
            block = (player.GetAxisRaw("Horizontal") > 0);
        }
    }

    void HurtBoxAssigner()
    {
        if (idling)
        {
            headBox = new HurtBox(.15f, new Vector3(0, 1, 0), new Vector3(.35f, .35f, .01f));
            armBox = new HurtBox(.2f, new Vector3(0, .6f, 0), new Vector3(1f, .5f, .01f));
            bodyBox = new HurtBox(-.06f, new Vector3(0, .35f, 0), new Vector3(.35f, 1f, .01f));
            legBox = new HurtBox(.04f, new Vector3(0, -.5f, 0), new Vector3(1f, 1f, .01f));
        }
        if (jumping)
        {
            headBox = new HurtBox(0.02f, new Vector3(0, 1.19f, 0), new Vector3(.35f, .35f, .01f));
            armBox = new HurtBox(-0.2f, new Vector3(0, .6f, 0), new Vector3(1.2f, .6f, .01f));
            bodyBox = new HurtBox(-.17f, new Vector3(0, .35f, 0), new Vector3(.35f, 1f, .01f));
            legBox = new HurtBox(.04f, new Vector3(0, -.2f, 0), new Vector3(.6f, 1.4f, .01f));
        }
        if (crouching)
        {
            headBox = new HurtBox(.15f, new Vector3(0, .7f, 0), new Vector3(.35f, .35f, .01f));
            armBox = new HurtBox(.1f, new Vector3(0, .3f, 0), new Vector3(1f, .5f, .01f));
            bodyBox = new HurtBox(-.06f, new Vector3(0, 0f, 0), new Vector3(.35f, 1f, .01f));
            legBox = new HurtBox(.04f, new Vector3(0, -.5f, 0), new Vector3(1f, 1f, .01f));
        }
        if (walking)
        {
            headBox = new HurtBox(.15f, new Vector3(0, 1, 0), new Vector3(.35f, .75f, .01f));
            armBox = new HurtBox(.2f, new Vector3(0, .6f, 0), new Vector3(1f, .5f, .01f));
            bodyBox = new HurtBox(-.06f, new Vector3(0, .35f, 0), new Vector3(.35f, 1f, .01f));
            legBox = new HurtBox(.04f, new Vector3(0, -.5f, 0), new Vector3(1f, 1f, .01f));
        }
    }

    void DrawColliders()
    {
        head.transform.position = transform.position + transform.right * headBox.offset + headBox.center;
        head.transform.localScale = headBox.scale + new Vector3(0, -headBox.scale.y / 2, 0);

        arms.transform.position = transform.position + transform.right * armBox.offset + armBox.center;
        arms.transform.localScale = armBox.scale + new Vector3(0, -armBox.scale.y / 2, 0);

        legs.transform.position = transform.position + transform.right * legBox.offset + legBox.center;
        legs.transform.localScale = legBox.scale + new Vector3(0, -legBox.scale.y / 2, 0);

        body.transform.position = transform.position + transform.right * bodyBox.offset + bodyBox.center;
        body.transform.localScale = bodyBox.scale + new Vector3(0, -bodyBox.scale.y / 2, 0);
    }

    void HitDetection()
    {
        if (hitActive)
        {
            if (Physics2D.OverlapBox(transform.position + transform.right * hitBox.offset + hitBox.center, hitBox.scale, 0, movementData.layerMaskEXCLD))
            {
                if (!movementData.otherGuy.GetComponent<AttackInputs>().block)
                {
                    Debug.Log("PUNCHEE");
                    movementData.otherGuy.GetComponent<PlayerMover>().HIT = true;
                }
                else
                {
                    Debug.Log("BLOCKEED");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawCube(transform.position, Vector3.one);
        if(1==1)
        { 
        //hurtboxes
        Gizmos.DrawWireCube(transform.position+transform.right*headBox.offset+headBox.center, headBox.scale);//head
        Gizmos.DrawWireCube(transform.position+transform.right* armBox.offset+ armBox.center, armBox.scale);//arms
        Gizmos.DrawWireCube(transform.position+transform.right* bodyBox.offset+ bodyBox.center, bodyBox.scale);//body
        Gizmos.DrawWireCube(transform.position+transform.right* legBox.offset+ legBox.center, legBox.scale);//legs
        }
        if (hitActive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + transform.right * hitBox.offset + hitBox.center, hitBox.scale);//hitBox
        }
    }
}

public struct HurtBox
{
    public float offset;
    public Vector3 center, scale;

    public HurtBox(float _offset, Vector3 _center, Vector3 _scale)
    {
        offset = _offset;
        center = _center;
        scale = _scale;
    }

}

public struct HitBox
{
    public float offset;
    public Vector3 center, scale;

    public HitBox(float _offset, Vector3 _center, Vector3 _scale)
    {
        offset = _offset;
        center = _center;
        scale = _scale;
    }

}
