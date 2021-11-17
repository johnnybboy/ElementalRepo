using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapCollision : MonoBehaviour
{
    public float damage = 1f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            enemy.TakeDamage(damage);
        }

        if (collision.gameObject.tag == "Player")
        {
            PlayerController1 player = collision.gameObject.GetComponent<PlayerController1>();
            StartCoroutine(player.TakeDamage(damage));
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            enemy.TakeDamage(damage);
        }

        if (collision.gameObject.tag == "Player")
        {
            PlayerController1 player = collision.gameObject.GetComponent<PlayerController1>();
            StartCoroutine(player.TakeDamage(damage));
        }
    }
}
