using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAttackCollision : MonoBehaviour
{
    public float damage = 1f;
    public float pushForce = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.SendMessage("TakeDamage", damage);
            KnockBack(other.gameObject, pushForce); //knocks target away from this
        }
    }

    void KnockBack(GameObject target, float force)
    {
        Vector2 knockBackForce;
        if (target.transform.position.x >= transform.position.x)
        {
            knockBackForce = new Vector2(force, 0);
            
        }
        else
        {
            knockBackForce = new Vector2(-force, 0);
        }

        target.GetComponent<Rigidbody2D>().AddForce(knockBackForce, ForceMode2D.Impulse);
    }
}
