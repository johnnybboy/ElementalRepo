using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Player Stats
    public float health = 5f;
    public int maxHearts = 5;
    public int mana = 3;
    public int maxMana = 3;
    public int coins = 00;

    public float damage_1 = .5f;
    public float damage_2 = 1f;
    public float damage_3 = 1.5f;

    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public float attackRate = 2f;
    float nextAttackTime = 0f;

    public float invulRate = 1f;
    float invulTime = 0f;

    void Update()
    {
        //update with attackRate and call Attack()
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }

        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //damage collisions, check if invulnerable
        if (Time.time > invulTime)
        {
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Projectile")
            {

                TakeDamage(damage_1);
                
            }

            // trap damage should be handled by the trap itself now.
            //if (collision.gameObject.tag == "Trap")
            //{
            //    TakeDamage(damage_2);
            //}
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

    void Die()
    {
        gameObject.SetActive(false);
    }

    void PickupHealth(float amount)
    {
        HealthBar.instance.AddHearts(amount);
        health = HealthBar.instance.currentHearts;
        //animator.SetTrigger("Heal");      //heal animation?

        invulTime = Time.time + 1f / invulRate;     //temporary invul when pickup heart

    }

    public void TakeDamage(float amount)
    {
        HealthBar.instance.RemoveHearts(amount);
        health = HealthBar.instance.currentHearts;
        animator.SetTrigger("Hurt");

        if (health <= 0)
            Die();
        else
            invulTime = Time.time + 1f / invulRate;
    }

    void Attack()
    {
        // Play an attack animation
        animator.SetTrigger("Attack");

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
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

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
