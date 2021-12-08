using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    //private components
    protected Rigidbody2D body;
    protected Collider2D projCollider;
    protected Animator animator;

    //public fields
    [Header("Base Projectile Values:")]
    public bool hasHitAnim = true;      //set to false if there is no hit animation
    public bool explodeOnHit = true;    //set to true if there should be splash damage
    public bool sourceFacingLeft = false;      //only if source is not facingRight by default

    public float damage = 1f;
    public float projSpeed = 3f;
    public float despawnTime = 5f;

    public float explodeDamage = 0.5f;
    public float explodeRadius = 1f;

    //private fields
    protected bool isPlayerProjectile = false;
    protected bool isHit = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        projCollider = GetComponent<Collider2D>();

        //determine if this projectile is a PlayerProjectile or not
        if (gameObject.tag == "PlayerProjectile")
            isPlayerProjectile = true;
        else
            isPlayerProjectile = false;

        //start TimedDeath()
        StartCoroutine(TimedDeath());

        //setup the movement or other needed values for this projectile
        SetupProjectile();

        //if sourceFacingLeft, flip the projectile
        if (sourceFacingLeft)
        {
            transform.Rotate(0f, 180f, 0f);
        }
    }

    public virtual void SetupProjectile()
    {
        //this will be overridden by another projectile script
        return;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerProjectile)
        {
            //call TakeDamage on enemies and then Hit()
            if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Boss")
            {
                other.gameObject.SendMessage("TakeDamage", damage);
                StartCoroutine(Hit());
            }
            //call Hit on enemy projectiles
            if (other.gameObject.layer.Equals("EnemyProjectiles"))
            {
                other.gameObject.SendMessage("Hit");
            }
        }

        else
        {
            //call TakeDamage on player then Hit()
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.SendMessage("TakeDamage", damage);
                StartCoroutine(Hit());
            }
            //Hit() if it is blocked by something else
            else
            {
                StartCoroutine(Hit());
            }
        }
    }

    public IEnumerator Hit()
    {
        if (isHit)
            yield break;

        isHit = true;

        //call Explode() if explodeOnHit
        if (explodeOnHit)
            Explode();

        //stop motion, disable collision
        body.velocity = new Vector2(0, 0);
        projCollider.enabled = false;

        if (hasHitAnim)
        {
            //play animation
            animator.SetTrigger("hit");
            yield return new WaitForSeconds(0.5f);
        }

        Destroy(this.gameObject);
    }

    public IEnumerator TimedDeath()
    {
        yield return new WaitForSeconds(despawnTime);
        StartCoroutine(Hit());
    }

    public void Explode()
    {
        //scale it up for explosion
        if (hasHitAnim)
            transform.localScale *= 2f;

        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, explodeRadius);
        foreach (Collider2D target in targets)
        {
            //make sure it doesn't affect itself
            if (target == this.gameObject.GetComponent<Collider2D>())
            {
                return;
            }

            //call TakeDamage or Hit depending on if this isPlayerProjectile or not
            if (isPlayerProjectile)
            {
                if (target.tag == "Enemy")
                    target.gameObject.SendMessage("TakeDamage", explodeDamage);
                if (target.tag == "EnemyProjectile")
                    target.gameObject.SendMessage("Hit");
            }
            else
            {
                if (target.tag == "Player")
                    target.gameObject.SendMessage("TakeDamage", explodeDamage);
            }
        }
    }
}
