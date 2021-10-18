using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject player;

    public enum Projectile_Type { curve, straight, bounce, boomerang}
    public Projectile_Type projectileType;

    Vector2 startPos; //starting position

    public float projSpeed;
    public bool move_Right = true; // starts patrol in the right direction
    public bool move_Up = true;    // starts patrol in the up direction
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        startPos = new Vector2(rb.position.x, rb.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        bool direction = playerDirection();
        if (projectileType == Projectile_Type.straight)
        {
            if(direction == false)
            {
                rb.AddForce(new Vector2(-projSpeed, 0));
            }
            if(direction == true)
            {
                rb.AddForce(new Vector2(projSpeed, 0));
            }
            
        }
    }

    private bool playerDirection()
    {
        if (player.transform.position.x < rb.position.x) // fires left
        {
            return false;
        }
        else if (player.transform.position.x > rb.position.x) //fires right
        {
            return true;
        }
        else { return false; }
    }
}
