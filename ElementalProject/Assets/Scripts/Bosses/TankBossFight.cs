using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBossFight : MonoBehaviour
{
    //private Components
    private Rigidbody2D body;
    private Animator animator;
    private EnemyMovement movement;
    private EnemyController controller;
    private ParticleSystem particles;
    private GameObject player;

    //public variables
    public Transform bossFightArea;
    public GameObject bulletPrefab, flamePrefab;
    public Transform start, enter;
    public float keepDistanceBoss = 3f;
    public float offScreenXOffset = 10f;
    public float healthState = 5f;
    // Health Bar???


    //private variables for coding
    private bool fightStarted = false;
    private bool cannonDone = false;
    private bool flameDone = false;
    private bool fightEnded = false;
    private bool movingTowardsTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movement = GetComponent<EnemyMovement>();
        controller = GetComponent<EnemyController>();
        movement = GetComponent<EnemyMovement>();
        particles = GetComponent<ParticleSystem>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (startBattle() && !fightStarted)
        {
            fightStarted = true;
            StartCoroutine(BossBattle());
        }

        if(fightEnded)
        {
            Debug.Log("You defeated the Tank Boss!!");
        }
    }

    //added to make coding easier for this transform.position
    float DistanceTo(Vector2 target)
    {
        return Vector2.Distance(transform.position, target);
    }

    public bool startBattle()
    {
        //if you step near the boss zone,
        if (Vector2.Distance(bossFightArea.position, player.transform.position) <= 5f)
        {
            return true;
        }
        else
            return false;
    }

    IEnumerator BossBattle()
    {
        fightEnded = false;

        //move the tank to startPosition
        transform.position = start.position;

        while (DistanceTo(enter.position) >= keepDistanceBoss)
        {
            StartCoroutine(MoveTowards(enter.position, movement.moveSpeed));
            yield return null;
        }

        //slam for fun, intimidation
        animator.SetTrigger("slam");
        yield return new WaitForSeconds(3);

        //begin looping between sting and slam attacks
        while (controller.isAlive)
        {
            //put up defenses


            //attempt a sting attack, wait for it to complete
            cannonDone = false;
            StartCoroutine(CannonAttack());
            while (!cannonDone)
            {
                yield return null;
            }

            //drop defenses

            //attempt a slam attack, wait for completion
            flameDone = false;
            StartCoroutine(FlameAttack());
            while (!flameDone)
            {
                yield return null;
            }
        }

        //end
        fightEnded = true;
    }

    IEnumerator CannonAttack()
    {
        //launch a barrage of energy bullets at the player


        yield return null;

        cannonDone = true;
    }

    IEnumerator FlameAttack()
    {
        //shoot flames for a few seconds


        yield return null;

        //end
        flameDone = true;
    }


    IEnumerator MoveTowards(Vector2 target, float speed)
    {
        if (!movingTowardsTarget)
        {
            movingTowardsTarget = true;

            while (Vector2.Distance(transform.position, target) > keepDistanceBoss && controller.isAlive)
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

            movingTowardsTarget = false;
        }
        
    }
}
