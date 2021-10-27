using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Components
    public Animator animator;
    public Rigidbody2D body;
    public SpriteRenderer sprite;

    //loot drops (MUST be assigned in Unity!)
    public GameObject coin, heart, potion;

    //stats
    public float maxHealth = 3f;
    public float despawnTime = 2f;

    public float damage_weak = .5f;
    public float damage_medium = 1f;
    public float damage_strong = 1.5f;

    public bool facingRight = false;
    public bool isAlive = true;

    private float currentHealth;

    //animation fields
    private float speed;

    //combat conditions
    //TODO
    //stunned, on fire, frozen, poisoned, vulnerable


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isAlive)
        {
            //animation for moving
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
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);

        // Play hurt animation
        animator.SetTrigger("hurt");
        body.velocity = new Vector2(0, 0);

        //TODO
        //knockback away from player based on damage

        if(currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        //die animation
        animator.SetBool("isDead", true);

        //disable enemy
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Movement>().enabled = false;
        
        //delete after some delay
        isAlive = false;
        yield return new WaitForSeconds(despawnTime);

        //destroy this gameobject and spawn loot
        SpawnLoot();
        Destroy(gameObject);
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
