using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private PlayerController player;
    private GameManager gm;

    public Text healthText;
    public Text enemyCountText;
    public Text endText;
    public Text coinText;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Sprite manaPotion;
    public Image[] hearts;
    public Image[] mana;

    private int maxMana;
    private int currentMana;
    private int maxHearts;
    private int currentHearts;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHearts();
        UpdateMana();
        UpdateText();
    }

    void UpdateHearts()
    {
        //update the heart values with the player script
        currentHearts = player.hearts;
        maxHearts = player.maxHearts;

        //if the player has more hearts than maximum, set it to the max visible.
        if (currentHearts > maxHearts)
        {   
            currentHearts = maxHearts;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHearts)
            {
                hearts[i].sprite = fullHeart;
            } else
            {
                hearts[i].sprite = emptyHeart;
            }
            if (i < maxHearts)
            {
                hearts[i].enabled = true;
            } else
            {
                hearts[i].enabled = false;
            }
        }
    }
    
    void UpdateMana()
    {
        //update the mana values with the player script
        currentMana = player.mana;
        maxMana = player.maxMana;

        //if the player has more mana than maximum, set it to the max visible.
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }

        for (int i = 0; i < mana.Length; i++)
        {
            if (i < currentMana)
            {
                mana[i].sprite = manaPotion;
            }
            else
            {
                mana[i].sprite = null;
            }
            if (i < maxMana)
            {
                mana[i].enabled = true;
            }
            else
            {
                mana[i].enabled = false;
            }
        }
    }

    void UpdateText()
    {
        healthText.text = "HEALTH : ";
        enemyCountText.text = "Enemies Left : " + gm.enemyCount;
        coinText.text = "COINS :        x " + player.coins;
        if (gm.enemyCount <= 0)
        {
            endText.enabled = true;
            endText.text = "You Win!";
        }
        else if (player.hearts <= 0)
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
