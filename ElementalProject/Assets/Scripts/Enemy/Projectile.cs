using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D proj;
    private GameObject player;

    public enum Projectile_Type { straight, curve, bounce, boomerang, homing }
    public Projectile_Type projectileType;

    Vector2 startPos; //starting position

    private bool findDirection = true;
    private bool direction = true;
    private bool destroy = false;
    public float projSpeed = 1f;
    public float Bounce = .6f;
    public float boomeRange = 2.5f;
    public bool fly_Right = true; // starts patrol in the right direction
    public bool fly_Up = true;    // starts patrol in the up direction
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        //player = GameObject.FindGameObjectWithTag("Player");
        //we'll want to start implementing the Tag system so we could have multiple player objects potentially
        //      For character switching, down the road
        
        proj = GetComponent<Rigidbody2D>();
        startPos = new Vector2(proj.position.x, proj.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (projectileType == Projectile_Type.straight)
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
        else if(projectileType== Projectile_Type.bounce) //still working on it
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
            else if (direction == true) //right
            {
                proj.AddForce(new Vector2((projSpeed), 0f));
            }
        }
        else if (projectileType == Projectile_Type.boomerang)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
