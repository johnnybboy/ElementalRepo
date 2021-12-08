using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowardsTargetProj : BaseProjectile
{
    //private components
    private GameObject target;

    [Header("TowardsTarget Projectile Values:")]
    public bool isTargettingPlayer = true;

    public override void SetupProjectile()
    {
        //determine target
        if (isTargettingPlayer)
            target = GameObject.FindGameObjectWithTag("Player");

        //TODO
        //What if target is not player?

        //head towards target
        if (target != null)
            StartCoroutine(HeadTowards(target.transform.position));
    }

    IEnumerator HeadTowards(Vector2 target)
    {
        //determine direction based on where target is, move towards it
        Vector2 direction = (target - body.position);
        body.velocity = direction.normalized * projSpeed;

        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            yield return null;
        }

        //Hit() once it has reached the target
        StartCoroutine(Hit());
    }
}
