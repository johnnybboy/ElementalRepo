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
    public bool hasHitAnim = true;
    public bool isPlayerProj = false;   //set this to true if it should damage enemies

    //private fields
    private bool hit = false;


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        projCollider = GetComponent<Collider2D>();
        cam = GameObject.Find("Main Camera").GetComponent<Transform>();

        //give the projectile moving
        body.velocity = transform.right * projSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
            //stop projectile
            body.velocity = new Vector2(0, 0);
        }

        if (Vector2.Distance(transform.position, cam.position) > 50f)
        {
            //if the projectile is far away from the main camera, it is destroyed
            Destroy(this.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPlayerProj && collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.SendMessage("TakeDamage", damage);
        }
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        //stop motion, disable collision
        hit = true;
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


}
