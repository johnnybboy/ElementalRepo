using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempScript : MonoBehaviour
{
    public SpriteRenderer[] All;
    public PolygonCollider2D[] all;
    public Transform player;
    public GameObject a, b, c, d, e, f;

    // Start is called before the first frame update
    void Start()
    {
        All = GetComponentsInChildren<SpriteRenderer>();
        all = GetComponentsInChildren<PolygonCollider2D>();
        a.GetComponent<SawbladeManager>().enabled = false;
        b.GetComponent<SawbladeManager>().enabled = false;
        c.GetComponent<SawbladeManager>().enabled = false;
        d.GetComponent<SawbladeManager>().enabled = false;
        e.GetComponent<SawbladeManager>().enabled = false;
        f.GetComponent<SawbladeManager>().enabled = false;
        for (int i = 0; i < 5; i++)
        {
            foreach (var sr in All)
            {
                sr.enabled = false;
            }
            foreach (var sr in all)
            {
                sr.enabled = false;
            }
        }
     
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.x > 199)
        {
            a.GetComponent<SawbladeManager>().enabled = true;
            b.GetComponent<SawbladeManager>().enabled = true;
            c.GetComponent<SawbladeManager>().enabled = true;
            d.GetComponent<SawbladeManager>().enabled = true;
            e.GetComponent<SawbladeManager>().enabled = true;
            f.GetComponent<SawbladeManager>().enabled = true;

            for (int i = 0; i < 5; i++)
            {
                foreach (var sr in All)
                {
                    sr.enabled = true;
                }
                foreach (var sr in all)
                {
                    sr.enabled = true;
                }
            }
            
        }
    }
}
