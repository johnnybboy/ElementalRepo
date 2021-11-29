using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool Paused = false;
    public bool Pausable = true;
    public GameObject PauseUI, EndUI, DeathUI;
    public GameObject Player;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Pausable)
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

        PlayerController1 PC = Player.GetComponent<PlayerController1>();
        if (PC.isAlive == false)
        {
            DeathMenu();
        }
    }

    public void End()
    {
        EndUI.SetActive(true);
        Time.timeScale = 0f;
    }

    void DeathMenu()
    {
        DeathUI.SetActive(true);
        Pausable = false;
        //Time.timeScale = 0f;
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

    public void SelectLevel()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }

}
