using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyMovement : MonoBehaviour
{
    //private Components
    private Rigidbody2D rb;
    private EnemyController controller;
    private GameObject player;

    public float moveSpeed = 3; // default speed it can move
    public float chaseSpeed = 3; //default speed it chases the player
    public float detectRange = 5f; //default distance it will detectPlayer()
    public float keepDistance = .5f;    //distance all X movement will stop to keep distance from the player

    public enum MOVE_TYPE { idle, patrol, sleep, wander, chase, fly }; //movement types 
    public MOVE_TYPE movementType = MOVE_TYPE.wander;
    private MOVE_TYPE currentType;

    private Vector2 startPos; //starting position
    private Vector2 wanDirection;   //new wander direction for Wander()

    public float patrolBounds_X = 0f; // how far vertical it can patrol
    public float patrolBounds_Y = 0f; // how far horizontal it can patrol
    public float patrolDelay = 0f;

    public bool isDetectingPlayer = true;    // will attempt to detect player by default
    
    private bool movingTowardsTarget = false;
    private bool isPatrolling = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        controller = GetComponent<EnemyController>();
        rb = GetComponent<Rigidbody2D>();
        
        //initialize variables
        startPos = rb.position;
        wanDirection = RandomDirection(moveSpeed);
        currentType = movementType;
    }
    
    // Update is called once per frame
    void Update()
    {
        //if detecting the player, chase them
        if (isDetectingPlayer && PlayerDetected())
        {
            currentType = MOVE_TYPE.chase;
        }
        else
        {
            currentType = movementType;
        }

        //then update movement based on currentType's MOVE_TYPE
        if (currentType == MOVE_TYPE.wander)
            Wander();
        else if (currentType == MOVE_TYPE.patrol)
            Patrol();
        else if (currentType == MOVE_TYPE.chase)
            Chase();
        else if (currentType == MOVE_TYPE.idle)
            Idle();
    }

    public void Wander()    //applies newDirection to the enemy's movement
    {
        rb.AddForce(wanDirection);  
        //this changes on collision only, methods below
    }

    public void Patrol()
    {
        if (!isPatrolling)
        {
            //patrol path is based on two points, which use patrolBounds to create
            Vector2 patrolPos1 = new Vector2(startPos.x - patrolBounds_X, startPos.y - patrolBounds_Y);
            Vector2 patrolPos2 = new Vector2(startPos.x + patrolBounds_X, startPos.y + patrolBounds_Y);
            StartCoroutine(PatrolBetween(patrolPos1, patrolPos2, patrolDelay));
        }
    }

    public void Chase()    //uses keepDistance
    {
        //determine seperation distance between this and the player
        float seperation = Vector2.Distance(rb.transform.position, player.transform.position);

        //check if within keepDistance
        if (seperation <= keepDistance)
        {
            //move along side the player's position on y only while within keepDistance
            if (player.transform.position.y > rb.position.y)
            {
                rb.AddForce(new Vector2(0, chaseSpeed));
            }
            else if (player.transform.position.y < rb.position.y)
            {
                rb.AddForce(new Vector2(0, -chaseSpeed));
            }
        }
        else
        {
            //move towards the player's positon on x and y using AddForce()
            if (player.transform.position.x > rb.position.x)
            {
                rb.AddForce(new Vector2(chaseSpeed, 0));
            }
            else
            {
                rb.AddForce(new Vector2(-chaseSpeed, 0));
            }

            if (player.transform.position.y > rb.position.y)
            {
                rb.AddForce(new Vector2(0, chaseSpeed));
            }
            else
            {
                rb.AddForce(new Vector2(0, -chaseSpeed));
            }
        }
    }

    public void Idle()
    {
        //uses keepDistance for now as an area around startPos
        if (Vector2.Distance(transform.position, startPos) > keepDistance && !movingTowardsTarget)
            StartCoroutine(MoveTowards(startPos, moveSpeed));
    }

    public Vector2 RandomDirection(float speed)
    {
        float x = 0;
        float y = 0;
        //makes sure the random value is at least half of the movespeed in x
        while (x > -(speed) / 2f && x < (speed) / 2f)
        {
            x = Random.Range(-speed, speed);
        }
        //y direction doesn't matter as much, can be any value between negative and positive moveSpeed
        y = Random.Range(-speed, speed);
        
        return new Vector2(x, y);
    }

    public bool PlayerDetected()
    {
        //added check to avoid errors
        if (player == null || !player.activeSelf)
        {
            Debug.Log("Cannot detectPlayer(), because player is null or inactive!");
            return false;
        }

        float seperation = Vector2.Distance(rb.transform.position, player.transform.position);

        return seperation <= detectRange;
    }

    IEnumerator MoveTowards(Vector2 target, float speed)
    {
        movingTowardsTarget = true;
        MOVE_TYPE startingType = currentType;

        while (Vector2.Distance(transform.position, target) > .5f && controller.isAlive)
        {
            if (currentType != startingType)    //break loop if movementType changes
            {
                movingTowardsTarget = false;
                yield break;
            }


            //move towards target position using AddForce()
            if (target.x > rb.position.x)
            {
                rb.AddForce(new Vector2(speed, 0));
            }
            else
            {
                rb.AddForce(new Vector2(-speed, 0));
            }

            if (target.y > rb.position.y)
            {
                rb.AddForce(new Vector2(0, speed));
            }
            else
            {
                rb.AddForce(new Vector2(0, -speed));
            }

            yield return null;
        }

        movingTowardsTarget = false;
    }

    IEnumerator PatrolBetween(Vector2 first, Vector2 second, float waitTime)
    {
        isPatrolling = true;
        Vector2 target = first; //moves towards this target first

        while (currentType == MOVE_TYPE.patrol && controller.isAlive) //break the loop if no longer patrolling
        {
            if (!movingTowardsTarget)
            {
                StartCoroutine(MoveTowards(target, moveSpeed));
            }

            //check if close enough to switch target
            if (Vector2.Distance(transform.position, target) < 0.5f)
            {
                if (target != second)
                    target = second;
                else
                    target = first;

                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }

        //end
        isPatrolling = false;
    }

    IEnumerator ResetPosition()
    {
        //store previousType to return to after, set movement to idle to return to startPos
        MOVE_TYPE previousType = movementType;
        movementType = MOVE_TYPE.idle;
        currentType = MOVE_TYPE.idle;

        while (Vector2.Distance(transform.position, startPos) > .5f && controller.isAlive)
        {
            yield return null;
        }
        //once we have arrived at startPos, return to previousType
        movementType = previousType;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentType == MOVE_TYPE.wander)
        {
            wanDirection = RandomDirection(moveSpeed);
        }

        if (currentType == MOVE_TYPE.patrol)    //attempt to return to startPos
        {
            StartCoroutine(ResetPosition());
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (currentType == MOVE_TYPE.wander)
        {
            wanDirection = RandomDirection(moveSpeed);
        }

        if (currentType == MOVE_TYPE.patrol)    //attempt to return to startPos
        {
            StartCoroutine(ResetPosition());
        }
    }
    
}
