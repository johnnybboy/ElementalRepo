using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCollection : MonoBehaviour
{
    Rigidbody2D Player;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player").GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter is working...");
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Coin")
        {
            Destroy(collision.gameObject);
        }
    }

}
