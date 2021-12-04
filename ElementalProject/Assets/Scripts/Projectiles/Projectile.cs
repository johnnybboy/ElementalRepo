using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D proj;
    private GameObject player;
    private SpriteRenderer sprite;
    public enum Projectile_Path { straight, curve, bounce, boomerang, Wave, homing }
    public Projectile_Path projectilePath;
    

    Vector2 startPos; //starting position

    private bool findDirection = true;
    private bool direction = true;
    private bool destroy = false;
    public float projSpeed = 1f;
    public float BounceRange = .6f;
    public float BounceFreq = 0f;
    public float boomeRange = 2.5f;
    public bool fly_Right = true; // starts patrol in the right direction
    public bool fly_Up = true;    // starts patrol in the up direction
    public float death_distance = 20;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        //player = GameObject.FindGameObjectWithTag("Player");
        //we'll want to start implementing the Tag system so we could have multiple player objects potentially
        //      For character switching, down the road
        
        proj = GetComponent<Rigidbody2D>();
        startPos = new Vector2(proj.position.x, proj.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        float seperation = Vector2.Distance(proj.transform.position, startPos);
        if (seperation > death_distance)
        {Destroy(gameObject);}

        if (projectilePath == Projectile_Path.straight)
        {
            if (findDirection == true)
            {
                direction = PDirectX();
                findDirection = false;
            }
            if (direction == false) //left
            {
                proj.AddForce(new Vector2(-projSpeed, 0f));
            }
            else if(direction == true) //right
            {
                proj.AddForce(new Vector2(projSpeed, 0f));
            }   
        }
        else if(projectilePath== Projectile_Path.bounce) //still working on it
        {
            if (findDirection == true)
            {
                direction = PDirectX();
                findDirection = false;
            }
            if (direction == false) //left
            {
                proj.AddForce(new Vector2(-projSpeed, 0f));
                bounce();
            }
            else if (direction == true) //right
            {
                proj.AddForce(new Vector2((projSpeed), 0f));
                bounce();
            }
        }
        else if (projectilePath == Projectile_Path.Wave) //still working on it
        {
            if (findDirection == true)
            {
                direction = PDirectX();
                findDirection = false;
            }
            if (direction == false) //left
            {
                proj.AddForce(new Vector2(-projSpeed, 0f));
                Wave();
            }
            else if (direction == true) //right
            {
                proj.AddForce(new Vector2((projSpeed), 0f));
                Wave();
            }
        }
        else if (projectilePath == Projectile_Path.boomerang)
        {
            if (findDirection == true)
            {
                direction = PDirectX();
                findDirection = false;
            }
            if (direction == false) //left
            {
                if(proj.position.x > startPos.x - boomeRange && destroy == false)
                {
                    proj.AddForce(new Vector2(-projSpeed, 0f));
                }
                else if(proj.position.x <= startPos.x - boomeRange)
                {
                    proj.AddForce(new Vector2(projSpeed, 0f));
                    destroy = true;
                }
                else if (proj.position.x > startPos.x && destroy == true)
                {
                    Destroy(gameObject);
                }
            }
            else if (direction == true) //right
            {
                if (proj.position.x < startPos.x + boomeRange && destroy == false)
                {
                    proj.AddForce(new Vector2(projSpeed, 0f));
                }
                else if (proj.position.x >= startPos.x + boomeRange)
                {
                    proj.AddForce(new Vector2(-projSpeed, 0f));
                    destroy = true;
                }
                else if (proj.position.x < startPos.x && destroy == true)
                {
                    Destroy(gameObject);
                }
            }
        }

    }
    public void FlipFacing()
    {
        fly_Right = !fly_Right;
        gameObject.transform.Rotate(0f, 180f, 0f);
    }
    private bool PDirectX()
    {
        if (player.transform.position.x < proj.position.x) // fires left
        {
            return false;
        }
        else if (player.transform.position.x > proj.position.x) //fires right
        {
            return true;
        }
        else { return false; }
    }
    private void bounce()
    {
        if(proj.position.y >= startPos.y - BounceRange)
        {
            print("GOING DOWN");
            proj.AddForce(new Vector2(0, -projSpeed*BounceFreq));
        }
        else if(proj.position.y < startPos.y - BounceRange)
        {
            print("SUPERMAN");
            proj.AddForce(new Vector2(0, projSpeed *BounceFreq));
        }

    }
    private void Wave()
    {
        print("Smile And Wave Boys, Smile And Wave");
        int UpDown = Random.Range(0, 1);
        if(UpDown == 0)
        { 
            startPos.y = startPos.y + .65f;
            projectilePath = Projectile_Path.bounce;
        }
        else if(UpDown == 1)
        {
            
            projectilePath = Projectile_Path.bounce;
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
