using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //public Components
    public AudioClip hurtSound, swingSound;

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
    public bool isAttacking = false;

    public float hurtTime = 0.75f;    //invulnerable per second
    public bool isInvulnerable = false;


    //private Components
    private Transform player;
    private Animator animator;
    private AudioSource sound;
    private GameObject rightSwordHitBox;
    private GameObject leftSwordHitBox;

    private void Start()
    {
        sound = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        rightSwordHitBox = this.gameObject.transform.GetChild(0).gameObject;
        leftSwordHitBox = this.gameObject.transform.GetChild(1).gameObject;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isAttacking)
            {
                StartCoroutine(SwordAttack());
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //damage collisions, check if invulnerable
        if (!isInvulnerable)
        {
            //collisions with enemies or projectiles deal damage_weak to player
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Projectile")
            {

                TakeDamage(damage_weak);
                
            }
        }

        //item collisions
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

    public void OnCollisionStay2D(Collision2D collision)
    {
        //if something is continuing to touch, check if vulnerable and apply damage
        if (!isInvulnerable)
        {
            //collisions with enemies or projectiles deal damage_weak to player
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Projectile")
            {

                TakeDamage(damage_weak);

            }
        }
    }

    void PickupHealth(float amount)
    {
        HealthBar.instance.AddHearts(amount);
        health = HealthBar.instance.currentHearts;
        //animator.SetTrigger("Heal");      //heal animation?

        StartCoroutine(Invulnerable(hurtTime));     //temporary invul when pickup heart

    }

    public void TakeDamage(float amount)
    {
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
            sound.clip = hurtSound;
            sound.Play();
        }

        if (health <= 0)
            Die();
        else
            if (!isInvulnerable)
                StartCoroutine(Invulnerable(hurtTime));    //make invulnerable for hurtTime
    }

    void Die()
    {
        gameObject.SetActive(false);
    }

    IEnumerator SwordAttack()
    {
        isAttacking = true;

        //play animation and sound
        animator.SetTrigger("Attack");
        if (swingSound != null)  //make sure there's something to play
        {
            sound.clip = swingSound;
            sound.Play();
        }

        //depending on facing, activate the sword hitbox
        if (GetComponent<PlayerMovement>().facingRight)
        {
            rightSwordHitBox.SetActive(true);
        }
        else
        {
            leftSwordHitBox.SetActive(true);
        }

        yield return new WaitForSeconds(attackRate);

        rightSwordHitBox.SetActive(false);
        leftSwordHitBox.SetActive(false);
        isAttacking = false;
    }

    IEnumerator Invulnerable(float time)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(time);
        isInvulnerable = false;
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

}
