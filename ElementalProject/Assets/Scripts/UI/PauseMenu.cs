using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool Paused = false;
    public GameObject PauseUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Resume()
    {
        PauseUI.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

    void Pause()
    {
        PauseUI.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }

    public void Back()
    {
        PauseUI.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

}
