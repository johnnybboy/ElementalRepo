using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool Paused = false;
    private bool Pausable = true;
    public GameObject PauseUI, EndUI, DeathUI;
    private GameObject Player;
    public GameObject P1, P2;
    public Text winTime;
    private float Timer;
    private Menu Selection;

    void Awake()
    {
        Selection = GameObject.Find("New Canvas").GetComponent<Menu>();
        if (Selection.getChosenPlayer() == 1)
        {
            P1.SetActive(true);
        }
        else if (Selection.getChosenPlayer() == 2)
        {
            P2.SetActive(true);
        }
        // Prevents errors from happening when skipping the main menu
        else
        {
            P1.SetActive(true); // Defaults to P1
        }
    }

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

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

        Timer = Time.timeSinceLevelLoad / 60;
    }

    public void End()
    {
        EndUI.SetActive(true);
        Time.timeScale = 0f;
        Timer = Mathf.Round(Timer * 100f) / 100f;
        winTime.text = "<color=white>Time Taken: </color>" + Timer + "<color=white> min</color>";
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

    public void setUnpausable()
    {
        Pausable = false;
    }

    public void setPausable()
    {
        Pausable = true;
    }

}
