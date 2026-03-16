using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    public GameObject gameOverScreen;
    public TextMeshProUGUI messageText;

    public Transform respawnPoint;
    public int extraLives = 1;

    void Awake()
    {
        instance = this;
    }

    public void ShowGameOver(string msg)
    {
        if (extraLives > 0)
        {
            extraLives--;

            if (gameOverScreen != null)
                gameOverScreen.SetActive(true);

            if (messageText != null)
                messageText.text = "ONE MORE CHANCE...";

            StartCoroutine(RespawnPlayer());

            return;
        }

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        if (messageText != null)
            messageText.text = msg;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;
    }

    IEnumerator RespawnPlayer()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(2f);

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        player.position = respawnPoint.position + Vector3.up * 0.5f;

        yield return null;

        if (cc != null)
            cc.enabled = true;

        if (PlayerHealth.instance != null)
            PlayerHealth.instance.ResetPlayer();

        if (LanternToggle.instance != null)
        {
            LanternToggle.instance.isOn = false;
            LanternToggle.instance.lanternLight.enabled = false;
        }

        GhostAI ghost = FindObjectOfType<GhostAI>();

        if (ghost != null)
        {
            ghost.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            ghost.SetActive(true);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}