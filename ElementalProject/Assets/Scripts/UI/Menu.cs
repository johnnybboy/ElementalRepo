using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private int chosenLevel;
    private static int chosenPlayer;

    public void Water()
    {
        chosenPlayer = 1;
    }

    public void Fire()
    {
        chosenPlayer = 2;
    }

    public void Forest()
    {
        chosenLevel = 1;
    }

    public void Factory()
    {
        chosenLevel = 2;
    }

    public void Temple()
    {
        chosenLevel = 3;
    }

    public void Ready()
    {
        SceneManager.LoadScene(chosenLevel);
        Time.timeScale = 1f;
    }

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

    public int getChosenPlayer()
    {
        return chosenPlayer;
    }
}
