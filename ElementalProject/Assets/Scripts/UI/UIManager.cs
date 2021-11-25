using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject player;
    private PlayerController1 playerController;
    
    public Text enemyCountText;
    public Text endText;
    public Text coinText;

    public int enemyCount = 0;
    public float combatArea = 25f;
    public float spawnRange = 1.5f;
    public LayerMask layer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController1>();
    }

    // Update is called once per frame
    void Update()
    {
        CountEnemies();
        UpdateText();
    }

    void UpdateText()
    {
        enemyCountText.GetComponent<Text>().color = Color.white;
        enemyCountText.text = "<color=white>Enemies Left : </color>" + enemyCount;
        coinText.GetComponent<Text>().color = Color.white;
        coinText.text = "<color=white>x </color>" + playerController.coins;
        
        if (playerController.health <= 0)
        {
            endText.enabled = true;
            endText.text = "<color=red><b>You Died!</b></color>";
        }
        else if (playerController.health > 0)
        {
            endText.enabled = false;
        }
    }

    void CountEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, combatArea, layer);

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
