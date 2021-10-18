using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private PlayerController player;
    private GameManager gm;
    
    public Text enemyCountText;
    public Text endText;
    public Text coinText;
    public Sprite manaPotion;
    public Image[] mana;
    public Transform playerPosition;

    private int maxMana;
    private int currentMana;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    void UpdateText()
    {
        enemyCountText.GetComponent<Text>().color = Color.white;
        enemyCountText.text = "<color=white>Enemies Left : </color>" + gm.enemyCount;
        coinText.GetComponent<Text>().color = Color.white;
        coinText.text = "<color=white>x </color>" + player.coins;
        if (playerPosition.position.x >= 54.5)
        {
            //endText.enabled = true;
            //endText.text = "<color=white><b>You Win!</b></color>";
            SceneManager.LoadScene("WinningScene");
        }
        if (player.health <= 0)
        {
            endText.enabled = true;
            endText.text = "<color=red><b>You Died!</b></color>";
        }
        else if (player.health > 0 && playerPosition.position.x <= 54.5)
        {
            endText.enabled = false;
        }
    }
}
