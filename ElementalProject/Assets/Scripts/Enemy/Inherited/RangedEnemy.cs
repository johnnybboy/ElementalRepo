using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : BaseEnemy
{
    private Transform firePoint;

    [Header("Ranged Enemy Values:")]
    public GameObject projectilePrefab;
    public float damageDealt = .5f;
    public float rangedAttackRange = 8f;
    public float rangedAttackAnimDelay = .3f;
    public float rangedAttackInterval = 2f;

    // Start is called before the first frame update
    public override void SetupEnemy()
    {
        //Get FirePoint
        firePoint = transform.Find("FirePoint");

        if (firePoint == null)
        {
            Debug.LogError("No FirePoint found for " + name + "!");
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("No projectilePrefab found for " + name + "!");
        }
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

            StartCoroutine(RangedAttack());
        }
    }
    public IEnumerator RangedAttack()
    {
        if (canAttack && !stunned)
        {
            canAttack = false;
            float seperation = Vector2.Distance(body.transform.position, player.transform.position);
            if (seperation <= rangedAttackRange)
            {
                //play animation, wait for animation to finish
                animator.SetTrigger("attack");
                yield return new WaitForSeconds(rangedAttackAnimDelay);

                //fire projectile, wait for attack delay, after allow attacking again
                Instantiate(projectilePrefab, firePoint.position, transform.rotation);
                yield return new WaitForSeconds(rangedAttackInterval);
            }
            canAttack = true;
        }
    }
}
