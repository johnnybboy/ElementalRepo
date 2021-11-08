using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspQueenFight : MonoBehaviour
{
    //private Components
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;
    private EnemyMovement movement;
    private EnemyController controller;
    private ParticleSystem particles;
    private GameObject player;

    //public variables
    public Transform bossFightArea;
    public GameObject queen_wasp_object;

    public Transform start, enter;
    public float keepDistanceBoss = 3f;
    public float offScreenXOffset = 20f;
    public float stingSpeed = 10f;
    public float slamSpeed = 10f;
    // Health Bar???


    //private variables for coding
    private bool fightStarted = false;
    private bool stingAttempted = false;
    private bool stingDone = false;
    private bool slamDone = false;
    private bool fightEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
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
            BossBattle();
            fightStarted = true;
        }

        if(fightEnded)
        {
            Debug.Log("You defeated the Queen Wasp!!");
        }
    }

    public bool startBattle()
    {
        //Here is where you put the condition to start the boss fight.
        return false;

        //if you step near the boss zone,

        //return true;
    }

    IEnumerator BossBattle()
    {
        fightEnded = false;

        //move the queen to outside position, startPosition
        transform.position = start.position;

        while (Vector2.Distance(transform.position, enter.position) >= keepDistanceBoss)
        {
            StartCoroutine(movement.MoveTowards(enter.position, movement.moveSpeed));
            yield return null;
        }

        //slam for fun, intimidation
        animator.SetTrigger("slam");
        yield return new WaitForSeconds(2);

        //begin looping between sting and slam attacks
        while (controller.isAlive)
        {
            //attempt a sting attack, wait for it to complete
            stingDone = false;
            StartCoroutine(StingAttack());
            while (!stingDone)
            {
                yield return null;
            }

            //attempt a slam attack, wait for completion
            slamDone = false;
            StartCoroutine(SlamAttack());
            while (!slamDone)
            {
                yield return null;
            }
        }

        //end
        fightEnded = true;
    }

    IEnumerator StingAttack()
    {
        //choose random direction to head towards
        int choice = Random.Range(0, 2);
        float distanceX;
        if (choice == 0)
            distanceX = -offScreenXOffset;
        else
            distanceX = offScreenXOffset;

        //set position and move off to starting position
        Vector2 offScreenPosition = new Vector2(distanceX, player.transform.position.y);
        while (Vector2.Distance(transform.position, offScreenPosition) >= keepDistanceBoss)
        {
            StartCoroutine(movement.MoveTowards(offScreenPosition, movement.moveSpeed));
            yield return null;
        }

        //move towards player, and then do a stinger attack when close enough
        while (Vector2.Distance(transform.position, player.transform.position) >= keepDistanceBoss)
        {
            StartCoroutine(movement.MoveTowards(player.transform.position, stingSpeed));

            if (Vector2.Distance(transform.position, player.transform.position) <= keepDistanceBoss*2 && !stingAttempted)
            {
                animator.SetTrigger("sting");
                stingAttempted = true;
            }

            yield return null;
        }

        //then fly off opposite side offScreenPosition
        offScreenPosition = new Vector2(-distanceX, player.transform.position.y);
        while (Vector2.Distance(transform.position, offScreenPosition) >= keepDistanceBoss)
        {
            StartCoroutine(movement.MoveTowards(offScreenPosition, movement.moveSpeed));
            yield return null;
        }

        stingDone = true;
    }

    IEnumerator SlamAttack()
    {
        //choose random direction to head towards
        int choice = Random.Range(0, 2);
        float distanceX;
        if (choice == 0)
            distanceX = -offScreenXOffset;
        else
            distanceX = offScreenXOffset;

        //set position and move to offScreenPosition
        Vector2 offScreenPosition = new Vector2(distanceX, player.transform.position.y);
        while (Vector2.Distance(transform.position, offScreenPosition) >= keepDistanceBoss)
        {
            StartCoroutine(movement.MoveTowards(offScreenPosition, movement.moveSpeed));
            yield return null;
        }

        //move to abovePlayer, wait 1 second
        Vector2 abovePlayer = new Vector2(player.transform.position.x, 10f);
        while (Vector2.Distance(transform.position, abovePlayer) >= keepDistanceBoss)
        {
            StartCoroutine(movement.MoveTowards(abovePlayer, movement.moveSpeed));
            yield return null;
        }
        yield return new WaitForSeconds(1);

        //slam attack down from abovePlayer and then pause there
        Vector2 slamPosition = new Vector2(abovePlayer.x, player.transform.position.y);
        while (Vector2.Distance(transform.position, player.transform.position) >= keepDistanceBoss)
        {
            StartCoroutine(movement.MoveTowards(slamPosition, slamSpeed));
            animator.SetTrigger("slam");
            yield return null;
        }
        yield return new WaitForSeconds(2);

        //then fly off opposite side offScreenPosition
        offScreenPosition = new Vector2(-distanceX, player.transform.position.y);
        while (Vector2.Distance(transform.position, offScreenPosition) >= .5f)
        {
            StartCoroutine(movement.MoveTowards(offScreenPosition, movement.moveSpeed));
            yield return null;
        }

        //end
        slamDone = true;
    }
}
