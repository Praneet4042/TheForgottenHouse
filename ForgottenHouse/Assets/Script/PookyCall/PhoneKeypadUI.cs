using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhoneKeypadUI : MonoBehaviour
{
    [Header("── Code ──")]
    public string correctCode = "147714";

    [Header("── UI References ──")]
    public TextMeshProUGUI codeDisplayText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI instructionText;
    public Button[] numberButtons;
    public GameObject keypadPanel;

    [Header("── References ──")]
    public PhoneCallManager callManager;

    // private
    private string inputCode = "";
    private bool keypadActive = false;
    private bool solved = false;
    private int maxDigits = 6;

    void Start()
    {
        if (keypadPanel) keypadPanel.SetActive(false);
        SetupButtons();
    }

    void Update()
    {
        if (!keypadActive || solved) return;

        if (Input.GetKeyDown(KeyCode.R)) ClearInput();

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (callManager) callManager.ReplayHint();
            feedbackText.text = "Replaying hint...";
        }

        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) ||
                Input.GetKeyDown(KeyCode.Keypad0 + i))
                AppendDigit(i.ToString());
        }
    }

    public void OpenKeypad()
    {
        inputCode = "";
        solved = false;
        keypadActive = true;

        if (keypadPanel) keypadPanel.SetActive(true);
        UpdateCodeDisplay();
        feedbackText.text = "";
        instructionText.text = "Type the code  |  [R] Retry  |  [C] Replay hint";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseKeypad()
    {
        keypadActive = false;
        if (keypadPanel) keypadPanel.SetActive(false);
        MinigameManager.Instance.EndMinigame(keypadPanel);
    }

    void SetupButtons()
    {
        for (int i = 0; i < numberButtons.Length; i++)
        {
            int digit = i + 1;
            if (numberButtons[i])
                numberButtons[i].onClick.AddListener(
                    () => AppendDigit(digit.ToString()));
        }
    }

    void AppendDigit(string digit)
    {
        if (!keypadActive || solved) return;
        if (inputCode.Length >= maxDigits) return;

        inputCode += digit;
        UpdateCodeDisplay();
        feedbackText.text = "";

        if (inputCode.Length == maxDigits)
            CheckCode();
    }

    void ClearInput()
    {
        inputCode = "";
        UpdateCodeDisplay();
        feedbackText.text = "";
    }

    void UpdateCodeDisplay()
    {
        string display = "";
        for (int i = 0; i < maxDigits; i++)
        {
            if (i < inputCode.Length)
                display += $"<color=white>{inputCode[i]}</color>";
            else
                display += "<color=#555555>_</color>";
            if (i < maxDigits - 1) display += "  ";
        }
        if (codeDisplayText) codeDisplayText.text = display;
    }

    void CheckCode()
    {
        if (inputCode == correctCode)
        {
            solved = true;
            feedbackText.text = "Code Accepted!";
            feedbackText.color = Color.green;
            StartCoroutine(SuccessSequence());
        }
        else
        {
            feedbackText.text = "Wrong code.  [R] retry  |  [C] replay hint";
            feedbackText.color = Color.red;
            StartCoroutine(ShakeDisplay());
        }
    }

    IEnumerator SuccessSequence()
    {
        instructionText.text = "Puzzle solved!";
        yield return new WaitForSeconds(1.5f);
        CloseKeypad();
        OnPuzzleSolved();
    }

    IEnumerator ShakeDisplay()
    {
        if (!codeDisplayText) yield break;
        Vector3 origin = codeDisplayText.rectTransform.anchoredPosition;
        for (int i = 0; i < 6; i++)
        {
            codeDisplayText.rectTransform.anchoredPosition =
                origin + new Vector3(Random.Range(-6f, 6f), 0f, 0f);
            yield return new WaitForSeconds(0.05f);
        }
        codeDisplayText.rectTransform.anchoredPosition = origin;
        yield return new WaitForSeconds(0.8f);
        ClearInput();
    }

    protected virtual void OnPuzzleSolved()
    {
        TaskManager.Instance?.TaskCompleted();
        Debug.Log("[PhoneKeypad] Solved! Task counted.");
    }
}