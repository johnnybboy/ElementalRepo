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
    private AudioSource breathSound, roarSound, whipSound, teleportSound;
    private Transform breathPoint;
    private ParticleSystem afterImage;

    //public variables
    public Transform bossFightArea;
    public Transform start;
    public GameObject callLightningPrefab;
    public GameObject breathLightningPrefab;
    public float meleeDistance = 3f;        //how far the boss will be when MeleeAttack()
    public float lungeForce = 10f;
    public float lightningDistance = 6f;    //how far the boss will be when CallLightning()
    public float boltCount = 10f;           //how many bolts will launch
    public float boltOffset = 1f;           //distance between bolts
    public float boltInterval = .2f;        //time interval between bolts
    public float breathDistance = 5f;       //how far the boss will be when BreathAttack()
    public float breathCount = 10f;
    public float breathModLimit = 2f;
    public float breathInterval = 3f;     //how long the breath will spawn lightning

    // Health Bar???


    //private variables for coding
    private bool fightStarted = false;
    private bool meleeAttackDone = false;
    private bool callLightningDone = false;
    private bool breathAttackDone = false;
    private bool fightEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        if (bossFightArea == null)
            bossFightArea = this.transform;
        if (start == null)
            start = this.transform;
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controller = GetComponent<BossController>();
        player = GameObject.FindGameObjectWithTag("Player");

        //AudioSources
        if (transform.Find("AudioSources").Find("BreathSound") != null)
            breathSound = transform.Find("AudioSources").Find("BreathSound").GetComponent<AudioSource>();
        if (transform.Find("AudioSources").Find("RoarSound") != null)
            roarSound = transform.Find("AudioSources").Find("RoarSound").GetComponent<AudioSource>();
        if (transform.Find("AudioSources").Find("WhipSound") != null)
            whipSound = transform.Find("AudioSources").Find("WhipSound").GetComponent<AudioSource>();
        if (transform.Find("AudioSources").Find("TeleportSound") != null)
            teleportSound = transform.Find("AudioSources").Find("TeleportSound").GetComponent<AudioSource>();

        //FirePoints
        breathPoint = transform.Find("FirePoints").Find("BreathPoint");

        //AfterImage
        if (transform.Find("AfterImage") != null)
            afterImage = transform.Find("AfterImage").GetComponent<ParticleSystem>();

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
    }

    private void TeleportToPlayer(float offsetFromPlayer)
    {
        //choose randomly which side of player to TeleportTo()
        int choice = Random.Range(0, 2);
        float distanceX;
        if (choice == 0)
            distanceX = -offsetFromPlayer;
        else
            distanceX = offsetFromPlayer;

        //TeleportTo() new location near player
        Vector2 teleportTarget = player.transform.position;
        Vector2 teleportOffset = new Vector2(distanceX, -1f);   //-1f in y to better line up
        TeleportTo(teleportTarget + teleportOffset);
    }

    private void TeleportTo(Vector2 target)
    {
        //teleport effect and sound
        if (afterImage != null)
            afterImage.Play();
        if (teleportSound != null)
            teleportSound.Play();

        //move to target
        transform.position = target;
    }

    private void LungeTowardsPlayer(float force)
    {
        if (player.transform.position.x > transform.position.x)
            body.AddForce(new Vector2(force, 0), ForceMode2D.Impulse);
        else body.AddForce(new Vector2(-force, 0), ForceMode2D.Impulse);
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

        //breath attack "roar" for intimidation, introduction
        animator.SetTrigger("breath");
        if (breathSound != null)
            breathSound.Play();
        yield return new WaitForSeconds(1f);
        if (roarSound != null)
            roarSound.Play();

        //wait for 3 seconds
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
            while (!breathAttackDone)
            {
                yield return null;
            }

            yield return null;
        }

        //end
        fightEnded = true;
    }

    IEnumerator MeleeAttack()
    {
        if (!controller.Alive())
            yield break;

        //teleport within distance of the player, wait 1 second
        TeleportToPlayer(meleeDistance);
        yield return new WaitForSeconds(1);

        //lunge and attack, wait for anim, play sound
        LungeTowardsPlayer(lungeForce);
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(.5f);
        if (whipSound != null)
            whipSound.Play();

        yield return new WaitForSeconds(2);

        //end
        meleeAttackDone = true;
    }

    IEnumerator CallLightning()
    {
        if (!controller.Alive())
            yield break;

        //teleport within distance of player, wait 1 second
        TeleportToPlayer(lightningDistance);
        yield return new WaitForSeconds(1);

        //call lightning based on boltCount, boltOffset, and current facing
        float boltDistance = 0f;
        for (int i = 0; i < boltCount; i++)
        {
            GameObject lightningBolt = Instantiate(callLightningPrefab, transform);
            lightningBolt.transform.position = lightningBolt.transform.position + new Vector3(boltDistance, 0, 0);
            if (controller.facingRight) //if facing Right, the bolts will travel right
                boltDistance += boltOffset;
            else boltDistance -= boltOffset;    //else bolts will travel left

            yield return new WaitForSeconds(boltInterval);
        }

        //wait for 2 seconds, end attack
        yield return new WaitForSeconds(2);
        callLightningDone = true;
    }

    IEnumerator BreathAttack()
    {
        if (!controller.Alive())
            yield break;

        //teleport within distance of the player, wait for 1 second
        TeleportToPlayer(breathDistance);
        yield return new WaitForSeconds(1f);

        //start animation, play sound and wait for anim
        animator.SetTrigger("breath");
        if (breathSound != null)
            breathSound.Play();
        yield return new WaitForSeconds(1f); //this is exact windup animation length

        //unleash the breathLightningPrefabs, prepare for rotation
        if (roarSound != null)
            roarSound.Play();
        Quaternion origRotation = transform.rotation;
        for (int i = 0; i < breathCount; i++)
        {
            GameObject lightningBreath = Instantiate(breathLightningPrefab, breathPoint.position, transform.rotation);
            //Randomly scale and rotate the projectile based on scale modifiers
            float randScale = Random.Range(1f, breathModLimit);
            float randDegree = Random.Range(-15f, 15f);
            lightningBreath.transform.localScale *= randScale;
            lightningBreath.transform.Rotate(0, 0, randDegree);

            //randomly rotate the dragon slightly
            int randDragonRot = Random.Range(-5, 5);
            gameObject.transform.Rotate(0, 0, randDragonRot);
            yield return new WaitForSeconds(breathInterval);
        }

        //reset rotation, wait for 3 seconds
        gameObject.transform.rotation = origRotation;
        yield return new WaitForSeconds(3f);

        breathAttackDone = true;
    }


    
}
