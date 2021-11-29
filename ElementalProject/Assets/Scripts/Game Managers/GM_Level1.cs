using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM_Level1 : MonoBehaviour
{
    //variables for scripting
    public float spawnRange = 1.5f;

    private bool inFight = false;
    private int fight_index = 0; // 0 is FightArea1, 1 is FightArea2, 2 is BossFight

    // Level 1 References
    public LayerMask layer;
    public Transform FightArea1, FightArea2, BossFight;
    //private Transform testArea;

    private GameObject player;

    //for Instantiating enemies
    public GameObject bat, slime, skullmage, boss;
    public GameObject bounds;

    //

    //private int gameState = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //testArea = GameObject.Find("TestArea").transform;

        Application.targetFrameRate = 60;   //important for game speed to be same across all computers
    }

    // Update is called once per frame
    void Update()
    {
        // First encounter, FightArea1
        if (fight_index == 0)
        {
            checkFightStatus(FightArea1, 10f);
        }

        // Second encounter, FightArea2
        if (fight_index == 1)
        {
            checkFightStatus(FightArea2, 10f);
        }

        // Boss fight, BossFight
        if (fight_index == 2)
        {
            checkFightStatus(BossFight, 10f);
        }
    }

    void checkFightStatus(Transform fightArea, float areaRadius)
    {
        //move camera to the fight area if player is close enough to the fight area
        if (!inFight && Vector2.Distance(player.transform.position, fightArea.position) <= areaRadius)
        {

            if (CameraFollow.instance == null)
            {
                Debug.Log("CameraFollow.instance is null.");
                return;
            }

            CameraFollow.instance.SetTarget(fightArea);

            inFight = true;
            Vector2 spawnArea = fightArea.position + new Vector3(0, 7f);
            ControlledSpawn(bat, spawnArea, 3);
            //bounds = Instantiate(bounds, fightArea.position, Quaternion.identity);
            bounds.transform.position = fightArea.position;

            Debug.Log("In " + fightArea + ". Enemies to fight: " + CountEnemiesNear(fightArea.position, areaRadius));
        }

        //count the enemies in the fight area, end fight if none remain
        if (inFight && CountEnemiesNear(fightArea.position, areaRadius) <= 0)
        {
            if (fight_index < 2)
            {
                //return camera to player
                if (CameraFollow.instance != null)
                    CameraFollow.instance.SetTarget(player.transform);

                //end fight, progress fight_index
                inFight = false;
                fight_index++;
                Debug.Log("Enemies defeated! " + fightArea + " is complete.");

                //move bounds
                bounds.transform.position = new Vector3(0, 20, 0);
            }
            else
            {
                GameObject levelBoss = GameObject.FindGameObjectWithTag("Boss");
                if (levelBoss == null)
                    return;

                if (!levelBoss.GetComponent<EnemyController>().isAlive)
                {
                    //end the fight, move bounds
                    if (CameraFollow.instance != null)
                        CameraFollow.instance.SetTarget(player.transform);

                    inFight = false;
                    fight_index++;
                    bounds.transform.position = new Vector3(0, 20, 0);
                }
                
            }
        }
    }

    int CountEnemiesNear(Vector2 position, float combatArea)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, combatArea, layer);
        int count = 0;
        foreach (Collider2D enemy in enemies)
            count++;

        return count;
    }


    //ControlledSpawn(GameObject, Vector2);
    //This version has a default offset of 1f
    void ControlledSpawn(GameObject enemy, Vector2 position, int amount)
    {
        float offset = 1f;
        float x = position.x;
        float y = position.y;
        for (int i = 0; i < amount; i++)
        {
            Vector2 Location = new Vector2(x + Random.Range(-offset, offset), y + Random.Range(-offset, offset));
            Instantiate(enemy, Location, player.transform.rotation);
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
            Instantiate(enemy, Location, player.transform.rotation);
        }
    }

    //SpawnNearPlayer(GameObject);
    //This version will place one object near the player (default offset of .5)
    void SpawnNearPlayer(GameObject enemy)
    {
        float offset = 2f;
        float x = player.transform.position.x;
        float y = player.transform.position.y;
        Vector2 Location = new Vector2(x + Random.Range(-offset, offset), y + Random.Range(-offset, offset));
        Instantiate(enemy, Location, player.transform.rotation);
    }

    //This version allows you to specific how many of the object and an offset range
    void SpawnNearPlayer(GameObject enemy, int amount, float offsetVal)
    {
        float offset = offsetVal;
        float x = player.transform.position.x;
        float y = player.transform.position.y;
        for (int i = 0; i < amount; i++)
        {
            Vector2 Location = new Vector2(x + Random.Range(-offset, offset), y + Random.Range(-offset, offset));
            Instantiate(enemy, Location, player.transform.rotation);
        }
    }

    
}
