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
    public float detectRange = 10f; //default distance it will detectPlayer()
    public float meleeDistance = 1f;    //TODO this should be based on the size of the enemy
    public float rangedDistance = 10f; //distance the movement will stop to avoid pushing the player
    public float smoothing = 1f; //This helps smooth out movement, keep around 1 for now

    public enum MOVE_TYPE { idle, patrol, sleep, wander, chase_melee, chase_ranged, fly }; //movement types 
    public MOVE_TYPE movementType;
    private MOVE_TYPE previousType;

    Vector2 startPos; //starting position
    Vector2 newDirection;   //new direction for movement

    public float patrolBounds_X = 5f; // how far vertical it can patrol
    public float patrolBounds_Y = 5f; // how far horizontal it can patrol
    public bool patrol_X = false;
    public bool patrol_Y = false;

    public bool isDetectingPlayer = true;    // will attempt to detect player by default
    public bool patrolsVertical = false; 
    public bool patrolsHorizontal = false;
    public bool isRangedEnemy = false;

    private bool movingTowardsTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();

        //meleeDistance = transform.position.magnitude;   //TODO is magnitude good enough? 
        
        startPos = new Vector2(rb.position.x, rb.position.y);
        previousType = movementType;
    }
    
    // Update is called once per frame
    void Update()
    {
        //first check if we are detecting the player
        if (isDetectingPlayer == true)
        {
            DetectPlayer();
        }

        //then update movement based on MOVE_TYPE
        if (movementType == MOVE_TYPE.wander)
            Wander();   //TODO not working
        else if (movementType == MOVE_TYPE.patrol)
            Patrol();   //TODO not working
        else if (movementType == MOVE_TYPE.chase_melee)
            ChaseMelee();
        else if (movementType == MOVE_TYPE.chase_ranged)
            ChaseRanged();  
        else if (movementType == MOVE_TYPE.idle)
            Idle();
    }

    public void Wander()
    {
        rb.AddForce(newDirection);
        //this changes on collision
    }

    public void Patrol()
    {
        if (patrol_X)
        {
            if (rb.position.x >= startPos.x + patrolBounds_X)
            {
                //Move towards starting position - patrolBoundsVertical (march left)
                StartCoroutine(MoveTowards(new Vector2(startPos.x - patrolBounds_X, startPos.y)));
            }
            else
            {
                //Move towards starting position + patrolBoundsVertical (march right)
                StartCoroutine(MoveTowards(new Vector2(startPos.x + patrolBounds_X, startPos.y)));
            }
        }
        
        if (patrol_Y)
        {
            if (rb.position.y >= startPos.y + patrolBounds_Y)
            {
                //Move towards starting position - patrolBoundsVertical (march left)
                StartCoroutine(MoveTowards(new Vector2(startPos.x, startPos.y - patrolBounds_Y)));
            }
            else
            {
                //Move towards starting position + patrolBoundsVertical (march right)
                StartCoroutine(MoveTowards(new Vector2(startPos.x, startPos.y + patrolBounds_Y)));
            }
        }

        if(!patrol_X && !patrol_X)
        {
            Debug.Log("Patrol bools are false! Set X or Y to patrol.");
        }
    }

    public void ChaseMelee()    //uses meleeDistance
    {
        float seperation = Vector2.Distance(rb.transform.position, player.transform.position);
        if (seperation <= meleeDistance) //will only change y if within keepDistance
        {
            if (player.transform.position.y > rb.position.y)
            {
                rb.AddForce(new Vector2(0, moveSpeed));
            }
            else if (player.transform.position.y < rb.position.y)
            {
                rb.AddForce(new Vector2(0, -moveSpeed));
            }
        }
        else
        {
            if (player.transform.position.x > rb.position.x)
            {
                rb.AddForce(new Vector2(moveSpeed, 0));
            }
            else if (player.transform.position.x < rb.position.x)
            {
                rb.AddForce(new Vector2(-moveSpeed, 0));
            }
            if (player.transform.position.y > rb.position.y)
            {
                rb.AddForce(new Vector2(0, moveSpeed));
            }
            else if (player.transform.position.y < rb.position.y)
            {
                rb.AddForce(new Vector2(0, -moveSpeed));
            }
        }
    }

    public void ChaseRanged()    //uses rangedDistance
    {
        float seperation = Vector2.Distance(rb.transform.position, player.transform.position);
        if (seperation <= rangedDistance) //will only change y if within keepDistance
        {
            if (player.transform.position.y > rb.position.y)
            {
                rb.AddForce(new Vector2(0, moveSpeed));
            }
            else if (player.transform.position.y < rb.position.y)
            {
                rb.AddForce(new Vector2(0, -moveSpeed));
            }
        }
    }

    public void Idle()
    {
        if (Vector2.Distance(transform.position, startPos) > rangedDistance && !movingTowardsTarget)
            StartCoroutine(MoveTowards(startPos));
    }

    IEnumerator MoveTowards(Vector2 target)
    {
        movingTowardsTarget = true;
        MOVE_TYPE startingType = movementType;

        //begin movement loop
        while(Vector2.Distance(transform.position, startPos) > meleeDistance)
        {
            //check if something has changed the movementType to break the loop
            if (movementType != startingType)
            {
                movingTowardsTarget = false;
                yield break;
            }
            
            //move towards target position every frame, with smoothing
            transform.position = Vector2.Lerp(transform.position, startPos, smoothing * Time.deltaTime);
            yield return null;
        }
        movingTowardsTarget = false;
    }

    public void DirectionChange()
    {
        float x = 0;
        float y = 0;
        //makes sure the random value is at least half of the movespeed in x
        while (x > -(moveSpeed) / 2f && x < (moveSpeed) / 2f)
        {
            x = Random.Range(-moveSpeed, moveSpeed);
        }
        //y direction doesn't matter as much, can be any value between negative and positive moveSpeed
        y = Random.Range(-moveSpeed, moveSpeed);

        //previousType = movementType; //????
        newDirection = new Vector2(x, y);
    }

    private void DetectPlayer()
    {
        //added check to avoid errors
        if (player == null || !player.activeSelf)
        {
            Debug.Log("Cannot detectPlayer(), because player is null or inactive!");
            if (movementType == MOVE_TYPE.chase_melee || movementType == MOVE_TYPE.chase_ranged)
                movementType = previousType;
            return;
        }

        float seperation = Vector2.Distance(rb.transform.position, player.transform.position);
        if (seperation <= detectRange)
        {
            previousType = movementType;    //store previous type to return to it later
            if (isRangedEnemy)
                movementType = MOVE_TYPE.chase_ranged;  //is ranged so chase ranged
            else
                movementType = MOVE_TYPE.chase_melee;   //otherwise chase melee
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (movementType == MOVE_TYPE.wander)
        {
            DirectionChange();
        }

        if (movementType == MOVE_TYPE.patrol)
        {
            //check if patroling vertical or horizontal
            //reverse direction based on the vertical or horizontal patrol
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (movementType == MOVE_TYPE.wander)
        {
            DirectionChange();
        }
    }
    
}
