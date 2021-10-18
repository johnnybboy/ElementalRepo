using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Winning : MonoBehaviour
{
    public Text winText;

    // Start is called before the first frame update
    void Start()
    {
        winText.enabled = true;
        winText.text = "<color=white>You Won!</color>";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
