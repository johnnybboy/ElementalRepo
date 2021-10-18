using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    Rigidbody2D rb;
<<<<<<< Updated upstream
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
=======
    public GameObject player;
    
    public int WanderSpeed = 3; // default speed it can wander
    public int PatrolSpeed_X = 3; //default speed it can patrol x-axis
    public int PatrolSpeed_Y = 3; //default speed it can patrol y-axis
    public int ChaseSpeed = 3; //default speed it chases the player

    public enum Movement_Type { idle, patrol, sleep, attack_cqb ,wander } //movement types 
    public Movement_Type moveType;
    private Movement_Type previousType;

    Vector2 startPos; //starting position
    Vector2 newPos;   //new postiton
   
    public float patrolLBounds; // how far left it can patrol
    public float partolRBounds; // how far right it can patrol
    public float patrolUBounds; // how far up it can patrol
    public float partolDBounds; // how far down it can patrol
    
    public bool move_Right = true; // starts patrol in the right direction
    public bool move_Up = true;    // starts patrol in the up direction
>>>>>>> Stashed changes
    
    // Start is called before the first frame update
    void Start()
    {
<<<<<<< Updated upstream
        if(WanderSpeed <= 0 || PatrolSpeed <= 0)
        {
            WanderSpeed = 1;
            PatrolSpeed = 1;
=======
        
        
        if (WanderSpeed <= 0)    //WanderSpeed can't be 0, use Movement_Type "idle" instead
        {
            WanderSpeed = 1;
        }
        if(PatrolSpeed_X <= 0 && PatrolSpeed_Y <= 0)   //PatrolSpeed can't be 0 for x and y, use Movement_Type "idle" instead
        {
            PatrolSpeed_X = 1;
>>>>>>> Stashed changes
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
        previousType = moveType;
        newPos = new Vector2(-x, y);
    }
    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        
        if(moveType == Movement_Type.patrol)
=======
        detectPlayer();
        if (moveType == Movement_Type.patrol)
>>>>>>> Stashed changes
        {
            if(move_Right == true) 
            {
                MarchRight();
              
            }
            else
            {
                MarchLeft();
              
            }
<<<<<<< Updated upstream
            
=======
            if (move_Up == true)
            {
                MarchUp();
                
            }
            else
            {
                MarchDown();
                
            }

>>>>>>> Stashed changes
        }
        else if(moveType == Movement_Type.wander)
        {
            rb.AddForce(newPos);
        }
<<<<<<< Updated upstream
=======
        else if(moveType == Movement_Type.attack_cqb)
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
                rb.AddForce(new Vector2(0,ChaseSpeed));
            }
            else if (player.transform.position.y < rb.position.y)
            {
                rb.AddForce(new Vector2(0,-ChaseSpeed));
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
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
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
        if (seperation <= 5)
        {
            moveType = Movement_Type.attack_cqb;
            //print("CAN ATTACK!");
        }
        else
        {
            moveType = previousType;
        }
    }
>>>>>>> Stashed changes
}
