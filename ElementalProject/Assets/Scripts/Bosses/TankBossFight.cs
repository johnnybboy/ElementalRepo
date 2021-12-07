using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBossFight : MonoBehaviour
{
    //private Components
    private Rigidbody2D body;
    private EnemyController controller;
    private GameObject player;
    private Transform cannonPoint, flamePoint_0, flamePoint_1, flamePoint_2;

    //public variables
    public Transform bossFightArea;
    public GameObject bulletPrefab, Flame_projectile;
    public float triggerDistance = 10f;
    public float barrageInterval = .3f;
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
        body = GetComponent<Rigidbody2D>();
        controller = GetComponent<EnemyController>();
        player = GameObject.FindGameObjectWithTag("Player");

        //get components in traps (3) children
        traps = transform.GetChild(3).GetComponentsInChildren<BoxCollider2D>();
        anim = transform.GetChild(3).GetComponentsInChildren<Animator>();

        cannonPoint = transform.GetChild(2).GetChild(0).gameObject.transform;
        flamePoint_0 = transform.GetChild(2).GetChild(1).gameObject.transform;
        flamePoint_1 = transform.GetChild(2).GetChild(2).gameObject.transform;
        flamePoint_2 = transform.GetChild(2).GetChild(3).gameObject.transform;

        //start spikes safe
        foreach (BoxCollider2D trap in traps)
        {
            trap.enabled = false;
        }
    }

    private void Update()
    {
        if (controller.isAlive)
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
        while (controller.isAlive)
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
        if (!controller.isAlive)    //dont do the attack if the tank is dead
            yield break;

        //launch a barrage of energy bullets at the player
        int randomNum = Random.Range(5, 10);
        for (int i = 0; i < randomNum; i++)
        {
            Instantiate(bulletPrefab, cannonPoint.position, transform.rotation);
            yield return new WaitForSeconds(barrageInterval);
        }

        yield return new WaitForSeconds(3f);

        cannonDone = true;
    }

    IEnumerator FlameAttack()
    {
        if (!controller.isAlive)    //dont do the attack if the tank is dead
            yield break;

        //shoot 3 waves of fire
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < FlameAmount; j++)
            {
                GameObject flame1 = Instantiate(Flame_projectile, flamePoint_0.position, transform.rotation);
                //flame1.GetComponent<Projectile>().BounceRange = Random.Range(-fireSpreadValue, fireSpreadValue);
                GameObject flame2 = Instantiate(Flame_projectile, flamePoint_1.position, transform.rotation);
                //flame2.GetComponent<Projectile>().BounceRange = Random.Range(-fireSpreadValue, fireSpreadValue);
                GameObject flame3 = Instantiate(Flame_projectile, flamePoint_2.position, transform.rotation);
                //flame3.GetComponent<Projectile>().BounceRange = Random.Range(-fireSpreadValue, fireSpreadValue);

                yield return new WaitForSeconds(fireInterval);
            }
            yield return new WaitForSeconds(fireWaveInterval);
        }

        yield return new WaitForSeconds(2f);

        //end
        flameDone = true;
    }
}
