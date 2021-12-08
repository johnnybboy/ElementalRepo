using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    [Header("Melee Enemy Values:")]
    public float damageDealt = .5f;
    public float meleeAttackRange = 1f;
    public float meleeAttackDelay = 2f;

    // Start is called before the first frame update
    public override void SetupEnemy()
    {
        //no override here!
        return;
    }

    protected override void UpdateAttack()
    {
        if (PlayerDetected()) // If it detects the player
        {
            if (player.transform.position.x < body.position.x) // looks left if player is left
            {
                if (facingRight)
                {
                    FlipFacing();
                }
            }
            else if (player.transform.position.x > body.position.x) //looks right if player is right
            {
                if (facingRight != true)
                {
                    FlipFacing();
                }

            }

            StartCoroutine(MeleeAttack());
        }
    }
    IEnumerator MeleeAttack()
    {
        if (canAttack && !stunned)
        {
            canAttack = false;
            float seperation = Vector2.Distance(body.transform.position, player.transform.position);
            if (seperation <= meleeAttackRange)
            {
                //attack if within range, call player's TakeDamage() method.
                animator.SetTrigger("attack");
                player.SendMessage("TakeDamage", damageDealt);

                //wait until attack delay ends, after allow attacking again
                yield return new WaitForSeconds(meleeAttackDelay);
            }
            canAttack = true;
        }
    }
}
