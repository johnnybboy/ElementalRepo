using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private GameObject player;
    private PlayerController playerController;
    
    public Text enemyCountText;
    public Text endText;
    public Text coinText;
    public Sprite manaPotion;
    public Image[] mana;

    private int maxMana;
    private int currentMana;

    public int enemyCount = 0;
    public float combatArea = 25f;
    public float spawnRange = 1.5f;
    public LayerMask layer;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
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
<<<<<<< Updated upstream
        coinText.text = "<color=white>x </color>" + playerController.coins;
        if (player.transform.position.x >= 41.4)
=======
        coinText.text = "<color=white>x </color>" + player.coins;
        if (playerPosition.position.x >= 124)
>>>>>>> Stashed changes
        {
            //endText.enabled = true;
            //endText.text = "<color=white><b>You Win!</b></color>";
            SceneManager.LoadScene("WinningScene");
        }
        if (playerController.health <= 0)
        {
            endText.enabled = true;
            endText.text = "<color=red><b>You Died!</b></color>";
        }
<<<<<<< Updated upstream
        else if (playerController.health > 0 && player.transform.position.x <= 41.4)
=======
        else if (player.health > 0 && playerPosition.position.x <= 124)
>>>>>>> Stashed changes
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
