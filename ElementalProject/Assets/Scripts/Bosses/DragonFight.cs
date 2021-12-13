using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFight : MonoBehaviour
{
    //private Components
    private Rigidbody2D body;
    private Animator animator;
    private GameObject player;
    private BossController controller;
    private AudioSource breathSound, attackSound, callSound;

    //public variables
    public Transform bossFightArea;
    public Transform start, enter;
    public GameObject callLightningPrefab;
    public GameObject breathLightningPrefab;
    public float meleeDistance = 3f;
    public float lightningDistance = 6f;
    public float boltCount = 10f;
    public float breathTimeLength = 3f;
    public float attackPatternInterval = 3f;

    // Health Bar???


    //private variables for coding
    private float currentTime = 0f;
    private bool fightStarted = false;
    private bool meleeAttackDone = false;
    private bool callLightningDone = false;
    private bool breathAttackDone = false;
    private bool fightEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controller = GetComponent<BossController>();
        player = GameObject.FindGameObjectWithTag("Player");

        //AudioSources
        if (transform.Find("AudioSources").Find("BreathSound") != null)
            breathSound = transform.Find("AudioSources").Find("BreathSound").GetComponent<AudioSource>();
        if (transform.Find("AudioSources").Find("AttackSound") != null)
            attackSound = transform.Find("AudioSources").Find("AttackSound").GetComponent<AudioSource>();
        if (transform.Find("AudioSources").Find("CallSound") != null)
            callSound = transform.Find("AudioSources").Find("CallSound").GetComponent<AudioSource>();
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
            Debug.Log("You defeated the Lightning Dragon!!");
        }

        //if (controller.GetHealth() == controller.GetHealth() - healthState)
        //{
        //    Instantiate(Bee, new Vector2(body.position.x + 1, body.position.y + 1), transform.rotation);
        //    Instantiate(Bee, new Vector2(body.position.x - 1, body.position.y + 1), transform.rotation);
        //    Instantiate(Bee, body.position, transform.rotation);
        //    healthState *= 2;
        //}
    }

    //added to make coding easier for this transform.position
    private float DistanceTo(Vector2 target)
    {
        return Vector2.Distance(transform.position, target);
    }

    private void TeleportTo(Vector2 target)
    {
        //teleport to target
        //TODO teleport effect
        transform.position = start.position;
    }

    public bool startBattle()
    {
        if (player.activeSelf)
        {
            //if player steps near the boss zone, return true
            if (Vector2.Distance(bossFightArea.position, player.transform.position) <= 5f)
            {
                return true;
            }
            else return false;
        }
        else return false;
    }

    private IEnumerator BossBattle()
    {
        fightEnded = false;

        //teleport to startPosition
        TeleportTo(start.position);

        //breath attack "roar" for intimidation
        animator.SetTrigger("breath");
        yield return new WaitForSeconds(3);

        //begin looping between attack, call lighting, and breath attacks
        while (controller.Alive())
        {
            //attempt a melee attack, wait for it to complete
            meleeAttackDone = false;
            StartCoroutine(MeleeAttack());
            while (!meleeAttackDone)
            {
                yield return null;
            }

            //attempt to call lightning, wait for completion
            callLightningDone = false;
            StartCoroutine(CallLightning());
            while (!callLightningDone)
            {
                yield return null;
            }

            //attempt to breath attack, wait for completion
            breathAttackDone = false;
            StartCoroutine(BreathAttack());
            while (!callLightningDone)
            {
                yield return null;
            }
        }

        //end
        fightEnded = true;
    }

    IEnumerator MeleeAttack()
    {
        if (!controller.Alive())
            yield break;

        //choose randomly which side of player to TeleportTo()
        int choice = Random.Range(0, 2);
        float distanceX;
        if (choice == 0)
            distanceX = -meleeDistance;
        else
            distanceX = meleeDistance;

        //TeleportTo() new location near player
        Vector2 teleportTarget = player.transform.position;
        Vector2 teleportOffset = new Vector2(distanceX, 0);
        TeleportTo(teleportTarget + teleportOffset);

        //make sure facing towards player
        if (DistanceTo(player.transform.position) > 0)
        {
            if (!controller.facingRight)
                controller.FlipFacing();
        }
        else
        {
            if (controller.facingRight)
                controller.FlipFacing();
        }

        //wait for 1 second, then attack
        yield return new WaitForSeconds(1);
        animator.SetTrigger("attack");

        //wait for 2 seconds, end attack
        yield return new WaitForSeconds(2);
        meleeAttackDone = true;
    }

    IEnumerator CallLightning()
    {
        if (!controller.Alive())
            yield break;

        //choose randomly which side of player to TeleportTo()
        int choice = Random.Range(0, 2);
        float distanceX;
        if (choice == 0)
            distanceX = -lightningDistance;
        else
            distanceX = lightningDistance;

        //TeleportTo() new location away from player
        Vector2 teleportTarget = player.transform.position;
        Vector2 teleportOffset = new Vector2(distanceX, 0);
        TeleportTo(teleportTarget + teleportOffset);

        //wait for 1 second, then call lightning based on facing
        yield return new WaitForSeconds(1);
        if (controller.facingRight)
        {
            for(int i = 0; i < boltCount; i++)
            {
                Instantiate(callLightningPrefab, transform);
            }
        }

        //wait for 2 seconds, end attack
        yield return new WaitForSeconds(2);
        meleeAttackDone = true;
    }

    IEnumerator BreathAttack()
    {
        yield return null;
    }


    
}
