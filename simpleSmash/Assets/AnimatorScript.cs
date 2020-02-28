using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorScript : MonoBehaviour
{

    public Sprite[] idleFrames;
    public Sprite[] walkFrames;
    public Sprite[] jumpFrames;
    public Sprite[] crouchFrames;
    public Sprite[] punchFrames;
    public Sprite[] hitFrames;

    SpriteRenderer SR;
    AttackInputs inputData;
    PlayerMover moveData;
    public static int frames = 0;
    bool animStarted;

    int idleNum = 0;
    int walkNum = 0;
    int crouchNum = 0;
    int jumpNum = 0;
    int punchNum = 0;
    int hitNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        animStarted = false;
        frames = 0;
        inputData = GetComponentInParent<AttackInputs>();
        moveData = GetComponentInParent<PlayerMover>();
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (inputData.idling)
        {
            idleNum++;
            if (idleNum > idleFrames.Length-1)
            {
                idleNum = 0;
            }
            SR.sprite = idleFrames[idleNum];
        }
        else
        {
            idleNum = 0;
        }

        if (Mathf.Abs(inputData.horInput)>0)//inputData.walking)
        {
            walkNum++;
            if (walkNum > walkFrames.Length-1)
            {
                walkNum = 0;
            }
            SR.sprite = walkFrames[walkNum];
        }
        else
        {
            walkNum = 0;
        }

        if (inputData.crouching)
        {
            SR.sprite = crouchFrames[crouchNum];
        }

        if (inputData.jumping)
        {
            jumpNum++;
            if (jumpNum > jumpFrames.Length - 1)
            {
                jumpNum = jumpFrames.Length - 1;
            }
            SR.sprite = jumpFrames[jumpNum];
        }
        else
        {
            jumpNum = 0;
        }

        if (inputData.punching)
        {
            punchNum++;
            if (punchNum > 1 && punchNum < 4)
            {
                inputData.hitActive = true;
                inputData.hitBox = new HitBox(.6f, new Vector3(0f, .45f, 0f), new Vector3(.8f, .2f, .01f));
            }
            else
            {
                inputData.hitActive = false;
            }
            if (punchNum > punchFrames.Length - 1)
            {
                inputData.punching = false;
                punchNum = punchFrames.Length - 1;
            }
            SR.sprite = punchFrames[punchNum];
        }
        else
        {
            punchNum = 0;
        }

        if (moveData.HIT)
        {
            hitNum++;
            if (hitNum > hitFrames.Length-1)
            {
                moveData.HIT = false;
                hitNum = hitFrames.Length - 1;
            }
            SR.sprite = hitFrames[hitNum];
        }
        else
        {
            hitNum = 0;
        }
    }
}
