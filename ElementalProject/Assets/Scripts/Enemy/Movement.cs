using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    Rigidbody2D rb;
    
    public int WanderSpeed = 3;
    public int PatrolSpeed_X = 3;
    public int PatrolSpeed_Y = 3;

    //public Transform player;

    public enum Movement_Type { idle, patrol, sleep, attack,wander }
    public Movement_Type moveType;
    
    Vector2 vel;
    Vector2 startPos;
    Vector2 newPos;
   
    public float patrolLBounds;
    public float partolRBounds;
    public float patrolUBounds;
    public float partolDBounds;
    
    public bool move_Right = true;
    public bool move_Up = true;
    
    // Start is called before the first frame update
    void Start()
    {
        if(WanderSpeed <= 0)
        {
            WanderSpeed = 1;
        }
        else if(PatrolSpeed_X <= 0 && PatrolSpeed_Y <= 0)
        {
            PatrolSpeed_X = 1;
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
            if (move_Up == true)
            {
                MarchUp();
            }
            else
            {
                MarchDown();
            }

        }
        else if(moveType == Movement_Type.wander)
        {
            rb.AddForce(newPos);
        }
        else if(moveType == Movement_Type.attack)
        {
           
        }
        else if(moveType == Movement_Type.idle)
        { }
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
        rb.AddForce(new Vector2(PatrolSpeed_X, 0));
        if (rb.position.x >= startPos.x + partolRBounds)
        {
            move_Right = false;
        }
        
    }
    private void MarchLeft()
    {
        rb.AddForce(new Vector2(-PatrolSpeed_X, 0));
        if (rb.position.x <= startPos.x - patrolLBounds)
        {
            move_Right = true;
        }

    }
    private void MarchUp()
    {
        rb.AddForce(new Vector2(0, PatrolSpeed_Y));
        if (rb.position.y >= startPos.y + patrolUBounds)
        {
            move_Up = false;
        }

    }
    private void MarchDown()
    {
        rb.AddForce(new Vector2(0, -PatrolSpeed_Y));
        if (rb.position.y <= startPos.y - partolDBounds)
        {
            move_Up = true;
        }

    }
}
