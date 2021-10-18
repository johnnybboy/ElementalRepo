using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Level1 : MonoBehaviour
{
    public int enemyCount = 0;
    public float combatArea = 25f;
    public float spawnRange = 1.5f;

    private Transform player;
    public LayerMask layer;

    // Level 1 References
    public BoxCollider2D BarrierL, BarrierR;
    public Camera Cam1, Cam2;
    public Transform SpawnSpot1;

    //for creating gameObjects
    public GameObject enemy_slime;

    private int gameState = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        Application.targetFrameRate = 60;
        CountEnemies();
        //now spawns with spawn point
        ControlledSpawn(enemy_slime, SpawnSpot1.position, 3, 0);
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
        if (player.position.x >= -12 && player.position.x <= 5 && enemyCount > 0)
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
            SpawnNearPlayer(enemy_slime, 1, spawnRange);
        }

        CountEnemies();
    }

    //This version has a default offset of 1.5f
    void ControlledSpawn(GameObject enemy, Vector2 position, int amount)
    {
        float offset = 1.5f;
        float x = position.x;
        float y = position.y;
        for (int i = 0; i < amount; i++)
        {
            Vector2 Location = new Vector2(x + Random.Range(-offset, offset), y + Random.Range(-offset, offset));
            Instantiate(enemy, Location, player.rotation);
        }
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
