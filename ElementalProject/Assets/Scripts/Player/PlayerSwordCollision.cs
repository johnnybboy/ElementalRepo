using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordCollision : MonoBehaviour
{
    public float damage = 1f;
    public float knockBackDist = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            other.gameObject.SendMessage("TakeDamage", damage);
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            KnockBack(enemy.gameObject.GetComponent<Rigidbody2D>(), knockBackDist);
        }
        if (other.tag == "Boss")
        {
            other.gameObject.SendMessage("TakeDamage", damage);
        }
        //if (other.tag == "Projectile")
        //    Destroy(other.gameObject);
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
