using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    //private Components
    private Rigidbody2D rb;
    private GameObject player;
    
    public int WanderSpeed = 3; // default speed it can wander
    public int PatrolSpeed_X = 3; //default speed it can patrol x-axis
    public int PatrolSpeed_Y = 3; //default speed it can patrol y-axis
    public int ChaseSpeed = 3; //default speed it chases the player
    public float detectRange = 4f; //default distance it will detectPlayer()
    public float keepDistance = .1f; //distance the movement will stop to avoid pushing the player

    public enum Movement_Type { idle, patrol, sleep, attack_cqb ,wander,attack_ranged } //movement types 
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
    public bool rangedAttack = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();

        if (WanderSpeed <= 0)    //WanderSpeed can't be 0, use Movement_Type "idle" instead
        {
            WanderSpeed = 1;
        }
        if(PatrolSpeed_X <= 0 && PatrolSpeed_Y <= 0)   //PatrolSpeed can't be 0 for x and y, use Movement_Type "idle" instead
        {
            PatrolSpeed_X = 1;
        }
        
        
        
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
        if (DetectPlayer == true && player != null) //added check to make sure player exists
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
            if (player.activeSelf == false) //added this check to avoid errors when player dies
            {
                moveType = Movement_Type.wander;
                DirectionChange();
                return;
            }
                
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
        else if(moveType == Movement_Type.attack_ranged)
        {
            float seperation = Vector2.Distance(rb.transform.position, player.transform.position);
            if (seperation >= detectRange) //added keepDistance, will do nothing on x-axis
            {
                if (player.transform.position.y > rb.position.y)
                {
                    rb.AddForce(new Vector2(0, ChaseSpeed));
                }
                else if (player.transform.position.y < rb.position.y)
                {
                    rb.AddForce(new Vector2(0, -ChaseSpeed));
                }
                /*if (player.transform.position.x < rb.position.x)
                {
                    rb.AddForce(new Vector2(ChaseSpeed,0));
                }
                else if (player.transform.position.x > rb.position.x)
                {
                    rb.AddForce(new Vector2(-ChaseSpeed,0));
                }*/


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
        if (moveType == Movement_Type.wander)
        {
            DirectionChange();
        }
        else if(moveType == Movement_Type.patrol)
        {
            if(move_Right == true)
            {
                move_Right = false;
            }
            else if(move_Right == false)
            {
                move_Right = true;
            }
            if(move_Up == true)
            {
                move_Up = false;
            }
            else if(move_Up)
            {
                move_Up = true;
            }
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (moveType == Movement_Type.wander)
        {
            DirectionChange();
        }
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
        if (seperation > detectRange  && rangedAttack == true)
        {
            moveType = Movement_Type.attack_ranged;
            //print("CAN Range!");
        }
        else if (seperation <= detectRange)
        {
            moveType = Movement_Type.attack_cqb;
            //print("CAN CQB!");
        }
        else
        {
            moveType = previousType;
        }
    }
}
