using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            enemy.TakeDamage(enemy.damage_medium);
            KnockBack(enemy.gameObject.GetComponent<Rigidbody2D>(), enemy.damage_medium);
        }
        if (other.tag == "Boss")
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            enemy.TakeDamage(enemy.damage_medium);
        }
    }

    void KnockBack(Rigidbody2D enemy, float distance)  //based on damage for now
    {
        Vector2 knockBackForce;
        if (enemy.gameObject.transform.position.x >= transform.position.x)
        {
            knockBackForce = new Vector2(distance, enemy.transform.position.y);
            
        }
        else
        {
            knockBackForce = new Vector2(-distance, enemy.transform.position.y);
        }

        enemy.AddForce(knockBackForce, ForceMode2D.Impulse);
    }
}
