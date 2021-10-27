using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(enemy.damage_medium);
        }
    }
}
