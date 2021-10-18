using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike_Traps : MonoBehaviour
{
    BoxCollider2D Spike1, Spike2, Spike3, Spike4;

    // Start is called before the first frame update
    void Start()
    {
        Spike1 = GameObject.Find("Trap").GetComponent<BoxCollider2D>();
        Spike2 = GameObject.Find("Trap (1)").GetComponent<BoxCollider2D>();
        Spike3 = GameObject.Find("Trap (2)").GetComponent<BoxCollider2D>();
        Spike4 = GameObject.Find("Trap (3)").GetComponent<BoxCollider2D>();
        Spike1.enabled = false;
        Spike2.enabled = false;
        Spike3.enabled = false;
        Spike4.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Hurt()
    {
        Spike1.enabled = !Spike1.enabled;
        Spike2.enabled = !Spike2.enabled;
        Spike3.enabled = !Spike3.enabled;
        Spike4.enabled = !Spike4.enabled;
    }

}
