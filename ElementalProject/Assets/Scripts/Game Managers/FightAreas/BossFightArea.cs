using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightArea : MonoBehaviour
{
    //public variables
    public float triggerDistance = 5f;  //how close to the center of the area to trigger the fight
    public float boundOffset = 10f;     //how far apart the bounds are
    public float camSizeValue = 1.5f;   //multiplys camera size by this value

    //private variables
    private bool fightBegun = false;
    private bool fightComplete = false;

    //private references
    private GameObject boss;
    private GameObject player;
    private Camera cam;
    private GameObject leftBound, rightBound;

    // Start is called before the first frame update
    void Start()
    {
        //get references
        boss = GameObject.FindGameObjectWithTag("Boss");
        player = GameObject.FindGameObjectWithTag("Player");
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
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
            CheckBossStatus();
        }
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

        Debug.Log("Entered BossFightArea.");
    }

    void CheckBossStatus()
    {
        //if the boss is defeated, end the fight
        if (boss.GetComponent<EnemyController>().isAlive == false)
        {
            fightComplete = true;
            EndFight();
        }
    }

    void EndFight()
    {
        fightComplete = true;

        //return camera to player, resize
        if (CameraFollow.instance != null)
            CameraFollow.instance.SetTarget(player.transform);

        //resize camera back to original size
        cam.orthographicSize /= camSizeValue;

        //disable bounds
        leftBound.SetActive(false);
        rightBound.SetActive(false);

        Debug.Log(boss.GetComponent<EnemyController>().name + " has been defeated!");

        //destroy this FightArea, no longer needed
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow cube at the transform position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3((boundOffset - 1) * 2f, 10f, 0));
    }
}
