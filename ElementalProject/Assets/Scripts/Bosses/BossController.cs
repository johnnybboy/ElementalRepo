using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    //public components
    public GameObject coin, heart, potion;  //loot drops

    //private components
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;
    private ParticleSystem particles;
    private GameObject player;
    private AudioSource hurtSound, deathSound, idleSound;

    //stats and mutators
    public float maxHealth = 25f;
    public float stunTime = .5f;            //cannot attack while stunned for this amount of time
    public bool facePlayer = true;
    public float playerDetectRange = 10f;   //distance to detect player
    public float despawnTime = 2f;          //how long death should last before deletion

    public bool canMove = true;             //will this controller handle movement?
    public bool facingRight = false;
    public bool deathAnim = false;
    public bool particleDeath = false;      //if particles component exists, will play it at death
    public float particleDeathTime = 1f;    //how long sprite will be enabled during death

    private float currentHealth;
    private bool isAlive = true;

    //private conditions
    private bool canTakeDamage = true;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        particles = GetComponent<ParticleSystem>();
        player = GameObject.FindGameObjectWithTag("Player");

        //AudioSources
        if (transform.Find("AudioSources").Find("HurtSound") != null)
            hurtSound = transform.Find("AudioSources").Find("HurtSound").GetComponent<AudioSource>();
        if (transform.Find("AudioSources").Find("DeathSound") != null)
            deathSound = transform.Find("AudioSources").Find("DeathSound").GetComponent<AudioSource>();
        if (transform.Find("AudioSources").Find("IdleSound") != null)
            idleSound = transform.Find("AudioSources").Find("IdleSound").GetComponent<AudioSource>();
    }

    protected void Update()
    {
        if (isAlive)
        {
            if (canMove)
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
            }
            
            //flip sprite to face player
            if(PlayerDetected() && facePlayer)
            {
                if (player.transform.position.x <= transform.position.x) // looks left if player is left
                {
                    if (facingRight)
                    {
                        FlipFacing();
                    }
                }
                else if (player.transform.position.x > transform.position.x) //looks right if player is right
                {
                    if (facingRight != true)
                    {
                        FlipFacing();
                    }
                }
            }
        }
    }
    public IEnumerator TakeDamage(float damage)
    {
        if (canTakeDamage && isAlive)
        {
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
        }
    }

    IEnumerator Die()
    {
        //die in code
        isAlive = false;
        canTakeDamage = false;

        //disable Components
        GetComponent<Collider2D>().enabled = false;
        if (idleSound != null)
            idleSound.Stop();

        //play deathSound
        if (deathSound != null)  //make sure there's something to play
        {
            deathSound.Play();
        }

        //play death animation
        if (deathAnim)
        {
            animator.SetBool("isDead", true);
        }

        //particle death!
        if (particles != null && particleDeath)
        {
            particles.Play();
            yield return new WaitForSeconds(particleDeathTime);
            sprite.enabled = false;
        }
        
        //delete after some delay
        yield return new WaitForSeconds(despawnTime);

        //destroy this gameobject and spawn loot
        SpawnBossLoot();
        Destroy(gameObject);
    }

    public void FlipFacing()
    {
        facingRight = !facingRight;
        //sprite.flipX = !sprite.flipX;
        gameObject.transform.Rotate(0f, 180f, 0f);
    }

    void SpawnBossLoot()    //TODO make this a little more interesting
    {
        GameObject item = null;
        int loot = Random.Range(1, 7) + Random.Range(1, 7); //rolling 2d6 to increase odds of coins and hearts

        if (loot >= 3 && loot <= 6)
            item = coin;
        if (loot >= 8 && loot <= 11)
            item = heart;
        if (loot == 2 || loot == 12)
            item = potion;

        //make sure item is not null, instantiate it at current position
        if (item != null)
        {
            Instantiate(item, transform.position, transform.rotation);
        }  
    }

    public bool PlayerDetected()
    {
        //check to avoid errors
        if (player == null || !player.activeSelf)
        {
            return false;
        }

        float seperation = Vector2.Distance(body.transform.position, player.transform.position);

        return seperation <= playerDetectRange;
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public bool Alive()
    {
        return isAlive;
    }
}
