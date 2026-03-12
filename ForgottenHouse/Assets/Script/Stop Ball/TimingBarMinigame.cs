using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimingBarMinigame : MonoBehaviour
{
    [Header("── Trigger ──")]
    public float       interactRange  = 3f;
    public Transform   player;

    [Header("── UI ──")]
    public GameObject      minigamePanel;
    public RectTransform   barBackground;
    public RectTransform   greenZone;
    public RectTransform   ball;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI instructText;
    public GameObject      completedPanel;

    [Header("── Levels ──")]
    public TimingBarLevel[] levels;        // Drag 4 ScriptableObject assets here

    // ── Private ──
    private int   currentLevel = 0;
    private bool  uiOpen       = false;
    private bool  gameActive   = false;
    private float ballPos      = 0f;       // 0 to barLength
    private float ballDir      = 1f;
    private TimingBarLevel L;              // shorthand for current level data

    void Update()
    {
        if (!uiOpen && Input.GetKeyDown(KeyCode.E))
        {
            float dist = Vector3.Distance(player.position, transform.position);
            // Debug.Log($"Distance: {dist}  |  Range: {interactRange}");
            if (dist <= interactRange) OpenMinigame();
        }

        if (!gameActive) return;

        // Move ball
        ballPos += ballDir * L.ballSpeed * Time.deltaTime;

        if (ballPos >= L.barLength) { ballPos = L.barLength; ballDir = -1f; }
        if (ballPos <= 0f)          { ballPos = 0f;          ballDir =  1f; }

        // Update ball UI position (anchored to left of bar)
        ball.anchoredPosition = new Vector2(ballPos, 0f);

        // Click check
        if (Input.GetMouseButtonDown(0))
        {
            float greenEnd = L.greenStartX + L.greenWidth;
            bool onGreen   = ballPos >= L.greenStartX && ballPos <= greenEnd;

            if (onGreen) HandleSuccess();
            else         HandleFail();
        }
    }

    // ── Open / Close ─────────────────────────────────────────
    void OpenMinigame()
    {
        uiOpen        = true;
        currentLevel  = 0;
        minigamePanel.SetActive(true);
        if (completedPanel) completedPanel.SetActive(false);
        LoadLevel(0);
    }

    void CloseMinigame()
    {
        uiOpen      = false;
        gameActive  = false;
        minigamePanel.SetActive(false);
    }

    // ── Level Loading ─────────────────────────────────────────
    void LoadLevel(int idx)
    {
        gameActive = false;
        L          = levels[idx];

        // Apply bar size
        barBackground.sizeDelta = new Vector2(L.barLength, barBackground.sizeDelta.y);

        // Apply green zone
        greenZone.sizeDelta        = new Vector2(L.greenWidth, greenZone.sizeDelta.y);
        greenZone.anchoredPosition = new Vector2(L.greenStartX, 0f);

        // Apply ball size, reset position
        ball.sizeDelta        = new Vector2(L.ballSize, L.ballSize);
        ball.anchoredPosition = new Vector2(0f, 0f);
        ballPos = 0f;
        ballDir = 1f;

        levelText.text   = $"Level {idx + 1} / {levels.Length}";
        instructText.text = "Click when the ball is on GREEN!";

        StartCoroutine(StartDelay());
    }

    IEnumerator StartDelay()
    {
        instructText.text = "Get Ready...";
        yield return new WaitForSeconds(0.6f);
        instructText.text = "Click when ball is on GREEN!";
        gameActive = true;
    }

    // ── Win / Fail ────────────────────────────────────────────
    void HandleSuccess()
    {
        gameActive = false;
        bool isLast = currentLevel >= levels.Length - 1;

        if (isLast)
        {
            // Show completed screen
            if (completedPanel) completedPanel.SetActive(true);
            StartCoroutine(AutoClose(2.5f));
            OnAllLevelsComplete();
        }
        else
        {
            currentLevel++;
            StartCoroutine(NextLevelDelay());
        }
    }

    void HandleFail()
    {
        gameActive = false;
        StartCoroutine(FailRestart());
    }

    IEnumerator NextLevelDelay()
    {
        instructText.text = "✓ Nice! Next level...";
        yield return new WaitForSeconds(0.8f);
        LoadLevel(currentLevel);
    }

    IEnumerator FailRestart()
    {
        instructText.text = "✗ Missed! Restarting...";
        yield return new WaitForSeconds(0.8f);
        currentLevel = 0;
        LoadLevel(0);
    }

    IEnumerator AutoClose(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseMinigame();
    }

    // ── Override this to unlock doors etc. ───────────────────
    protected virtual void OnAllLevelsComplete()
    {
        Debug.Log("[TimingBar] All levels done! Add your unlock logic here.");
    }
}
