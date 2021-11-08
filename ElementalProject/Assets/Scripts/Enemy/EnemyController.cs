using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //Components
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;
    private AudioSource sound;
    private EnemyMovement detect;
    
    private ParticleSystem particles;
    
    public AudioClip hurtSound, deathSound;

    //loot drops (MUST be assigned in Unity!)
    public GameObject coin, heart, potion;
    public Transform FirePoint;
    public GameObject projectile;

    private GameObject player;
    public enum enemyType {Melee, Ranged, Both, Boss};
    public enemyType enemy_type;
    /*public enum Projectile_Path { straight, curve, bounce, boomerang, homing }
    public Projectile_Path projectilePath;*/
    //stats
    public float maxHealth = 3f;
    public float despawnTime = 2f;

    public float damage_weak = .5f;
    public float damage_medium = 1f;
    public float damage_strong = 1.5f;

    public float MeleeRange = 1.0f;
    public float RangeedRange = 6.0f;
    public float AttackDamageMelee = 1.0f;
    public float AttackDamageRanged = 1.0f;
    private float PreviousAttackTime;
    public float AttackDelayMelee = 1.0f;
    public float AttackDelayRanged = 1.0f;
    

    public bool facingRight = false;
    public bool isAlive = true;
    public bool particleDeath = false;

    private float currentHealth;

    //animation fields
    private float speed;

    //combat conditions
    //TODO
    //stunned, on fire, frozen, poisoned, vulnerable


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        detect = GetComponent<EnemyMovement>();
        particles = GetComponent<ParticleSystem>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (isAlive)
        {
            //animation for moving
            speed = Mathf.Abs(body.velocity.x) + Mathf.Abs(body.velocity.y);
            animator.SetFloat("speed", speed);

            //check and flip facing
            if (body.velocity.x >= 0.3f)
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
            if(detect.PlayerDetected() == true) // If it detects the player
            {
                if (player.transform.position.x < body.position.x) // looks left if player is left
                {
                    if (facingRight)
                    {
                        FlipFacing();
                    }
                }
                else if (player.transform.position.x > body.position.x) //looks right if player is right
                {
                    if (facingRight != true)
                    {
                        FlipFacing();
                    }
                    
                }
                if (enemy_type == enemyType.Ranged)
                {
                    
                    RangedAttack();

                }
                else if(enemy_type == enemyType.Melee)
                {
                    MeleeAttack();
                }
            }
            
        }
    }
    private void MeleeAttack()
    {
        
        float seperation = Vector2.Distance(body.transform.position, player.transform.position);
        if(seperation <= MeleeRange && Time.time >= PreviousAttackTime + AttackDelayMelee)
        {
            print("Hit em where the Sun don't shine!");
            player.SendMessage("TakeDamage",AttackDamageMelee); //does damage to player

            PreviousAttackTime = Time.time;
            animator.SetTrigger("attack");
        }
    }
    private void RangedAttack()
    {
        float seperation = Vector2.Distance(body.transform.position, player.transform.position);
        if(seperation <= RangeedRange && Time.time >= PreviousAttackTime + AttackDelayRanged)
        {
            print("Don't fire untill you see the whites of their eyes!");
            
            animator.SetTrigger("attack");
            PreviousAttackTime = Time.time;
            GameObject Projectile1 = Instantiate(projectile,FirePoint.transform.position,FirePoint.rotation);
            
        }

    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Play hurt animation and sound
        animator.SetTrigger("hurt");
        if (hurtSound != null)  //make sure there's something to play
        {
            sound.clip = hurtSound;
            sound.Play();
        }
        

        // stop movement briefly
        body.velocity = new Vector2(0, 0);

        //check if dead, die
        if(currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isAlive = false;

        //die animation and sound
        animator.SetBool("isDead", true);
        if (deathSound != null)  //make sure there's something to play
        { 
            sound.clip = deathSound;
            sound.Play();
        }

        //disable Components
        GetComponent<Collider2D>().enabled = false;
        if (GetComponent<Movement>() != null)
            GetComponent<Movement>().enabled = false;
        if (GetComponent<EnemyMovement>() != null)
            GetComponent<EnemyMovement>().enabled = false;

        //particle death!
        if (particles != null && particleDeath)
        {
            particles.Play();
            sprite.enabled = false;
        }
            
        
        //delete after some delay
        isAlive = false;
        yield return new WaitForSeconds(despawnTime);

        //destroy this gameobject and spawn loot
        SpawnLoot();
        Destroy(gameObject);
    }

    public void FlipFacing()
    {
        facingRight = !facingRight;
        //sprite.flipX = !sprite.flipX;
        transform.Rotate(0f, 180f, 0f);
    }

    void SpawnLoot()    //added chance (at a roll of 7) for no item to drop
    {
        GameObject item = null;
        int loot = Random.Range(1, 7) + Random.Range(1, 7); //rolling 2d6 to increase odds of coins and hearts

        if (loot >= 3 && loot <= 6)
            item = coin;
        if (loot >= 8 && loot <= 11)
            item = heart;
        if (loot == 2 || loot == 12)
            item = potion;

        //make sure item is not null, instantiate it at this enemy's position
        if (item != null)
            Instantiate(item, transform.position, transform.rotation);
    }
}
