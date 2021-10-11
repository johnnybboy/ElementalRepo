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
    public Image mainHeart;
    public int numberOfHearts = 0;
    private Image[] hearts;

    public float xOffset = 32f;

    // Start is called before the first frame update
    void Start()
    {
        PlaceHearts(5);
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "HEALTH : " + player.health;
        enemyCountText.text = "Enemies Left : " + gm.enemyCount;

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

    void PlaceHearts(int num)
    {
        numberOfHearts = 0;
        hearts = new Image[num];
        float spacer = mainHeart.transform.position.x;
        for (int i = 0; i < num; i++)
        {
            hearts[i] = Instantiate(mainHeart, mainHeart.transform.position, mainHeart.transform.rotation);
            hearts[i].transform.position = new Vector2(spacer, mainHeart.transform.position.y);
            spacer += xOffset;
            numberOfHearts++;
        }
    }
}
