using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            enemy.TakeDamage(enemy.damage_medium);
        }

        if (collision.gameObject.tag == "Player")
        {
            PlayerController1 player = collision.gameObject.GetComponent<PlayerController1>();
            StartCoroutine(player.TakeDamage(player.damage_medium));
        }
    }
}
