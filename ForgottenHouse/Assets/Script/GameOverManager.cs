using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;
    public GameObject gameOverScreen;
    public TextMeshProUGUI messageText;

    void Awake() { instance = this; }

    public void ShowGameOver(string msg)
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);
        if (messageText != null)
            messageText.text = msg;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}   