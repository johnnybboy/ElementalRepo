using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
<<<<<<< HEAD
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
=======
    Rigidbody2D rd;
    public float moveSpeed = 6;
    
    Vector2 vel;

    // Start is called before the first frame update
    void Start()
    {
        rd = GetComponent<Rigidbody2D>();    
>>>>>>> parent of c5de816 (Merge branch 'main' of https://github.com/johnnybboy/ElementalRepo)
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
        
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
=======
        vel = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Virtical")).normalized * moveSpeed;
>>>>>>> parent of c5de816 (Merge branch 'main' of https://github.com/johnnybboy/ElementalRepo)
    }

    private void FixedUpdate()
    {
<<<<<<< HEAD
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

=======
        rd.MovePosition(rd.position + vel * Time.fixedDeltaTime);
>>>>>>> parent of c5de816 (Merge branch 'main' of https://github.com/johnnybboy/ElementalRepo)
    }
}
