using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Level1()
    {
        SceneManager.LoadScene("Level One");
        Time.timeScale = 1f;
    }

    public void Level2()
    {
        SceneManager.LoadScene("Level Two");
        Time.timeScale = 1f;
    }

    public void Level3()
    {
        SceneManager.LoadScene("Level Three");
        Time.timeScale = 1f;
    }

    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
