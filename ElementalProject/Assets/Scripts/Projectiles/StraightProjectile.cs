using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightProjectile : MonoBehaviour
{
    //components
    private Rigidbody2D body;
    private Collider2D projCollider;
    private Animator animator;
    private Transform cam;

    //public fields
    public float damage = 1f;
    public float projSpeed = 3f;
    public float explodeRadius = 1f;
    public float explodeDamage = 0.5f;
    public bool explodeOnHit = true;
    public bool hasHitAnim = true;
    public bool isPlayerProj = false;   //set this to true if it should not damage player

    //private fields
    private LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        projCollider = GetComponent<Collider2D>();
        cam = GameObject.Find("Main Camera").GetComponent<Transform>();

        //get the projectile moving
        body.velocity = transform.right * projSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, cam.position) > 50f)
        {
            //if the projectile is far away from the main camera, it is destroyed
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerProj)
        {
            //call TakeDamage on enemies and Hit
            if (other.gameObject.tag == "Enemy")
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

    IEnumerator Hit()
    {
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

    void Explode()
    {
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
