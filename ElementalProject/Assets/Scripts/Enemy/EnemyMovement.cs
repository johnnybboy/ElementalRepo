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
    public float detectRange = 8f; //default distance it will detectPlayer()
    public float keepDistance = 1f; //distance the movement will stop to avoid pushing the player
    public float smoothing = 1f; //This helps smooth out movement, keep around 1 for now

    public enum MOVE_TYPE { idle, patrol, sleep, wander, chase_melee, chase_ranged, fly }; //movement types 
    public MOVE_TYPE movementType;
    private MOVE_TYPE previousType;

    Vector2 startPos; //starting position
    Vector2 newDirection;   //new direction for movement

    public float patrolBoundsVertical; // how far vertical it can patrol
    public float patrolBoundsHorizontal; // how far horizontal it can patrol

    public bool isDetectingPlayer = true;    // will attempt to detect player by default
    public bool patrolsVertical = false; 
    public bool patrolsHorizontal = false;
    public bool isRangedEnemy = false;

    private bool movingHome = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        
        startPos = new Vector2(rb.position.x, rb.position.y);
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
            Wander();
        else if (movementType == MOVE_TYPE.patrol)
            Patrol();   //TODO
        else if (movementType == MOVE_TYPE.chase_melee)
            ChaseMelee();
        else if (movementType == MOVE_TYPE.chase_ranged)
            ChaseRanged();  //currently exactly like ChaseMelee();
        else if (movementType == MOVE_TYPE.idle)
            Idle();     //TODO
    }

    public void Wander()
    {
        rb.AddForce(newDirection);
        //this changes on collision
    }

    public void Patrol()
    {
        //TODO

        //private void MarchRight()
        //{
        //    rb.AddForce(new Vector2(PatrolSpeed_X, 0));
        //    if (rb.position.x >= startPos.x + partolRBounds)
        //    {
        //        move_Right = false;
        //    }

        //}
        //private void MarchLeft()
        //{
        //    rb.AddForce(new Vector2(-PatrolSpeed_X, 0));
        //    if (rb.position.x <= startPos.x - patrolLBounds)
        //    {
        //        move_Right = true;
        //    }

        //}
        //private void MarchUp()
        //{
        //    rb.AddForce(new Vector2(0, PatrolSpeed_Y));
        //    if (rb.position.y >= startPos.y + patrolUBounds)
        //    {
        //        move_Up = false;
        //    }

        //}
        //private void MarchDown()
        //{
        //    rb.AddForce(new Vector2(0, -PatrolSpeed_Y));
        //    if (rb.position.y <= startPos.y - partolDBounds)
        //    {
        //        move_Up = true;
        //    }

        //}
    }

    public void ChaseMelee()
    {
        float seperation = Vector2.Distance(rb.transform.position, player.transform.position);
        if (seperation <= keepDistance) //will only change y if within keepDistance
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

    public void ChaseRanged()       //TODO this is currently exactly like ChaseMelee() pretty much
    {
        float seperation = Vector2.Distance(rb.transform.position, player.transform.position);
        if (seperation <= keepDistance) //will only change y if within keepDistance
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
        if (Vector2.Distance(transform.position, startPos) > keepDistance && !movingHome)
            StartCoroutine(MoveHome());
    }

    IEnumerator MoveHome()
    {
        movingHome = true;
        while(Vector2.Distance(transform.position, startPos) > keepDistance)
        {
            transform.position = Vector2.Lerp(transform.position, startPos, smoothing * Time.deltaTime);
            yield return null;
        }
        movingHome = false;
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
        if (player == null)
        {
            Debug.Log("Cannot detectPlayer(), because player is null!");
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
