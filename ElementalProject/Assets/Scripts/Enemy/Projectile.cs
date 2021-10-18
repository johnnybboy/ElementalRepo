using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D proj;
    public GameObject player;

    public enum Projectile_Type { curve, straight, bounce, boomerang}
    public Projectile_Type projectileType;

    Vector2 startPos; //starting position

    private bool findDirection = true;
    private bool direction = true;

    public float projSpeed = 1;
    public bool move_Right = true; // starts patrol in the right direction
    public bool move_Up = true;    // starts patrol in the up direction
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        
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
                direction = PDirect();
                findDirection = false;
            }
            if (direction == false) //left
            {
                proj.AddForce(new Vector2(-5, 0));
            }
            else if(direction == true) //right
            {
                proj.AddForce(new Vector2(5, 0));
            }   
        }
        
    }

    private bool PDirect()
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
}
