using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;

    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;
    public GameObject panel4;

    public void OpenInstructions()
    {
        mainMenuPanel.SetActive(false);
        panel1.SetActive(true);
    }

    public void Next1()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
    }

    public void Next2()
    {
        panel2.SetActive(false);
        panel3.SetActive(true);
    }

    public void Next3()
    {
        panel3.SetActive(false);
        panel4.SetActive(true);
    }

    public void Back1()
    {
        panel2.SetActive(false);
        panel1.SetActive(true);
    }

    public void Back2()
    {
        panel3.SetActive(false);
        panel2.SetActive(true);
    }

    public void Back3()
    {
        panel4.SetActive(false);
        panel3.SetActive(true);
    }

    public void ReturnToMenu()
    {
        panel4.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}