using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightArea : MonoBehaviour
{
    //public variables
    public float triggerDistance = 5f;  //how close to the center of the area to trigger the fight
    public float boundOffset = 10f;     //how far apart the bounds are
    public float camSize;               //how large (zoomed out) the camera should be during the fight
    public float spawnRandomRange = 3f; //random range for random spawning
    public int amountOfWaves = 1;       //how many times enemies will spawn
    public int waveEnemyCount = 3;      //how many enemies will spawn per wave
    public int waveEnemyIteration = 0;  //this changes the amount of enemies per wave (negative or positive)

    public Transform spawnArea;         //should be a child under bounds, enemies will spawn here.

    //private variables
    private int currentWave = 0;

    private bool fightBegun = false;
    private bool fightComplete = false;

    //public references
    public LayerMask layer;
    public GameObject enemyToSpawn;      //enemy that will be spawned

    //private references
    private GameObject player;
    private GameObject leftBound, rightBound;

    // Start is called before the first frame update
    void Start()
    {
        //get references
        player = GameObject.FindGameObjectWithTag("Player");
        leftBound = transform.GetChild(0).gameObject;   //left should always be GetChild(0)
        rightBound = transform.GetChild(1).gameObject;   //right should always be GetChild(1)

        //position left and right bounds, localPosition should be based on parent (this)
        leftBound.transform.localPosition = new Vector2(-boundOffset, transform.position.y);
        rightBound.transform.localPosition = new Vector2(boundOffset, transform.position.y);

        //disable bounds until needed
        leftBound.SetActive(false);
        rightBound.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if the fight hasn't started and the player is close enough, StartFight()
        if (!fightBegun && Vector2.Distance(player.transform.position, transform.position) <= triggerDistance)
        {
            StartFight();
        }
        
        //if the fight is started but not complete, CheckFightStatus()
        if (fightBegun && !fightComplete)
        {
            CheckFightStatus();
        }
        
        //otherwise do nothing
    }

    void StartFight()
    {
        fightBegun = true;

        //move camera to the fight area via CameraFollow.instance
        if (CameraFollow.instance == null)
        {
            Debug.LogError("CameraFollow.instance is null.");
            return;
        }
        CameraFollow.instance.SetTarget(transform);

        //enable bounds
        leftBound.SetActive(true);
        rightBound.SetActive(true);

        //start first wave
        ControlledSpawn(enemyToSpawn, transform.position, waveEnemyCount, spawnRandomRange);

        Debug.Log("Entered FightArea.");
    }

    void CheckFightStatus()
    {
        //count the enemies in a radius of boundOffset from the center of the FightArea
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, boundOffset, layer);
        int count = 0;
        foreach (Collider2D enemy in enemies)
            count++;

        //if all enemies are defeated, progress waveCount;
        if (count <= 0)
        {
            currentWave++;

            //spawn next wave if there are more waves
            if (currentWave < amountOfWaves)
            {
                ControlledSpawn(enemyToSpawn, transform.position, waveEnemyCount, spawnRandomRange);
            }
            else
            {
                //end the fight if there are no more waves
                EndFight();
            }
        }
    }

    void EndFight()
    {
        fightComplete = true;

        //return camera to player
        if (CameraFollow.instance != null)
            CameraFollow.instance.SetTarget(player.transform);

        //disable bounds
        leftBound.SetActive(false);
        rightBound.SetActive(false);

        //destroy this FightArea, no longer needed
        Destroy(gameObject);
    }

    void ControlledSpawn(GameObject enemy, Vector2 position, int amount, float offsetVal)
    {
        float offset = offsetVal;
        float x = position.x;
        float y = position.y;
        for (int i = 0; i < amount; i++)
        {
            Vector2 spawnLocation = new Vector2(x + Random.Range(-offset, offset), y + Random.Range(-offset, offset));
            Instantiate(enemy, spawnLocation, player.transform.rotation);
        }
    }
}
