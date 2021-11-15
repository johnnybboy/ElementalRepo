using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedTrapManager : MonoBehaviour
{
    public float startDelay = 0f;
    public float safeTime = 2f;
    public float bufferTime = 1f;
    public float dangerTime = 2f;

    private BoxCollider2D[] traps;
    private Animator[] anim;
    private bool busy = false;
    private bool firstTrigger = true;

    // Start is called before the first frame update
    void Start()
    {
        //get components in children
        traps = GetComponentsInChildren<BoxCollider2D>();
        anim = GetComponentsInChildren<Animator>();

        //start spikes safe
        foreach (BoxCollider2D trap in traps)
        {
            trap.enabled = false;
        }

        //start coroutine
        StartCoroutine(TrapTiming());
    }

    private void Update()
    {
        //start coroutine if it isn't already started
        if (!busy)
        {
            StartCoroutine(TrapTiming());
        }
    }

    IEnumerator TrapTiming()
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
        foreach (Animator trap in anim)
        {
            trap.SetTrigger("trigger");
        }
        yield return new WaitForSeconds(bufferTime);

        //then extend all spikes and make hitbox dangerous, wait for dangerTime
        foreach (Animator trap in anim)
        {
            trap.SetTrigger("extend");
        }
        foreach (BoxCollider2D trap in traps)
        {
            trap.enabled = true;
        }
        yield return new WaitForSeconds(dangerTime);

        //then retract all spikes and make hitbox safe
        foreach (Animator trap in anim)
        {
            trap.SetTrigger("retract");
        }
        foreach (BoxCollider2D trap in traps)
        {
            trap.enabled = false;
        }
        busy = false;
    }
}
