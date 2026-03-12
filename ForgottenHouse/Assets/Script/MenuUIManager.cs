using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class MenuUIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject howToPlayPanel;
    public GameObject creditsPanel;

    public TextMeshProUGUI textComponent;
    public float typingSpeed = 0.05f;

    private string fullText;
    void Start()
    {
        fullText = textComponent.text;
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        howToPlayPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void ShowHowToPlay()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
        creditsPanel.SetActive(false);

        StartCoroutine(TypeText());
    }

    public void ShowCredits()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }
    public void StartGame()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator TypeText()
    {
        textComponent.text = "";

        foreach (char letter in fullText)
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}

