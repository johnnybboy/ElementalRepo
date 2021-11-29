using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController1 : MonoBehaviour
{
    // Player Stats
    public float health = 5f;
    public int maxHearts = 5;
    public float mana = 3f;
    public int maxMana = 3;
    public int coins = 00;

    // Damage values
    public float damage_weak = .5f;
    public float damage_medium = 1f;
    public float damage_strong = 1.5f;

    //Player modifiers
    public float attackRange = 0.5f;
    public float comboInterval = .33f;  //time between each hit of 3-hit swing combo

    public float attackRate = 0.66f;   //attacks per second
    public float magicRate = 1f;    //magic per second
    public float spellCost = 0.5f;

    public float hurtTime = 0.75f;    //invulnerable per second

    public float dodgeRate = .5f;      //dodging per second
    public float dodgeCooldown = .5f;   //wait between dodges

    public float despawnTime = 3f;

    //player conditions
    public bool isInvulnerable = false;   //public to allow for invulnerablity if needed
    public bool isBusy = false;    //catchall for character actions
    public bool isAlive = true;
    public bool isDodging = false;

    private bool canSwordAttack = true;
    private bool canRoll = true;
    private bool canMagicAttack = true;
    private bool canHeavyAttack = true;
    private bool canTakeDamage = true;

    //public Components
    public GameObject magicProjectile; //should be different for each Player_color

    //private Components
    private Animator animator;
    private SpriteRenderer sprite;
    private Collider2D hitBox;
    private AudioSource hurtSound, swingSound, dodgeSound;
    private GameObject SwordHitBox;
    private Transform firePoint;
    private GameObject spell;
    private PlayerMovement movement;

    private void Start()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        hitBox = GetComponent<Collider2D>();
        movement = GetComponent<PlayerMovement>();

        //Colliders are GetChild(0)
        SwordHitBox = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;

        //AudioSources are GetChild(1)
        hurtSound = this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<AudioSource>();
        swingSound = this.gameObject.transform.GetChild(1).GetChild(1).GetComponent<AudioSource>();
        dodgeSound = this.gameObject.transform.GetChild(1).GetChild(2).GetComponent<AudioSource>();

        //FirePoint is GetChild(2)
        firePoint = this.gameObject.transform.GetChild(2).gameObject.transform;

        //Spell is GetChild(3)
        spell = this.gameObject.transform.GetChild(3).gameObject;
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

            if (Input.GetKey(KeyCode.L))
            {
                if (!isBusy && canHeavyAttack)
                {
                    StartCoroutine(HeavyAttack());
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

    void PickupMana(float amount)
    {
        ManaBar.instance.AddMana(amount);
        mana = ManaBar.instance.currentMana;
        //animator.SetTrigger("ManaUp");       //ManaUp animation?
    }

    void PickupCoins(int num)
    {
        coins += num;
        //coin animation?
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
            canSwordAttack = false;

            //start attack for combo1, wait for comboInterval
            StartCoroutine(ComboAttack("Attack", SwordHitBox)); //combo1
            yield return new WaitForSeconds(comboInterval);
            
            //check for input to continue to combo2, otherwise end attack
            if (Input.GetKey(KeyCode.J))
            {
                //show the combo is working, start attack for combo2
                sprite.color = new Color(1f, .5f, .5f);
                StartCoroutine(ComboAttack("Attack", SwordHitBox)); //combo2
                yield return null;
                sprite.color = new Color(1f, 1f, 1f);
                yield return new WaitForSeconds(comboInterval);

                //check for input to continue to combo3, otherwise end attack
                if (Input.GetKey(KeyCode.J)) 
                {
                    //show the combo is working, start attack for combo3
                    sprite.color = new Color(1f, .5f, .5f);
                    StartCoroutine(ComboAttack("Attack", SwordHitBox)); //combo3
                    yield return null;
                    sprite.color = new Color(1f, 1f, 1f);
                    yield return new WaitForSeconds(comboInterval);
                }
            }

            //end attack and wait for attackRate to allow attacking again
            SwordHitBox.SetActive(false);
            isBusy = false;
            yield return new WaitForSeconds(attackRate);

            canSwordAttack = true;
        }
    }

    IEnumerator ComboAttack(string comboAnim, GameObject hitBox)
    {
        //play combo animation and sound
        animator.SetTrigger(comboAnim);
        if (swingSound != null)  //make sure there's something to play
        {
            swingSound.Play();
        }

        //set hitBox active, wait for comboInterval, then set it inActive
        hitBox.SetActive(true);
        yield return new WaitForSeconds(comboInterval);
        hitBox.SetActive(false);
    }


    IEnumerator MagicAttack()
    {
        if (canMagicAttack && !isBusy)
        {
            isBusy = true;
            canMagicAttack = false;

            //first try to remove mana
            if (ManaBar.instance.currentMana == 0f)
            {
                isBusy = false;
                canMagicAttack = true;
                Debug.Log("No mana!");
                yield break;
            }
                
            //remove mana from the ManaBar
            if (ManaBar.instance != null)
            {
                ManaBar.instance.RemoveMana(spellCost);
                mana = ManaBar.instance.currentMana;
            }
            else
            {
                //otherwise just keep track privately
                mana -= spellCost;
                if (mana < 0)
                {
                    mana = 0f;
                }
            }

            //cast this player's spell if it exists
            if (spell != null)
            {
                animator.SetTrigger("Magic");
                spell.SendMessage("CastSpell", firePoint);
            }
            else
            {
                Debug.LogError("Spell failed to cast!");
            }
            
            yield return new WaitForSeconds(magicRate);
            isBusy = false;
            canMagicAttack = true;
        }
    }

    IEnumerator HeavyAttack()
    {
        if (canHeavyAttack)
        {
            isBusy = true;
            canHeavyAttack = false;

            //play combo animation and sound
            animator.SetTrigger("Attack");  //HeavyAttack TODO
            if (swingSound != null)  //make sure there's something to play
            {
                swingSound.Play();  //heavySwingSound TODO
            }

            //set heavyHitBox active, wait for comboInterval, then set it inActive
            SwordHitBox.SetActive(true);
            sprite.color = new Color(2f, 0.75f, 0.75f, 1f);
            yield return new WaitForSeconds(comboInterval);
            SwordHitBox.SetActive(false);
            sprite.color = new Color(1f, 1f, 1f, 1f);

            //wait for attackRate to allow attacking again
            yield return new WaitForSeconds(attackRate);
            isBusy = false;
            canHeavyAttack = true;
        }
    }

    IEnumerator DodgeRoll()
    {
        if (canRoll && !isBusy)
        {
            isDodging = true;
            isBusy = true;
            canRoll = false;

            //play animation and sound
            animator.SetTrigger("Dodge");
            if (dodgeSound != null)  //make sure there's something to play
            {
                dodgeSound.Play();
            }

            //start dodging
            sprite.color = new Color(1f, 1f, 1f, .5f);  //make transparent to signify dodging
            hitBox.enabled = false; //turn off collision with enemys and traps
            movement.moveSpeed *= 2;

            yield return new WaitForSeconds(dodgeRate); //dodge for dodgeRate

            //end dodging
            movement.moveSpeed /= 2;
            isDodging = false;
            isBusy = false;

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
                PickupMana(1f);
            if (tag == "Coin")
                PickupCoins(1);

            //remove the item
            Destroy(collision.gameObject);
        }
    }

}
