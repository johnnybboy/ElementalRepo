using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapV : MonoBehaviour
{
    public float startDelay = 0f;
    public float safeTime = 2f;
    public float bufferTime = 1f;
    public float dangerTime = 2f;

    private BoxCollider2D spikes;
    private Animator[] anim;
    private bool busy = false;
    private bool firstTrigger = true;

    // Start is called before the first frame update
    void Start()
    {
        spikes = GetComponent<BoxCollider2D>();
        spikes.enabled = false;
        anim = GetComponentsInChildren<Animator>();
        //start coroutine
        StartCoroutine(SpikeTrapTiming());
    }

    private void Update()
    {
        //start coroutine if it isn't already started
        if (!busy)
        {
            StartCoroutine(SpikeTrapTiming());
        }
    }

    IEnumerator SpikeTrapTiming()
    {
        busy = true;
        if (firstTrigger)   //delays the start of the loop for staggering traps
        {
            yield return new WaitForSeconds(startDelay);
            firstTrigger = false;
        }

        //wait for the total amount of safeTime
        yield return new WaitForSeconds(safeTime);

        //then trigger all spikes (about to extend) and wait for bufferTime
        foreach (Animator spike in anim)
        {
            spike.SetTrigger("trigger");
        }
        yield return new WaitForSeconds(bufferTime);

        //then extend all spikes and make hitbox dangerous, wait for dangerTime
        foreach (Animator spike in anim)
        {
            spike.SetTrigger("extend");
        }
        spikes.enabled = true;
        yield return new WaitForSeconds(dangerTime);

        //then retract all spikes and make hitbox safe
        foreach (Animator spike in anim)
        {
            spike.SetTrigger("retract");
        }
        spikes.enabled = false;
        busy = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(enemy.damage_medium);
        }

        if (collision.gameObject.tag == "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(player.damage_medium);
        }
    }

}
