using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum enemyType {Melee, Ranged, Both, Boss};
    public enemyType enemy_type = enemyType.Melee;  //defaults to Melee

    //stats and mutators
    public float maxHealth = 3f;
    public float currentHealth;
    public float despawnTime = 2f;

    public float damage = .5f;

    public float meleeAttackRange = 1.0f;
    public float rangedAttackRange = 6.0f;

    public float AttackDelayMelee = 1.0f;
    public float AttackDelayRanged = 1.0f;

    //combat conditions
    public float stunTime = .5f;    //enemies cannot attack while stunned for this amount of time
    public bool facingRight = false;
    public bool isAlive = true;
    public bool particleDeath = false;


    private bool stunned = false;   //stuns enemies when taking damage for short time
    private bool canAttack = true;
    private bool canTakeDamage = true;

    //public components
    public GameObject coin, heart, potion;  //loot drops
    public GameObject projectile;   //projectile to make RangedAttacks with
    
    //private components
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;
    private AudioSource sound;
    private EnemyMovement detect;
    private ParticleSystem particles;
    private AudioSource hurtSound, deathSound, magicSound;
    private Transform firePoint;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
        detect = GetComponent<EnemyMovement>();
        particles = GetComponent<ParticleSystem>();
        player = GameObject.FindGameObjectWithTag("Player");

        //AudioSources are GetChild(1), all prefabs should have these children objects
        if (transform.childCount >= 2) {
            hurtSound = this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<AudioSource>();
            deathSound = this.gameObject.transform.GetChild(1).GetChild(1).GetComponent<AudioSource>();
        }

        //FirePoint is GetChild(2)
        if (transform.childCount >= 3) //make sure its not null
            firePoint = this.gameObject.transform.GetChild(2).gameObject.transform;
    }

    private void Update()
    {
        if (isAlive)
        {
            //animate movement if moving
            animator.SetFloat("speed", (Mathf.Abs(body.velocity.x) + Mathf.Abs(body.velocity.y)));

            //check and flip facing
            if (body.velocity.x >= 0.3f)
            {
                if (facingRight != true)
                {
                    FlipFacing();
                }
            }
            else if (body.velocity.x <= -0.3f)
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
                    
                    StartCoroutine(RangedAttack());

                }
                else if(enemy_type == enemyType.Melee)
                {
                    StartCoroutine(MeleeAttack());
                }
            }
            
        }
    }
    IEnumerator MeleeAttack()
    {
        if (canAttack && !stunned)
        {
            canAttack = false;
            float seperation = Vector2.Distance(body.transform.position, player.transform.position);
            if (seperation <= meleeAttackRange)
            {
                //attack if within range, call player's TakeDamage() method.
                animator.SetTrigger("attack");
                player.SendMessage("TakeDamage", damage);

                //wait until attack delay ends, after allow attacking again
                yield return new WaitForSeconds(AttackDelayMelee);
            }
            canAttack = true;
        }
        
    }

    IEnumerator RangedAttack()
    {
        if (canAttack && !stunned)
        {
            canAttack = false;
            float seperation = Vector2.Distance(body.transform.position, player.transform.position);
            if (seperation <= rangedAttackRange)
            {
                //play animation, wait for animation to finish
                animator.SetTrigger("attack");
                yield return new WaitForSeconds(.3f);

                //fire projectile, wait for attack delay, after allow attacking again
                Instantiate(projectile, firePoint.position, transform.rotation);
                yield return new WaitForSeconds(AttackDelayRanged);
            }
            canAttack = true;
        }
    }
    public IEnumerator TakeDamage(float damage)
    {
        if (canTakeDamage && isAlive)
        {
            stunned = true;
            currentHealth -= damage;

            // Play hurt animation and sound
            animator.SetTrigger("hurt");
            if (hurtSound != null)  //make sure there's something to play
            {
                hurtSound.Play();
            }


            // stop movement briefly
            body.velocity = new Vector2(0, 0);

            //check if dead, die
            if (currentHealth <= 0)
            {
                StartCoroutine(Die());
            }

            yield return new WaitForSeconds(stunTime);
            stunned = false;
        }
    }

    IEnumerator Die()
    {
        //die in code
        isAlive = false;

        //die animation and sound
        animator.SetBool("isDead", true);
        if (deathSound != null)  //make sure there's something to play
        { 
            deathSound.Play();
        }

        //disable Components
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EnemyMovement>().enabled = false;

        //particle death!
        if (particles != null && particleDeath)
        {
            particles.Play();
            sprite.enabled = false;
        }
            
        
        //delete after some delay
        yield return new WaitForSeconds(despawnTime);

        //destroy this gameobject and spawn loot
        SpawnLoot();
        Destroy(gameObject);
    }

    public void FlipFacing()
    {
        facingRight = !facingRight;
        //sprite.flipX = !sprite.flipX;
        gameObject.transform.Rotate(0f, 180f, 0f);
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
