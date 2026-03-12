using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float typingSpeed = 0.05f;

    private string fullText;

    void Start()
    {
        fullText = "Here are the Instructions";
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        textComponent.text = "";

        foreach (char letter in fullText.ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}