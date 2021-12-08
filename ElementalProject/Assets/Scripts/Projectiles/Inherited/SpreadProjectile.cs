using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadProjectile : BaseProjectile
{
    [Header("Spread Projectile Values:")]
    public float spreadRange = 1.5f;

    // Start is called before the first frame update
    public override void SetupProjectile()
    {
        //add a random y-value to the projectile, decide x direction
        float spreadValue = Random.Range(-spreadRange, spreadRange);
        float xSpeed = projSpeed;
        if (sourceFacingLeft)  //will reverse the x value for movement
            xSpeed *= -1f;

        //get the projectile moving
        body.velocity = new Vector3(0, spreadValue, 0) + (transform.right * xSpeed);
    }
}
