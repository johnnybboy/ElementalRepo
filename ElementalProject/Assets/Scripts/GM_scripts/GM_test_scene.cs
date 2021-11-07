using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_test_scene : MonoBehaviour
{
    public int enemyCount = 0;
    public float combatArea = 25f;
    public float spawnRange = 4f;

    private Transform player;
    public LayerMask layer;

    //for creating gameObjects
    public GameObject spawnEnemy;

    private int gameState = 1;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        player = GameObject.FindGameObjectWithTag("Player").transform;
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
            SpawnNearPlayer(spawnEnemy, 1);
        }

        CountEnemies();
    }

    //This version has a default offset of spawnRange
    void ControlledSpawn(GameObject enemy, Vector2 position, int amount)
    {
        ControlledSpawn(enemy, position, amount, spawnRange);
    }

    //This version takes a float offset to manually set the offset values
    void ControlledSpawn(GameObject enemy, Vector2 position, int amount, float offsetVal)
    {
        float offset = offsetVal;
        float x = position.x;
        float y = position.y;
        for (int i = 0; i < amount; i++)
        {
            Vector2 Location = new Vector2(x + Random.Range(-offset, offset), y + Random.Range(-offset, offset));
            Instantiate(enemy, Location, player.rotation);
        }
    }

    //default spawnRange spawn near player
    void SpawnNearPlayer(GameObject enemy, int amount)
    {
        SpawnNearPlayer(enemy, amount, spawnRange);
    }

    //takes a float offsetVal
    void SpawnNearPlayer(GameObject enemy, int amount, float offsetVal)
    {
        float offset = offsetVal;
        float x = player.position.x;
        float y = player.position.y;
        for (int i = 0; i < amount; i++)
        {
            Vector2 Location = new Vector2(x + Random.Range(-offset, offset), y + Random.Range(-offset, offset));
            Instantiate(enemy, Location, player.rotation);
        }
    }

    void CountEnemies()
    {
        //get an array of all enemy colliders on the Enemy layer
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.position, combatArea, layer);

        // count them
        int count = 0;
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
