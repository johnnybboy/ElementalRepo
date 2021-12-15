using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightArea : MonoBehaviour
{
    //public variables
    public float triggerDistance = 5f;  //how close to the center of the area to trigger the fight
    public float boundOffset = 10f;     //how far apart the bounds are
    public float camSizeValue = 1f;   //multiples camera size by this value
    public float spawnRandomRange = 1f; //random range for random spawning
    public int amountOfWaves = 1;       //how many times enemies will spawn
    public int waveSpawnCount = 3;      //how many enemies will spawn per wave
    public int waveSpawnIteration = 0;  //this changes the amount of enemies per wave (negative or positive)

    public Transform spawnArea;         //should be a child under bounds, enemies will spawn here.

    //private variables
    private int currentWave = 0;
    private int currentSpawnCount;

    private bool fightBegun = false;
    private bool fightComplete = false;
    private bool checkingStatus = false;

    //public references
    public LayerMask layer;
    public GameObject enemyToSpawn;      //enemy that will be spawned

    //private references
    private GameObject player;
    private GameObject leftBound, rightBound;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        //get references
        player = GameObject.FindGameObjectWithTag("Player");
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        leftBound = transform.GetChild(0).gameObject;   //left should always be GetChild(0)
        rightBound = transform.GetChild(1).gameObject;   //right should always be GetChild(1)

        //disable sprite renderers
        SpriteRenderer lb = leftBound.GetComponent<SpriteRenderer>();
        SpriteRenderer rb = rightBound.GetComponent<SpriteRenderer>();
        lb.enabled = false;
        rb.enabled = false;

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
            if (!checkingStatus)
                StartCoroutine(CheckFightStatus());
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

        //size camera for the fight
        cam.orthographicSize *= camSizeValue;

        //enable bounds
        leftBound.SetActive(true);
        rightBound.SetActive(true);

        //start first wave
        Debug.Log("Entered FightArea.");
        currentSpawnCount = waveSpawnCount;
        if (enemyToSpawn != null)
            ControlledSpawn(enemyToSpawn, spawnArea.position, currentSpawnCount, spawnRandomRange);
        else Debug.LogError("No enemy prefab assigned to spawn!");
    }

    IEnumerator CheckFightStatus()
    {
        if (checkingStatus)
            yield break;

        checkingStatus = true;
        //count the enemies in a radius of boundOffset from the center of the FightArea
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, boundOffset, layer);
        int count = 0;
        foreach (Collider2D enemy in enemies)
            count++;

        //if all enemies are defeated, progress waveCount;
        if (count <= 0)
        {
            //iterate the currentWave and currentSpawnCount
            currentWave++;
            currentSpawnCount += waveSpawnIteration;
            if (currentSpawnCount < 0)
                currentSpawnCount = 0;

            //spawn next wave if there are more waves
            if (currentWave < amountOfWaves)
            {
                ControlledSpawn(enemyToSpawn, spawnArea.position, currentSpawnCount, spawnRandomRange);
            }
            else
            {
                //end the fight if there are no more waves
                EndFight();
            }
        }

        yield return new WaitForSeconds(1f);
        checkingStatus = false;
    }

    void EndFight()
    {
        fightComplete = true;

        //return camera to player
        if (CameraFollow.instance != null)
            CameraFollow.instance.SetTarget(player.transform);

        //resize camera back to original size
        cam.orthographicSize /= camSizeValue;

        //disable bounds
        leftBound.SetActive(false);
        rightBound.SetActive(false);

        //destroy this FightArea, no longer needed
        Destroy(gameObject);
    }

    void ControlledSpawn(GameObject enemy, Vector2 position, int amount, float offsetVal)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 spawnLocation = new Vector2(position.x + Random.Range(-offsetVal, offsetVal), position.y + Random.Range(-offsetVal, offsetVal));
            Instantiate(enemy, spawnLocation, player.transform.rotation);
            //Instantiate(enemy, position, player.transform.rotation);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow cube at the transform position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3((boundOffset-1)*2f, 10f, 0));
    }

    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(4);
    }
}
