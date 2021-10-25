using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fightArea : MonoBehaviour

    //TODO
    // NOT working currently

{
    //fight area variables
    public int enemyCount = 0;
    public float spawnRange = 1.5f;


    //barriers
    public BoxCollider2D BarrierL, BarrierR;

    //for creating gameObjects
    public GameObject enemy_slime;

    //private variables
    private float combatArea; //based on size of fight area
    private GameObject player;
    private LayerMask layer;

    // Start is called before the first frame update
    void Start()
    {
        combatArea = transform.position.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CountEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapAreaAll(transform.position, transform.position);
        int count = 0;
        foreach (Collider2D enemy in enemies)
            count++;

        if (count != enemyCount)
            enemyCount = count;
    }

    private void OnDrawGizmosSelected()
    {
        if (transform == null)
            return;
        Gizmos.DrawWireCube(transform.position, transform.position);
    }
}
