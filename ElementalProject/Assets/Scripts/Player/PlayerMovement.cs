using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //these references are public for editting in unity for ease
    private Animator animator;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private PlayerController controller;

    //settings
    public float moveSpeed = 3f;

    //conditions
    public bool facingRight = true;
    

    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.isAttacking)
        {
            //input key commands and add force
            if (Input.GetKey(KeyCode.W))
            {
                body.AddForce(new Vector2(0, moveSpeed));
            }
            if (Input.GetKey(KeyCode.S))
            {
                body.AddForce(new Vector2(0, -moveSpeed));
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (!facingRight)   //flip if not facing the correct direction
                {
                    flipSprite();
                }
                body.AddForce(new Vector2(moveSpeed, 0));
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (facingRight)    //flip if not facing the correct direction
                {
                    flipSprite();
                }
                body.AddForce(new Vector2(-moveSpeed, 0));
            }
        }

        //update speed for animator
        float speed = Mathf.Abs(body.velocity.x) + Mathf.Abs(body.velocity.y);
        animator.SetFloat("Speed", speed);
    }

    private void flipSprite()
    {
        facingRight = !facingRight;
        sprite.flipX = !sprite.flipX;
    }
}
