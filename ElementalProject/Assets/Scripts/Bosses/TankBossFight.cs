using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public class TankBossFight : MonoBehaviour
{
    //private Components
    private Animator animator;
    private BossController controller;
    private GameObject player;
    private Transform cannonPoint, flamePoint_0, flamePoint_1, flamePoint_2;
    private AudioSource cannonSound, flameSound, flameLaunchSound;
    private Light2D Oran1, Oran2, Oran3, Oran4, Oran5, Oran6;
    private Light2D Lred1, Lred2, Lred3, Lred4, Lred5, Lred6;
    private Light2D Dred1, Dred2, Dred3;
    //public variables
    public Transform bossFightArea;
    public GameObject bulletPrefab, Flame_projectile;
    public float triggerDistance = 10f;
    public float cannonInterval = .3f;
    public float FlameAttackRange = 10.0f;
    public float FlameAmount = 50f;
    public float fireInterval = 0.1f;
    public float fireWaveInterval = 1f;
    public float fireSpreadValue = 6f;

    // Health Bar???


    //private variables for coding
    private bool fightStarted = false;
    private bool cannonDone = false;
    private bool flameDone = false;
    private bool fightEnded = false;

    private BoxCollider2D[] traps;
    private Animator[] anim;

    // Start is called before the first frame update
    void Start()
    {
        if (bossFightArea == null)
        {
            bossFightArea = this.transform;
        }
        animator = GetComponent<Animator>();
        controller = GetComponent<BossController>();
        player = GameObject.FindGameObjectWithTag("Player");

        //AudioSources are GetChild(1)
        cannonSound = transform.GetChild(1).GetChild(3).gameObject.GetComponent<AudioSource>();
        flameSound = transform.GetChild(1).GetChild(4).gameObject.GetComponent<AudioSource>();
        flameLaunchSound = transform.GetChild(1).GetChild(5).gameObject.GetComponent<AudioSource>();

        //firePoints are GetChild(2)
        cannonPoint = transform.GetChild(2).GetChild(0).gameObject.transform;
        flamePoint_0 = transform.GetChild(2).GetChild(1).gameObject.transform;
        flamePoint_1 = transform.GetChild(2).GetChild(2).gameObject.transform;
        flamePoint_2 = transform.GetChild(2).GetChild(3).gameObject.transform;

        //fireTraps are GetChild(3)
        traps = transform.GetChild(3).GetComponentsInChildren<BoxCollider2D>();
        anim = transform.GetChild(3).GetComponentsInChildren<Animator>();

        //lights are GetChild(6)
        //orange Lights
        Oran1 = transform.GetChild(5).GetChild(0).GetChild(9).GetComponent<Light2D>();
        Oran2 = transform.GetChild(5).GetChild(0).GetChild(10).GetComponent<Light2D>();
        Oran3 = transform.GetChild(5).GetChild(0).GetChild(11).GetComponent<Light2D>();
        Oran4 = transform.GetChild(5).GetChild(0).GetChild(12).GetComponent<Light2D>();
        Oran5 = transform.GetChild(5).GetChild(0).GetChild(13).GetComponent<Light2D>();
        Oran6 = transform.GetChild(5).GetChild(0).GetChild(14).GetComponent<Light2D>();
        //Light Red Lights
        Lred1 = transform.GetChild(5).GetChild(0).GetChild(3).GetComponent<Light2D>();
        Lred2 = transform.GetChild(5).GetChild(0).GetChild(4).GetComponent<Light2D>();
        Lred3 = transform.GetChild(5).GetChild(0).GetChild(5).GetComponent<Light2D>();
        Lred4 = transform.GetChild(5).GetChild(0).GetChild(6).GetComponent<Light2D>();
        Lred5 = transform.GetChild(5).GetChild(0).GetChild(7).GetComponent<Light2D>();
        Lred6 = transform.GetChild(5).GetChild(0).GetChild(8).GetComponent<Light2D>();
        //Dark Red Lights
        Dred1 = transform.GetChild(5).GetChild(0).GetChild(0).GetComponent<Light2D>();
        Dred2 = transform.GetChild(5).GetChild(0).GetChild(1).GetComponent<Light2D>();
        Dred3 = transform.GetChild(5).GetChild(0).GetChild(2).GetComponent<Light2D>();
        //
        Oran1.intensity = 0f;
        Oran2.intensity = 0f;
        Oran3.intensity = 0f;
        Oran4.intensity = 0f;
        Oran5.intensity = 0f;
        Oran6.intensity = 0f;

        Lred1.intensity = 0f;
        Lred2.intensity = 0f;
        Lred3.intensity = 0f;
        Lred4.intensity = 0f;
        Lred5.intensity = 0f;
        Lred6.intensity = 0f;

        Dred1.intensity = 0f;
        Dred2.intensity = 0f;
        Dred3.intensity = 0f;
        //start spikes safe
        foreach (BoxCollider2D trap in traps)
        {
            trap.enabled = false;
        }
    }

    private void Update()
    {
        if (controller.Alive())
        {
            if (startBattle() && !fightStarted)
            {
                fightStarted = true;
                StartCoroutine(BossBattle());
            }

            if (fightEnded)
            {
                Debug.Log("You defeated the Tank Boss!!");
            }
        }

        else this.enabled = false;  //should deactivate this script on death
    }

    public bool startBattle()
    {
        //if you step near the boss zone,
        if (Vector2.Distance(bossFightArea.position, player.transform.position) <= triggerDistance)
        {
            return true;
        }
        else
            return false;
    }

    IEnumerator BossBattle()
    {
        fightEnded = false;

        //begin looping between flame up, cannon, flame down, flamethrower
        while (controller.Alive())
        {
            //trigger defenses to warn player, wait for 2 seconds
            foreach (Animator trap in anim)
            {
                trap.SetTrigger("trigger");
            }
            yield return new WaitForSeconds(2f);

            //put up defenses
            foreach (Animator trap in anim)
            {
                trap.SetTrigger("extend");
            }
            foreach (BoxCollider2D trap in traps)
            {
                trap.enabled = true;
            }
            yield return new WaitForSeconds(2f);

            //shoot a barrage of energy bullets at the player
            cannonDone = false;
            StartCoroutine(CannonAttack());
            while (!cannonDone)
            {
                yield return null;
            }

            //drop defenses
            foreach (Animator trap in anim)
            {
                trap.SetTrigger("retract");
            }
            foreach (BoxCollider2D trap in traps)
            {
                trap.enabled = false;
            }
            yield return new WaitForSeconds(2f);

            //shoot a jet of flame
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
        if (!controller.Alive())    //dont do the attack if the tank is dead
            yield break;

        //play warning animation
        animator.SetTrigger("cannon");
        yield return new WaitForSeconds(3f/8f);

        //launch a barrage of energy bullets at the player
        int randomNum = Random.Range(5, 10);
        for (int i = 0; i < randomNum; i++)
        {
            //play audio
            if (cannonSound != null)
                cannonSound.Play();
            Instantiate(bulletPrefab, cannonPoint.position, transform.rotation);
            yield return new WaitForSeconds(cannonInterval);
        }

        yield return new WaitForSeconds(3f);

        cannonDone = true;
    }

    IEnumerator FlameAttack()
    {
        if (!controller.Alive())    //dont do the attack if the tank is dead
            yield break;

        //play warning animation
        animator.SetTrigger("flame");
        Oran1.intensity = 3.5f;
        Oran2.intensity = 3.5f;
        Oran3.intensity = 3.5f;
        Oran4.intensity = 3.5f;
        Oran5.intensity = 3.5f;
        Oran6.intensity = 3.5f;
        yield return new WaitForSeconds(.1f);
        Lred1.intensity = 4f;
        Lred2.intensity = 4f;
        Lred3.intensity = 4f;
        Lred4.intensity = 4f;
        Lred5.intensity = 4f;
        Lred6.intensity = 4f;
        yield return new WaitForSeconds(.1f);
        Dred1.intensity = 5.5f;
        Dred2.intensity = 5.5f;
        Dred3.intensity = 5.5f;
        yield return new WaitForSeconds(3f / 8f);

        //shoot 3 waves of fire
        if (flameSound != null)
            flameSound.Play();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < FlameAmount; j++)
            {
                if (flameLaunchSound != null)
                    flameLaunchSound.Play();
                GameObject flame1 = Instantiate(Flame_projectile, flamePoint_0.position, transform.rotation);
                GameObject flame2 = Instantiate(Flame_projectile, flamePoint_1.position, transform.rotation);
                GameObject flame3 = Instantiate(Flame_projectile, flamePoint_2.position, transform.rotation);

                yield return new WaitForSeconds(fireInterval);
            }
            yield return new WaitForSeconds(fireWaveInterval);
        }

        yield return new WaitForSeconds(2f);
        Oran1.intensity =0f;
        Oran2.intensity = 0f;
        Oran3.intensity = 0f;
        Oran4.intensity = 0f;
        Oran5.intensity = 0f;
        Oran6.intensity = 0f;
       
        Lred1.intensity = 0f;
        Lred2.intensity = 0f;
        Lred3.intensity = 0f;
        Lred4.intensity = 0f;
        Lred5.intensity = 0f;
        Lred6.intensity = 0f;
        
        Dred1.intensity = 0f;
        Dred2.intensity = 0f;
        Dred3.intensity = 0f;
        //end
        flameDone = true;
    }
}
