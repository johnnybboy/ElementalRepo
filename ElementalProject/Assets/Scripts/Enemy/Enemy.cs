using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D body;
    public SpriteRenderer sprite;

    public GameObject coin;
    public GameObject heart;
    public GameObject potion;

    public int maxHealth = 100;
    public float despawnTime = 2000f;
    float timeOfDeath;
    int currentHealth;
    float speed;
    bool isAlive = true;
    
    private bool facingRight;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        facingRight = false;
    }

    private void Update()
    {
        if (isAlive)
        {
            // animation for moving
            speed = Mathf.Abs(body.velocity.x) + Mathf.Abs(body.velocity.y);
            animator.SetFloat("speed", speed);

            //check and flip facing
            if (body.velocity.x >= 0)
            {
                if (facingRight != true)
                {
                    FlipFacing();
                }
            }
            else
            {
                if (facingRight)
                {
                    FlipFacing();
                }
            }
        }
        else
        {
            if (Time.time > timeOfDeath + despawnTime)
            {
                SpawnLoot();
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hurt animation
        animator.SetTrigger("hurt");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //die animation
        animator.SetBool("isDead", true);
        GetComponent<SpriteRenderer>().color = Color.red;

        //disable enemy
        GetComponent<Collider2D>().enabled = false;
        GetComponent<wanderAI>().enabled = false;
        
        //delete after some delay
        isAlive = false;
        timeOfDeath = Time.time;
    }

    void FlipFacing()
    {
        facingRight = !facingRight;
        sprite.flipX = !sprite.flipX;
    }

    void SpawnLoot()
    {
        GameObject item = new GameObject();
        int loot = Random.Range(0, 3);

        if (loot == 0)
        {
            item = coin;
        }
        if (loot == 1)
        {
            item = heart;
        }
        if (loot == 2)
        {
            item = potion;
        }
        if (loot == 3)
        {
            Debug.Log("Case 3!!");
        }

        Instantiate(item, transform.position, transform.rotation);
    }
}
