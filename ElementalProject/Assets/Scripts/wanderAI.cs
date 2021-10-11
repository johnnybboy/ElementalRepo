using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wanderAI : MonoBehaviour
{
    Rigidbody2D body;
    Vector2 newPosition;
    public int moveSpeed = 3;

    void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();
        DirectionChange();
    }

    void DirectionChange()
    {
        int x = 0;
        int y = 0;
        while(x > -(moveSpeed)/2 && x < (moveSpeed)/2)
        {
            x = Random.Range(-moveSpeed, moveSpeed);
        }
        while (y > -1 && y < 1)
        {
            y = Random.Range(-moveSpeed, moveSpeed);
        }

        newPosition = new Vector2(-x, y);
    }

    void Update()
    {
        body.AddForce(newPosition);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DirectionChange();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DirectionChange();
    }
}
