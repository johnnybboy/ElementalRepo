using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyHomingProjectile : MonoBehaviour
{
    //components
    private Rigidbody2D body;
    private Collider2D projCollider;
    private Animator animator;
    private GameObject target;

    //public fields
    public float damage = 1f;
    public float projSpeed = 3f;
    public bool explodeOnHit = true;
    public float explodeRadius = 1f;
    public float explodeDamage = 0.5f;
    public float despawnTime = 5f;
    public bool hasHitAnim = true;
    public bool isPlayerProj = false;   //set this to true if it should not damage player
    public bool isTargettingPlayer = false;

    //private fields
    private LayerMask layerMask;
    private bool isHit = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        projCollider = GetComponent<Collider2D>();
        if (isTargettingPlayer)
            target = GameObject.FindGameObjectWithTag("Player");

        //start the TimedDeath coroutine
        StartCoroutine(TimedDeath());

        //move towards target
        StartCoroutine(HomeTowards(target.transform.position, projSpeed));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerProj)
        {
            //call TakeDamage on enemies and Hit
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
            //call TakeDamage on player
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.SendMessage("TakeDamage", damage);
                StartCoroutine(Hit());
            }
            //call Hit if it is blocked by something
            else
            {
                StartCoroutine(Hit());
            }
        }


    }

    IEnumerator HomeTowards(Vector2 target, float speed)
    {
        while (Vector2.Distance(transform.position, target) > 0.2f)
        {
            //move towards target position using AddForce()
            if (target.x > body.position.x)
            {
                body.AddForce(new Vector2(speed, 0));
            }
            else
            {
                body.AddForce(new Vector2(-speed, 0));
            }

            if (target.y > body.position.y)
            {
                body.AddForce(new Vector2(0, speed));
            }
            else
            {
                body.AddForce(new Vector2(0, -speed));
            }

            yield return null;
        }

        StartCoroutine(Hit());
    }

    IEnumerator Hit()
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

    IEnumerator TimedDeath()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }

    void Explode()
    {
        //scale it up for explosion
        transform.localScale *= 2f;

        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, explodeRadius);
        foreach (Collider2D target in targets)
        {
            //make sure it doesn't affect itself
            if (target == this.gameObject.GetComponent<Collider2D>())
            {
                return;
            }

            //call TakeDamage on all enemies within explodeRadius
            if (isPlayerProj && target.tag == "Enemy")
                target.gameObject.SendMessage("TakeDamage", explodeDamage);

            //call TakeDamage on all enemies within explodeRadius
            else if (!isPlayerProj && target.tag == "Player")
                target.gameObject.SendMessage("TakeDamage", explodeDamage);

            //call TakeDamage on all enemies within explodeRadius
            else if (target.tag == "Projectile")
                target.gameObject.SendMessage("Hit");
        }
    }
}
