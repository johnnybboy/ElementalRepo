using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike_Traps : MonoBehaviour
{
    BoxCollider2D Spike1, Spike2, Spike3, Spike4, Spike5, Spike6, Spike7, Spike8;

    // Start is called before the first frame update
    void Start()
    {
        Spike1 = GameObject.Find("Trap 1").GetComponent<BoxCollider2D>();
        Spike2 = GameObject.Find("Trap 2").GetComponent<BoxCollider2D>();
        Spike3 = GameObject.Find("Trap 3").GetComponent<BoxCollider2D>();
        Spike4 = GameObject.Find("Trap 4").GetComponent<BoxCollider2D>();
        Spike5 = GameObject.Find("Trap 5").GetComponent<BoxCollider2D>();
        Spike6 = GameObject.Find("Trap 6").GetComponent<BoxCollider2D>();
        Spike7 = GameObject.Find("Trap 7").GetComponent<BoxCollider2D>();
        Spike8 = GameObject.Find("Trap 8").GetComponent<BoxCollider2D>();
        Spike1.enabled = false;
        Spike2.enabled = false;
        Spike3.enabled = false;
        Spike4.enabled = false;
        Spike5.enabled = false;
        Spike6.enabled = false;
        Spike7.enabled = false;
        Spike8.enabled = false;

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

    public void Hurt2()
    {
        Spike5.enabled = !Spike5.enabled;
        Spike6.enabled = !Spike6.enabled;
        Spike7.enabled = !Spike7.enabled;
        Spike8.enabled = !Spike8.enabled;
    }

}
