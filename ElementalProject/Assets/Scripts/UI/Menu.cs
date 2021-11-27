using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Level1()
    {
        SceneManager.LoadScene("Level One");
    }

    public void Level2()
    {
        SceneManager.LoadScene("Level Two");
    }

    public void Level3()
    {
        SceneManager.LoadScene("Level Three");
    }

    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
