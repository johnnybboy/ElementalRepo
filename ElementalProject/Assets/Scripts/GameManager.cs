using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int enemyCount = 0;
    public float combatArea = 100f;
    public float spawnRange = 0f;

    public Transform player;
    public LayerMask layer;

    // Level 1 References
    public BoxCollider2D BarrierL, BarrierR;
    public Camera Cam1, Cam2;

    //for creating gameObjects
    public GameObject enemy;

    private int gameState = 1;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        //SpawnSlimes(3);
        ControlledSpawn(3);
        CountEnemies();
        gameState = 1;

        Cam2.enabled = false;
        BarrierL = GameObject.Find("Left Barrier").GetComponent<BoxCollider2D>();
        BarrierR = GameObject.Find("Right Barrier").GetComponent<BoxCollider2D>();
        BarrierL.enabled = false;
        BarrierR.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.x >= -12 && enemyCount > 0)
        {
            Cam2.enabled = true;
            Cam1.enabled = false;
            BarrierL.enabled = true;
        }
        else if (enemyCount <= 0)
        {
            BarrierL.enabled = false;
            BarrierR.enabled = false;
            Cam2.enabled = false;
            Cam1.enabled = true;
        }

        if (enemyCount <= 0 && gameState == 1)
        {
            //end game
            gameState = 0;

        }

        if (Input.GetKey(KeyCode.Return))
        {
            SpawnSlimes(1);
        }

        CountEnemies();
    }

    void ControlledSpawn(int num)
    {
        for (int i = 0; i < num; i++)
        {
            float x = -5;
            float y = -2;
            Vector2 Location = new Vector2(x, y);
            Instantiate(enemy, Location, player.rotation);
        }
    }

    void SpawnSlimes(int num)
    {
        for (int i = 0; i < num; i++)
        {
            float x = 0;
            float y = 0;

            while (x < spawnRange / 2 && x > -spawnRange / 2)
                x = player.position.x + Random.Range(-spawnRange, spawnRange);
            while (y < spawnRange / 2 && y > -spawnRange / 2)
                y = player.position.x + Random.Range(-spawnRange, spawnRange);

            Vector2 randomSpot = new Vector2(x, y);

            Instantiate(enemy, randomSpot, player.rotation);
        }
    }

    void CountEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.position, combatArea, layer);

        int count = 0;

        // Damage them
        foreach (Collider2D enemy in enemies)
        {
            count++;
        }

        if (count != enemyCount)
        {
            enemyCount = count;
        }
    }

}
