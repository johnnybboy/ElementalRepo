using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspQueenFight : MonoBehaviour
{
    //private Components
    private Rigidbody2D body;
    private Animator animator;
    private GameObject player;
    private BossController controller;
    private AudioSource stingSound, slamSound;

    //public variables
    public Transform bossFightArea;
    public GameObject Bee;
    public Transform start, enter;
    public float keepDistanceBoss = 3f;
    public float offScreenXOffset = 10f;
    public float abovePlayerOffset = 4f;
    public float moveSpeed = 8f;
    public float stingSpeed = 10f;
    public float slamSpeed = 10f;
    public float healthState = 5f;
    // Health Bar???


    //private variables for coding
    private bool fightStarted = false;
    private bool stingAttempted = false;
    private bool stingDone = false;
    private bool slamDone = false;
    private bool fightEnded = false;
    private bool movingTowardsTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controller = GetComponent<BossController>();
        player = GameObject.FindGameObjectWithTag("Player");

        //AudioSources
        if (transform.Find("AudioSources").Find("StingSound") != null)
            stingSound = transform.Find("AudioSources").Find("StingSound").GetComponent<AudioSource>();
        if (transform.Find("AudioSources").Find("SlamSound") != null)
            slamSound = transform.Find("AudioSources").Find("SlamSound").GetComponent<AudioSource>();
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
            Debug.Log("You defeated the Queen Wasp!!");
        }

        if (controller.GetHealth() == controller.GetHealth() - healthState)
        {
            Instantiate(Bee, new Vector2(body.position.x + 1, body.position.y + 1), transform.rotation);
            Instantiate(Bee, new Vector2(body.position.x - 1, body.position.y + 1), transform.rotation);
            Instantiate(Bee, body.position, transform.rotation);
            healthState *= 2;
        }
    }

    //added to make coding easier for this transform.position
    float DistanceTo(Vector2 target)
    {
        return Vector2.Distance(transform.position, target);
    }

    public bool startBattle()
    {
        if (player.activeSelf)
        {
            //if you step near the boss zone,
            if (Vector2.Distance(bossFightArea.position, player.transform.position) <= 5f)
            {
                return true;
            }
            else return false;
        }
        else return false;
    }

    IEnumerator BossBattle()
    {
        fightEnded = false;

        //move the queen to startPosition
        transform.position = start.position;

        while (DistanceTo(enter.position) >= keepDistanceBoss)
        {
            StartCoroutine(MoveTowards(enter.position, moveSpeed));
            yield return null;
        }

        //slam for fun, intimidation
        animator.SetTrigger("slam");
        yield return new WaitForSeconds(3);

        //begin looping between sting and slam attacks
        while (controller.Alive())
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
        stingAttempted = false; //reset sting

        //choose random direction to head towards
        int choice = Random.Range(0, 2);
        float distanceX;
        if (choice == 0)
            distanceX = -offScreenXOffset;
        else
            distanceX = offScreenXOffset;

        //set position and move off to starting position
        Vector3 offset = new Vector2(distanceX, abovePlayerOffset);
        Vector3 offScreenPosition = player.transform.position + offset;
        while (DistanceTo(offScreenPosition) >= keepDistanceBoss)
        {
            StartCoroutine(MoveTowards(offScreenPosition, moveSpeed));
            yield return null;
        }

        //move towards player, and then do a stinger attack when close enough
        while (DistanceTo(player.transform.position) >= keepDistanceBoss)
        {
            StartCoroutine(MoveTowards(player.transform.position, stingSpeed));

            if (DistanceTo(player.transform.position) <= keepDistanceBoss*2 && !stingAttempted)
            {
                //play anim, wait for anim, play sound
                animator.SetTrigger("sting");
                yield return new WaitForSeconds(.33f);
                if (stingSound != null)
                    stingSound.Play();

                stingAttempted = true;
            }

            yield return null;
        }
        

        //then fly off opposite side offScreenPosition
        offset = new Vector2(-distanceX, abovePlayerOffset);
        offScreenPosition = player.transform.position + offset;
        while (DistanceTo(offScreenPosition) >= keepDistanceBoss)
        {
            StartCoroutine(MoveTowards(offScreenPosition, moveSpeed));
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
        Vector3 offset = new Vector2(distanceX, abovePlayerOffset);
        Vector3 offScreenPosition = player.transform.position + offset;
        while (DistanceTo(offScreenPosition) >= keepDistanceBoss)
        {
            StartCoroutine(MoveTowards(offScreenPosition, moveSpeed));
            yield return null;
        }

        //move to abovePlayer, wait 1 second
        Vector3 aboveOffset = new Vector2(0, abovePlayerOffset);
        Vector3 abovePlayer = player.transform.position + aboveOffset;
        while (DistanceTo(abovePlayer) >= keepDistanceBoss)
        {
            StartCoroutine(MoveTowards(abovePlayer, moveSpeed));
            yield return null;
        }
        yield return new WaitForSeconds(1);

        //slam attack down from abovePlayer and then pause there
        Vector2 slamPosition = new Vector2(abovePlayer.x, player.transform.position.y - 1);
        
        //play anim, wait for anim, play sound
        animator.SetTrigger("slam");
        yield return new WaitForSeconds(.33f);
        if (slamSound != null)
            slamSound.Play();

        while (DistanceTo(slamPosition) >= keepDistanceBoss)
        {
            StartCoroutine(MoveTowards(slamPosition, slamSpeed));
            yield return null;
        }
        yield return new WaitForSeconds(2);

        //then fly off opposite side offScreenPosition
        offset = new Vector2(-distanceX, abovePlayerOffset);
        offScreenPosition = player.transform.position + offset;
        while (DistanceTo(offScreenPosition) >= keepDistanceBoss)
        {
            StartCoroutine(MoveTowards(offScreenPosition, moveSpeed));
            yield return null;
        }

        //end
        slamDone = true;
    }


    IEnumerator MoveTowards(Vector2 target, float speed)
    {
        if (!movingTowardsTarget)
        {
            movingTowardsTarget = true;

            while (Vector2.Distance(transform.position, target) > keepDistanceBoss && controller.Alive())
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
