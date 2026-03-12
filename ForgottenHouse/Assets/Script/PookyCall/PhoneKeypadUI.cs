using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// PhoneKeypadUI.cs
/// 3x3 number keypad (1-9) for entering the code.
/// Correct code: 147714
/// Shows input above keypad like a PIN screen.
/// R = retry (clear input), C = replay hint call.
/// </summary>
public class PhoneKeypadUI : MonoBehaviour
{
    [Header("── Code ──")]
    [Tooltip("The correct 6-digit code")]
    public string correctCode = "147714";

    [Header("── UI References ──")]
    public TextMeshProUGUI codeDisplayText;     // Shows typed code e.g. "1 4 _ _ 1 4"
    public TextMeshProUGUI feedbackText;         // "Wrong code", "Correct!" etc.
    public TextMeshProUGUI instructionText;      // "Type the code | R=Retry | C=Replay"
    public Button[]        numberButtons;        // 9 buttons — assign in Inspector (1–9 order)
    public GameObject      keypadPanel;

    [Header("── References ──")]
    public PhoneCallManager callManager;
    public MonoBehaviour    playerMovementScript;

    // ── Private ──
    private string  inputCode     = "";
    private bool    keypadActive  = false;
    private bool    solved        = false;
    private int     maxDigits     = 6;

    void Start()
    {
        if (keypadPanel) keypadPanel.SetActive(false);
        SetupButtons();
    }

    void Update()
    {
        if (!keypadActive || solved) return;

        // R = Retry (clear input)
        if (Input.GetKeyDown(KeyCode.R))
            ClearInput();

        // C = Replay hint
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (callManager) callManager.ReplayHint();
            feedbackText.text = "Replaying hint...";
        }

        // Keyboard number input (1-9)
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) ||
                Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                AppendDigit(i.ToString());
            }
        }
    }

    // ── Called by PhoneCallManager after call ends ────────────
    public void OpenKeypad()
    {
        inputCode    = "";
        solved       = false;
        keypadActive = true;

        if (keypadPanel) keypadPanel.SetActive(true);

        UpdateCodeDisplay();
        feedbackText.text    = "";
        instructionText.text = "Type the code  |  [R] Retry  |  [C] Replay hint";

        // Unlock cursor for clicking buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    void CloseKeypad()
    {
        keypadActive = false;
        if (keypadPanel) keypadPanel.SetActive(false);

        // Restore player controls
        if (playerMovementScript)
            playerMovementScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    // ── Button Setup ──────────────────────────────────────────
    void SetupButtons()
    {
        // Buttons should be ordered 1-9 in the Inspector array
        for (int i = 0; i < numberButtons.Length; i++)
        {
            int digit = i + 1; // 1 through 9
            if (numberButtons[i])
                numberButtons[i].onClick.AddListener(() => AppendDigit(digit.ToString()));
        }
    }

    // ── Digit Input ───────────────────────────────────────────
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

    // ── Code Display ──────────────────────────────────────────
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

    // ── Code Check ───────────────────────────────────────────
    void CheckCode()
    {
        if (inputCode == correctCode)
        {
            solved = true;
            feedbackText.text  = "✓  Code Accepted!";
            feedbackText.color = Color.green;
            StartCoroutine(SuccessSequence());
        }
        else
        {
            feedbackText.text  = "✗  Wrong code.  [R] to retry  |  [C] replay hint";
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

    // Shake effect on wrong code
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

        // Auto clear after a moment
        yield return new WaitForSeconds(0.8f);
        ClearInput();
    }

    // ── Override to unlock door etc. ──────────────────────────
    protected virtual void OnPuzzleSolved()
    {
        Debug.Log("[PhoneKeypad] Puzzle solved! Add door unlock here.");
    }
}
