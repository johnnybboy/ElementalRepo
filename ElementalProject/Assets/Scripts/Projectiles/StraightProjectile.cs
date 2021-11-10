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
    public float projSpeed = 2f;
    public bool hasHitAnim = true;

    //private fields
    private bool hit = false;


    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        projCollider = GetComponent<Collider2D>();
        cam = GameObject.Find("Main Camera").GetComponent<Transform>();
        //player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!hit)
        {
            body.AddForce(new Vector2(projSpeed, 0f));
        }

        if (Vector2.Distance(transform.position, cam.position) > 50f)
        {
            //if the projectile is far away from the main camera, it is destroyed
            Destroy(this.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
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
