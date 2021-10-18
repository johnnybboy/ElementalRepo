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
    public float despawnTime = 2f;

    private int currentHealth;
    private float timeOfDeath;
    private float speed;
    private bool isAlive = true;
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
            if (body.velocity.x >= 0.1f)
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
            //Delay despawn after death, destroy this gameobject and spawn loot
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
        body.velocity = new Vector2(0, 0);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //die animation
        animator.SetBool("isDead", true);

        //disable enemy
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Movement>().enabled = false;
        
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
        GameObject item = null;
        int loot = Random.Range(1, 7) + Random.Range(1, 7); //rolling 2d6 to increase odds of coins and hearts

        if (loot >= 3 && loot <= 6)
            item = coin;
        if (loot >= 7 && loot <= 11)
            item = heart;
        if (loot == 2 || loot == 12)
            item = potion;

        //make sure item is not null, instantiate it at this enemy's position
        if (item != null)
            Instantiate(item, transform.position, transform.rotation);
    }
}
