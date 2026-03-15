using UnityEngine;

public class InstructionsPanel : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;
    public GameObject panel4;

    public void Panel1ToPanel2()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
    }

    public void Panel2ToPanel3()
    {
        panel2.SetActive(false);
        panel3.SetActive(true);
    }

    public void Panel2ToPanel1()
    {
        panel2.SetActive(false);
        panel1.SetActive(true);
    }

    public void Panel3ToPanel2()
    {
        panel3.SetActive(false);
        panel2.SetActive(true);
    }
    public void Panel3ToPanel4()
    {
        panel3.SetActive(false);
        panel4.SetActive(true);
    }
    public void Panel4ToPanel3()
    {
        panel4.SetActive(false);
        panel3.SetActive(true);
    }
}
