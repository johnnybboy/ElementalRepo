using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    Rigidbody2D rb;
    public int WanderSpeed = 3;
    public int PatrolSpeed = 3;
    public enum Movement_Type { idle, patrol, sleep, attack,wander }
    public Movement_Type moveType;
    Vector2 vel;
    Vector2 startPos;
    Vector2 newPos;
    public float patrolLBounds;
    public float partolRBounds;
    public bool move_Right = true;
    
    // Start is called before the first frame update
    void Start()
    {
        if(WanderSpeed <= 0 || PatrolSpeed <= 0)
        {
            WanderSpeed = 1;
            PatrolSpeed = 1;
        }
        rb = GetComponent<Rigidbody2D>();
        startPos = new Vector2(rb.position.x,rb.position.y);
        DirectionChange();
        //print(startPos); //to see if startPos is working
    }
    void DirectionChange()
    {
        int x = 0;
        int y = 0;
        while (x > -(WanderSpeed) / 2 && x < (WanderSpeed) / 2)
        {
            x = Random.Range(-WanderSpeed, WanderSpeed);
        }
        while (y > -1 && y < 1)
        {
            y = Random.Range(-WanderSpeed, WanderSpeed);
        }

        newPos = new Vector2(-x, y);
    }
    // Update is called once per frame
    void Update()
    {
        
        if(moveType == Movement_Type.patrol)
        {
            if(move_Right == true) 
            {
                MarchRight();
            }
            else
            {
                MarchLeft();
            }
            
        }
        else if(moveType == Movement_Type.wander)
        {
            rb.AddForce(newPos);
        }
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DirectionChange();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DirectionChange();
    }

    private void MarchRight()
    {
        rb.AddForce(new Vector2(PatrolSpeed, 0));
        if (rb.position.x >= startPos.x + partolRBounds)
        {
            move_Right = false;
        }
        
    }
    private void MarchLeft()
    {
        rb.AddForce(new Vector2(-PatrolSpeed, 0));
        if (rb.position.x <= startPos.x - patrolLBounds)
        {
            move_Right = true;
        }

    }
}
