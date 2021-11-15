using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController1 : MonoBehaviour
{
    // Player Stats
    public float health = 5f;
    public int maxHearts = 5;
    public int mana = 3;
    public int maxMana = 3;
    public int coins = 00;

    // Damage values
    public float damage_weak = .5f;
    public float damage_medium = 1f;
    public float damage_strong = 1.5f;

    //Player modifiers
    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public float attackRate = 0.5f;   //attacks per second
    public float magicRate = 1f;    //magic per second

    public float hurtTime = 0.75f;    //invulnerable per second

    public float dodgeRate = 1f;      //dodging per second
    public float dodgeCooldown = .5f;   //wait between dodges

    public float despawnTime = 3f;

    //player conditions
    public bool isInvulnerable = false;   //public to allow for invulnerablity if needed
    public bool isBusy = false;    //catchall for character actions
    public bool isAlive = true;
    public bool isAttacking = false;
    private bool isDodging = false;

    private bool canSwordAttack = true;
    private bool canRoll = true;
    private bool canMagicAttack = true;
    private bool canTakeDamage = true;

    //public Components
    public GameObject magicProjectile; //should be different for each Player_color

    //private Components
    private Animator animator;
    private SpriteRenderer sprite;
    private Collider2D hitBox;
    private AudioSource hurtSound, swingSound, magicSound;
    private GameObject SwordHitBox;
    private Transform firePoint;

    private void Start()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        hitBox = GetComponent<Collider2D>();

        //Colliders are GetChild(0)
        SwordHitBox = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;

        //AudioSources are GetChild(1)
        hurtSound = this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<AudioSource>();
        swingSound = this.gameObject.transform.GetChild(1).GetChild(1).GetComponent<AudioSource>();

        //FirePoint is GetChild(2)
        firePoint = this.gameObject.transform.GetChild(2).gameObject.transform;
    }

    void Update()
    {
        if (isInvulnerable)
            canTakeDamage = false;

        if (isAlive)
        {
            if (Input.GetKey(KeyCode.J))
            {
                if (!isBusy && canSwordAttack)
                {
                    StartCoroutine(SwordAttack());
                }
            }

            if (Input.GetKey(KeyCode.K))
            {
                if (!isBusy && canMagicAttack)
                {
                    StartCoroutine(MagicAttack());
                }
            }

            if (Input.GetKey(KeyCode.Space))
            {
                if (!isBusy && canRoll)
                {
                    StartCoroutine(DodgeRoll());
                }
            }
        }
    }

    

    void PickupHealth(float amount)
    {
        HealthBar.instance.AddHearts(amount);
        health = HealthBar.instance.currentHearts;
        //animator.SetTrigger("Heal");      //heal animation?
        //TODO

        //temporary invul when pickup heart?

    }

    public IEnumerator TakeDamage(float amount)
    {
        if (canTakeDamage && !isDodging)  //check if canTakeDamage
        {
            canTakeDamage = false;

            //adjust health based on the HealthBar if it exists
            if (HealthBar.instance != null)
            {
                HealthBar.instance.RemoveHearts(amount);
                health = HealthBar.instance.currentHearts;
            }
            else
            {
                //otherwise just keep track privately
                health -= amount;
            }

            //play animation and sound
            animator.SetTrigger("Hurt");
            if (hurtSound != null)  //make sure there's something to play
            {
                hurtSound.Play();
            }

            if (health <= 0)
            {
                StartCoroutine(Die());  //won't set canTakeDamage = true if player Dies!
            }     
            else
            {
                yield return new WaitForSeconds(hurtTime);
                canTakeDamage = true;
            }
        }
    }

    IEnumerator SwordAttack()
    {
        if (canSwordAttack)
        {
            isBusy = true;
            isAttacking = true; //for movement stopping
            canSwordAttack = false;

            //play animation and sound
            animator.SetTrigger("Attack");
            if (swingSound != null)  //make sure there's something to play
            {
                swingSound.Play();
            }

            //start attack
            SwordHitBox.SetActive(true);
            yield return new WaitForSeconds(attackRate);

            //end attack
            SwordHitBox.SetActive(false);

            isBusy = false;
            isAttacking = false;
            canSwordAttack = true;
        }
    }

    IEnumerator MagicAttack()
    {
        if (canMagicAttack && !isBusy)
        {
            isBusy = true;
            isAttacking = true; //for stopping movement
            canMagicAttack = false;

            //check if there's enough mana
            //TODO

            //play animation and sound
            animator.SetTrigger("Magic");
            if (magicSound != null)  //make sure there's something to play
            {
                magicSound.Play();
                 
            }

            //fire magicProjectile if it exists and firePoint is set up properly
            if (firePoint != null && magicProjectile != null)
            {
                Instantiate(magicProjectile, firePoint.position, transform.rotation);
            }
            
            yield return new WaitForSeconds(magicRate);
            isBusy = false;
            isAttacking = false;
            canMagicAttack = true;
        }
    }

    IEnumerator DodgeRoll()
    {
        if (canRoll && !isBusy)
        {
            isBusy = true;
            isDodging = true;
            canRoll = false;

            //play animation and sound
            //animator.SetTrigger("Roll");
            //if (rollSound != null)  //make sure there's something to play
            //{
            //    rollSound.Play();
            //}

            //start dodging
            //make transparent to signify dodging
            sprite.color = new Color(1f, 1f, 1f, .5f);

            //turn off collision with enemys and traps
            hitBox.enabled = false;

            yield return new WaitForSeconds(dodgeRate); //dodge for dodgeRate

            //end dodging
            isBusy = false;
            isDodging = false;

            sprite.color = new Color(1f, 1f, 1f, 1f);   //return to default transparency
            hitBox.enabled = true;

            yield return new WaitForSeconds(dodgeCooldown); //cooldown for dodgeCooldown

            //end cooldown
            canRoll = true;
        }
    }

    IEnumerator Die()   //TODO
    {
        isAlive = false;

        //TODO
        //die animation and sound
        //animator.SetBool("isDead", true);
        //if (deathSound != null)  //make sure there's something to play
        //{
        //    sound.clip = deathSound;
        //    sound.Play();
        //}

        //disable Components
        GetComponent<Collider2D>().enabled = false;
        if (GetComponent<PlayerMovement>() != null)
            GetComponent<PlayerMovement>().enabled = false;
        gameObject.SetActive(false);    //temp just sets inactive in scene

        //save information somewhere?

        //despawn after delay? 
        yield return new WaitForSeconds(despawnTime);


        //play game over screen?



    }

    void AddMana(int num)
    {
        if (mana + num > maxMana)
            mana = maxMana;
        else
            mana += num;
    }

    void AddCoins(int num)
    {
        coins += num;
    }

    //COLLISIONS
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInvulnerable)
        {
            //collisions with enemies or projectiles deal damage_weak to player
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Projectile")
                StartCoroutine(TakeDamage(damage_weak));
            if (collision.gameObject.tag == "Boss")
                StartCoroutine(TakeDamage(damage_medium));
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (!isInvulnerable)
        {
            //collisions with enemies or projectiles deal damage_weak to player
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Projectile")
                StartCoroutine(TakeDamage(damage_weak));
            if (collision.gameObject.tag == "Boss")
                StartCoroutine(TakeDamage(damage_weak));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //item triggers
        if (collision.gameObject.layer == 8)    //Layer 8 is Items
        {
            string tag = collision.gameObject.tag;
            if (tag == "Heart")
                PickupHealth(1f);
            if (tag == "Mana")
                AddMana(1);
            if (tag == "Coin")
                AddCoins(1);

            //remove the item
            Destroy(collision.gameObject);
        }
    }

}
