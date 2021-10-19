using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject player;
    
    public int WanderSpeed = 3; // default speed it can wander
    public int PatrolSpeed_X = 3; //default speed it can patrol x-axis
    public int PatrolSpeed_Y = 3; //default speed it can patrol y-axis
    public int ChaseSpeed = 3; //default speed it chases the player
    public float detectRange = 1.5f; //default distance it will detectPlayer()
    public float keepDistance = .1f; //distance the movement will stop to avoid pushing the player

    public enum Movement_Type { idle, patrol, sleep, attack_cqb ,wander } //movement types 
    public Movement_Type moveType;
    private Movement_Type previousType;

    Vector2 startPos; //starting position
    Vector2 newPos;   //new postiton
   
    public float patrolLBounds; // how far left it can patrol
    public float partolRBounds; // how far right it can patrol
    public float patrolUBounds; // how far up it can patrol
    public float partolDBounds; // how far down it can patrol

    public bool DetectPlayer = true;// will detect player by default
    public bool move_Right = true; // starts patrol in the right direction
    public bool move_Up = true;    // starts patrol in the up direction
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        if (WanderSpeed <= 0)    //WanderSpeed can't be 0, use Movement_Type "idle" instead
        {
            WanderSpeed = 1;
        }
        if(PatrolSpeed_X <= 0 && PatrolSpeed_Y <= 0)   //PatrolSpeed can't be 0 for x and y, use Movement_Type "idle" instead
        {
            PatrolSpeed_X = 1;
        }
        rb = GetComponent<Rigidbody2D>();
        
        
        startPos = new Vector2(rb.position.x,rb.position.y);
        DirectionChange();
        //print(startPos); //to see if startPos is working
    }
    public void DirectionChange()
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
        previousType = moveType;
        newPos = new Vector2(-x, y);
    }
    // Update is called once per frame
    void Update()
    {
        if (DetectPlayer == true)
        {
            detectPlayer();
        }
        if (moveType == Movement_Type.patrol)
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
        else if(moveType == Movement_Type.attack_cqb)
        {
            float seperation = Vector2.Distance(rb.transform.position, player.transform.position);
            if (seperation <= keepDistance) //added keepDistance, will do nothing on x-axis
            {
                if (player.transform.position.y > rb.position.y)
                {
                    rb.AddForce(new Vector2(0, ChaseSpeed));
                }
                else if (player.transform.position.y < rb.position.y)
                {
                    rb.AddForce(new Vector2(0, -ChaseSpeed));
                }
            }
            else
            {
                //transform.position = Vector2.MoveTowards(rb.transform.position, player.transform.position, 3 * Time.deltaTime);
                if (player.transform.position.x > rb.position.x)
                {
                    rb.AddForce(new Vector2(ChaseSpeed, 0));
                }
                else if (player.transform.position.x < rb.position.x)
                {
                    rb.AddForce(new Vector2(-ChaseSpeed, 0));
                }
                if (player.transform.position.y > rb.position.y)
                {
                    rb.AddForce(new Vector2(0, ChaseSpeed));
                }
                else if (player.transform.position.y < rb.position.y)
                {
                    rb.AddForce(new Vector2(0, -ChaseSpeed));
                }
            }
        }
        else if(moveType == Movement_Type.idle)
        {
            //transform.position = Vector2.MoveTowards(rb.position, startPos, 1 * Time.deltaTime);
            if (startPos.x > rb.position.x)
            {
                rb.AddForce(new Vector2(ChaseSpeed, 0));
            }
            else if (startPos.x < rb.position.x)
            {
                rb.AddForce(new Vector2(-ChaseSpeed, 0));
            }
            if (startPos.y > rb.position.y)
            {
                rb.AddForce(new Vector2(0, ChaseSpeed));
            }
            else if (startPos.y < rb.position.y)
            {
                rb.AddForce(new Vector2(0, -ChaseSpeed));
            }
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
    private void detectPlayer()
    {
        float seperation = Vector2.Distance(rb.transform.position, player.transform.position);
        if (seperation <= detectRange)
        {
            moveType = Movement_Type.attack_cqb;
            //print("CAN ATTACK!");
        }
        else
        {
            moveType = previousType;
        }
    }
}
