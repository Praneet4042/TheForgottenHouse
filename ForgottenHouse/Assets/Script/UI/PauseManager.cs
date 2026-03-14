using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseCanvas;
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
    public void NewGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
        isPaused = false;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
