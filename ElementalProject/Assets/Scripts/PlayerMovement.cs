using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //these references are public for editting in unity for ease
    public Transform player;
    public Transform attackPoint;
    public Animator animator;
    public Rigidbody2D playerPhysics;
    public SpriteRenderer sprite;
    public float APXOffset = 0.5f;
    public float APYOffset = -0.1f;
    public float moveSpeed = 3f;


    private bool facingRight = true;
    private float speed;
    private Vector2 moveUp;
    private Vector2 moveDown;
    private Vector2 moveRight;
    private Vector2 moveLeft;

    void Start()
    {
        //set vectors to values based on moveSpeed
        moveUp = new Vector2(0, moveSpeed);
        moveDown = new Vector2(0, -moveSpeed);
        moveRight = new Vector2(moveSpeed, 0);
        moveLeft = new Vector2(-moveSpeed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //input key commands and add force
        if (Input.GetKey(KeyCode.W))
        {
            playerPhysics.AddForce(moveUp);
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerPhysics.AddForce(moveDown);
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (!facingRight)   //flip if not facing the correct direction
            {
                flipSprite();
            }
            playerPhysics.AddForce(moveRight);
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (facingRight)    //flip if not facing the correct direction
            {
                flipSprite();
            }
            playerPhysics.AddForce(moveLeft);
        }

        //update speed for animator
        speed = Mathf.Abs(playerPhysics.velocity.x) + Mathf.Abs(playerPhysics.velocity.y);
        animator.SetFloat("Speed", speed);
    }

    private void flipSprite()
    {
        facingRight = !facingRight;
        sprite.flipX = !sprite.flipX;

        //transform.Rotate(0f, 180f, 0f);

        if (facingRight)
        {
            attackPoint.position = new Vector2(player.position.x + APXOffset, player.position.y + APYOffset);
        }
        else
        {
            attackPoint.position = new Vector2(player.position.x - APXOffset, player.position.y + APYOffset);
        }

    }
}
