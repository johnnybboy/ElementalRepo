using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightProjectile : BaseProjectile
{
    // Start is called before the first frame update
    public override void SetupProjectile()
    {
        //decide x direction
        float xSpeed = projSpeed;
        if (sourceFacingLeft)  //will reverse the x value for movement
            xSpeed *= -1f;

        //get the projectile moving
        body.velocity = (transform.right * xSpeed);
    }
}
