using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Player Stats
    public int hearts = 10;
    public int maxHearts = 10;
    public int mana = 3;
    public int maxMana = 3;
    public int coins = 00;

    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public float attackRate = 2f;
    float nextAttackTime = 0f;

    public float invulRate = 2f;
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
        if (Time.time > invulTime)
        {
            if (collision.gameObject.tag == "Enemy")
            {

                TakeDamage(1);
                if (hearts <= 0)
                {
                    Die();
                }
                else
                {
                    invulTime = Time.time + 1f / invulRate;
                }
            }
        }

        if (collision.gameObject.layer == 8)    //Layer 8 is Items
        {
            string tag = collision.gameObject.tag;
            if (tag == "Heart")
                AddHearts(1);
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
        Debug.Log("You are dead!");
    }

    void TakeDamage(int damage)
    {
        hearts -= damage;
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
            Debug.Log("We hit " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    void AddHearts(int num)
    {
        if (hearts + num > maxHearts)
            hearts = maxHearts;
        else
            hearts += num;
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
