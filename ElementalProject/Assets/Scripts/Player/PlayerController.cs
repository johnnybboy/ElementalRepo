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
    
    public float invulRate = 2f;
    float invulTime = 0f;

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
   
}
