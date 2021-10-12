using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Player Stats
    public float health = 10f;
    public float mana = 5f;
    
    public float invulRate = 2f;
    float invulTime = 0f;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time > invulTime)
        {
            if (collision.gameObject.tag == "Enemy")
            {

                health = health - 1;
                if (health <= 0)
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
   
}
