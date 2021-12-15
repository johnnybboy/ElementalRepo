using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    //private components
    protected Rigidbody2D body;
    protected SpriteRenderer sprite;
    protected Animator animator;
    protected ParticleSystem particles;
    protected AudioSource hurtSound, deathSound, attackSound, idleSound;
    protected GameObject player;

    //private variables
    protected float currentHealth;
    protected bool isAlive = true;
    protected bool stunned = false;   //can't do actions while stunned
    protected bool canAttack = true;
    protected bool canTakeDamage = true;
    protected bool movingTowardsTarget = false;
    protected bool isPatrolling = false;

    protected Vector2 startPos;       //starting position
    protected Vector2 wanDirection;   //new wander direction for Wander()
    protected MOVE_TYPE currentType;

    [Tooltip("These GameObjects have a chance to drop on death.")]
    public GameObject commonDrop1, commonDrop2, rareDrop;

    [Header("Base Enemy Values:")]
    public float maxHealth = 3f;
    public float despawnTime = 2f;
    public float stunTime = .5f;            //cannot attack while stunned for this time

    public bool facingRight = false;
    public bool canMove = true;
    public bool particleDeath = false;
    public float particleDeathTime = 0.5f;    //how long sprite will be enabled during death

    [Header("Base Movement:")]
    public float moveSpeed = 3;             //default speed it can move
    public float chaseSpeed = 3;            //default speed it chases the player
    public bool detectPlayer = true;        //will attempt to detect player by default
    public float playerDetectRange = 6f;    //default distance it will detectPlayer()
    public float keepDistance = .5f;        //keeps this distance away from the player

    public enum MOVE_TYPE { idle, patrol, wander, chase }; //movement types 
    public MOVE_TYPE movementType = MOVE_TYPE.wander;

    [Header("Patrol Values:")]
    public float patrolBounds_X = 0f;   // how far vertical it can patrol
    public float patrolBounds_Y = 0f;   // how far horizontal it can patrol
    public float patrolDelay = 0f;      //delays between reaching destinations


    // Start is called before the first frame update
    void Start()
    {
        //initialize variables
        startPos = transform.position;
        wanDirection = RandomDirection(moveSpeed);
        currentType = movementType;
        currentHealth = maxHealth;

        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        particles = GetComponent<ParticleSystem>();
        player = GameObject.FindGameObjectWithTag("Player");

        //AudioSources, check it exists then assign it
        if (transform.Find("AudioSources").Find("HurtSound") != null)
            hurtSound = transform.Find("AudioSources").Find("HurtSound").GetComponent<AudioSource>();
        if (transform.Find("AudioSources").Find("DeathSound") != null)
            deathSound = transform.Find("AudioSources").Find("DeathSound").GetComponent<AudioSource>();
        if (transform.Find("AudioSources").Find("AttackSound") != null)
            attackSound = transform.Find("AudioSources").Find("AttackSound").GetComponent<AudioSource>();
        if (transform.Find("AudioSources").Find("IdleSound") != null)
            idleSound = transform.Find("AudioSources").Find("IdleSound").GetComponent<AudioSource>();

        //call SetupEnemy()
        SetupEnemy();
    }

    public virtual void SetupEnemy()
    {
        //this method is overridden by different enemy types
        return;
    }


    //PROTECTED METHODS
    protected void Update()
    {
        if (isAlive)
        {
            if (canMove)
            {
                UpdateFacing();
                UpdateMovement();
            }
            UpdateAttack();
        }
    }

    protected void UpdateFacing()
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

    protected void UpdateMovement()
    {
        //if detecting the player, chase them
        if (PlayerDetected())
        {
            currentType = MOVE_TYPE.chase;
        }
        else
        {
            currentType = movementType;
        }

        //then update movement based on currentType's MOVE_TYPE
        if (currentType == MOVE_TYPE.wander)
            Wander();
        else if (currentType == MOVE_TYPE.patrol)
            Patrol();
        else if (currentType == MOVE_TYPE.chase)
            Chase();
        else if (currentType == MOVE_TYPE.idle)
            Idle();
    }

    protected virtual void UpdateAttack()
    {
        //this method is overridden by other enemy types
        return;
    }

    protected void Wander()    //applies newDirection to the enemy's movement
    {
        body.AddForce(wanDirection);
        //this changes on collision only, methods below
    }

    protected void Patrol()
    {
        if (!isPatrolling)
        {
            //patrol path is based on two points, which use patrolBounds to create
            Vector2 patrolPos1 = new Vector2(startPos.x - patrolBounds_X, startPos.y - patrolBounds_Y);
            Vector2 patrolPos2 = new Vector2(startPos.x + patrolBounds_X, startPos.y + patrolBounds_Y);
            StartCoroutine(PatrolBetween(patrolPos1, patrolPos2, patrolDelay));
        }
    }

    protected void Idle()
    {
        //uses keepDistance for now as an area around startPos
        if (Vector2.Distance(transform.position, startPos) > keepDistance && !movingTowardsTarget)
            StartCoroutine(MoveTowards(startPos, moveSpeed));
    }

    protected void Chase()    //uses keepDistance
    {
        //determine seperation distance between this and the player
        float seperation = Vector2.Distance(body.transform.position, player.transform.position);

        //check if within keepDistance
        if (seperation <= keepDistance)
        {
            //move along side the player's position on y only while within keepDistance
            if (player.transform.position.y > body.position.y)
            {
                body.AddForce(new Vector2(0, chaseSpeed));
            }
            else if (player.transform.position.y < body.position.y)
            {
                body.AddForce(new Vector2(0, -chaseSpeed));
            }
        }
        else
        {
            //move towards the player's positon on x and y using AddForce()
            if (player.transform.position.x > body.position.x)
            {
                body.AddForce(new Vector2(chaseSpeed, 0));
            }
            else
            {
                body.AddForce(new Vector2(-chaseSpeed, 0));
            }

            if (player.transform.position.y > body.position.y)
            {
                body.AddForce(new Vector2(0, chaseSpeed));
            }
            else
            {
                body.AddForce(new Vector2(0, -chaseSpeed));
            }
        }
    }

    protected void FlipFacing()
    {
        facingRight = !facingRight;
        //sprite.flipX = !sprite.flipX;
        gameObject.transform.Rotate(0f, 180f, 0f);
    }

    protected void SpawnLoot()
    {
        // 2D6 Outcome Chances:
        // 2: 2.78%
        // 3: 5.56%
        // 4: 8.33%
        // 5: 11.11&
        // 6: 13.89%
        // 7: 16.67%
        // 8: 13.89%
        // 9: 11.11%
        // 10: 8.33%
        // 11: 5.56%
        // 12: 2.78%

        GameObject item = null;
        // Loot Chances #1
        /*
        int loot = Random.Range(1, 7) + Random.Range(1, 7);
        if (loot >= 3 && loot <= 6)
            item = commonDrop1;
        if (loot >= 8 && loot <= 11)
            item = commonDrop2;
        if (loot == 2 || loot == 12)
            item = rareDrop;
        */

        // Loot Chances #2
        int loot = Random.Range(1, 6) + Random.Range(1, 6);
        // 27.78%
        if (loot == 6 || loot == 8)
            item = commonDrop1;
        // 22.22%
        if (loot == 5 || loot == 9)
            item = commonDrop2;
        // 16.67%
        if (loot == 7)
            item = rareDrop;
        // 33.33% chance nothing drops

        //make sure item is not null, instantiate it at this enemy's position
        if (item != null)
            Instantiate(item, transform.position, transform.rotation);
    }

    protected Vector2 RandomDirection(float speed)
    {
        float x = 0;
        float y = 0;
        //makes sure the random value is at least half of the movespeed in x
        while (x > -(speed) / 2f && x < (speed) / 2f)
        {
            x = Random.Range(-speed, speed);
        }
        //y direction doesn't matter as much, can be any value between negative and positive moveSpeed
        y = Random.Range(-speed, speed);

        return new Vector2(x, y);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentType == MOVE_TYPE.wander)
        {
            wanDirection = RandomDirection(moveSpeed);
        }

        if (currentType == MOVE_TYPE.patrol)    //attempt to return to startPos
        {
            StartCoroutine(ResetPosition());
        }

    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (currentType == MOVE_TYPE.wander)
        {
            wanDirection = RandomDirection(moveSpeed);
        }

        if (currentType == MOVE_TYPE.patrol)    //attempt to return to startPos
        {
            StartCoroutine(ResetPosition());
        }
    }

    //PUBLIC METHODS
    public bool PlayerDetected()
    {
        if (!detectPlayer)  //return false if detectPlayer is false
            return false;

        //returns false if player does not exist
        if (player == null || !player.activeSelf)
        {
            return false;
        }

        float seperation = Vector2.Distance(body.transform.position, player.transform.position);
        return seperation <= playerDetectRange;
    }

    public bool Alive()
    {
        return isAlive;
    }

    public IEnumerator MoveTowards(Vector2 target, float speed)
    {
        movingTowardsTarget = true;
        MOVE_TYPE startingType = currentType;

        while (Vector2.Distance(transform.position, target) > .5f && isAlive)
        {
            if (currentType != startingType)    //break loop if movementType changes
            {
                movingTowardsTarget = false;
                yield break;
            }


            //move towards target position using AddForce()
            if (target.x > body.position.x)
            {
                body.AddForce(new Vector2(speed, 0));
            }
            else
            {
                body.AddForce(new Vector2(-speed, 0));
            }

            if (target.y > body.position.y)
            {
                body.AddForce(new Vector2(0, speed));
            }
            else
            {
                body.AddForce(new Vector2(0, -speed));
            }

            yield return null;
        }

        movingTowardsTarget = false;
    }

    public IEnumerator PatrolBetween(Vector2 first, Vector2 second, float waitTime)
    {
        isPatrolling = true;
        Vector2 target = first; //moves towards this target first

        while (currentType == MOVE_TYPE.patrol && isAlive) //break the loop if no longer patrolling
        {
            if (!movingTowardsTarget)
            {
                StartCoroutine(MoveTowards(target, moveSpeed));
            }

            //check if close enough to switch target
            if (Vector2.Distance(transform.position, target) < 0.5f)
            {
                if (target != second)
                    target = second;
                else
                    target = first;

                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }

        //end
        isPatrolling = false;
    }

    public IEnumerator ResetPosition()
    {
        //store previousType to return to after, set movement to idle to return to startPos
        MOVE_TYPE previousType = movementType;
        movementType = MOVE_TYPE.idle;
        currentType = MOVE_TYPE.idle;

        while (Vector2.Distance(transform.position, startPos) > .5f && isAlive)
        {
            yield return null;
        }
        //once we have arrived at startPos, return to previousType
        movementType = previousType;
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

            //die if at 0 health
            if (currentHealth <= 0)
            {
                StartCoroutine(Die());
            }

            if (isAlive)
                yield return new WaitForSeconds(stunTime);

            stunned = false;
        }
    }

    public IEnumerator Die()
    {
        isAlive = false;

        //die animation and sound
        animator.SetBool("isDead", true);
        if (idleSound != null)
            idleSound.Stop();

        if (deathSound != null)  //make sure there's something to play
        { 
            deathSound.Play();
        }

        //disable Components
        GetComponent<Collider2D>().enabled = false;

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
        SpawnLoot();
        Destroy(gameObject);
    }

    
}
