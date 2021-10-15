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

    //for creating gameObjects
    public GameObject enemy;

    private int gameState = 1;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        SpawnSlimes(3);
        CountEnemies();
        gameState = 1;
    }

    // Update is called once per frame
    void Update()
    {
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

    void SpawnSlimes(int num)
    {
        for (int i = 0; i < num; i++)
        {
            float x = 0;
            float y = 0;

            while (x < spawnRange/2 && x > -spawnRange / 2)
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
