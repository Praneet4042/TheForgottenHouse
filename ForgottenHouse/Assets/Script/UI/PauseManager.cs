using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject pausePanel;

    public GameObject objectives1;
    public GameObject objectives2;
    public GameObject objectives3;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
        pausePanel.SetActive(true);
        objectives1.SetActive(false);
        objectives2.SetActive(false);
        objectives3.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;

    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
        pausePanel.SetActive(false);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    // OBJECTIVES MENU

    public void OpenObjectives()
    {
        pausePanel.SetActive(false);
        objectives1.SetActive(true);
    }

    public void NextObjective1()
    {
        objectives1.SetActive(false);
        objectives2.SetActive(true);
    }

    public void NextObjective2()
    {
        objectives2.SetActive(false);
        objectives3.SetActive(true);
    }

    public void BackObjective1()
    {
        objectives2.SetActive(false);
        objectives1.SetActive(true);
    }

    public void BackObjective2()
    {
        objectives3.SetActive(false);
        objectives2.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        objectives1.SetActive(false);
        objectives2.SetActive(false);
        objectives3.SetActive(false);

        pausePanel.SetActive(true);
    }
}