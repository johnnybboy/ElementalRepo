using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private PlayerController player;
    private GameManager gm;
    
    public Text enemyCountText;
    public Text endText;
    public Text coinText;
    public Sprite manaPotion;
    public Image[] mana;

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
        enemyCountText.text = "Enemies Left : " + gm.enemyCount;
        coinText.text = "x " + player.coins;
        if (gm.enemyCount <= 0)
        {
            endText.enabled = true;
            endText.text = "You Win!";
        }
        else if (player.health <= 0)
        {
            endText.enabled = true;
            endText.text = "You Died!";
        }
        else
        {
            endText.enabled = false;
        }
    }
}
