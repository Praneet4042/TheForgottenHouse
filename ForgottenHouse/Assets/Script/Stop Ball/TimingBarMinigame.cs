using System.Collections;
using UnityEngine;
using TMPro;

public class TimingBarMinigame : MonoBehaviour
{
    [Header("Asset to Glow")]
    public Outline assetOutline;
    [Header("── Trigger ──")]
    public float interactRange = 15f;
    private Transform _player;

    [Header("── UI ──")]
    public GameObject minigamePanel;
    public RectTransform barBackground;
    public RectTransform greenZone;
    public RectTransform ball;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI instructText;
    public GameObject completedPanel;

    [Header("── Levels ──")]
    public TimingBarLevel[] levels;

    [Header("Prompt + Glow")]
    public TextMeshPro worldPrompt;
    private Outline _outline;

    // private
    private int currentLevel = 0;
    private bool uiOpen = false;
    private bool gameActive = false;
    private float ballPos = 0f;
    private float ballDir = 1f;
    private bool _completed = false;
    private bool _playerInRange = false;
    private TimingBarLevel L;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        if (completedPanel != null) completedPanel.SetActive(false);
        if (minigamePanel != null) minigamePanel.SetActive(false);

        _outline = assetOutline;
        if (_outline != null) _outline.enabled = false;

        if (worldPrompt != null) worldPrompt.text = "";
    }

    void Update()
    {
        if (worldPrompt != null && Camera.main != null)
            worldPrompt.transform.rotation = Camera.main.transform.rotation;

        if (!uiOpen && !_completed && _player != null)
        {
            float dist = Vector3.Distance(_player.position, transform.position);
            _playerInRange = dist <= interactRange;

            bool lanternOn = LanternToggle.instance != null &&
                             LanternToggle.instance.isOn;

            bool showPrompt = _playerInRange && lanternOn;
            if (_outline != null) _outline.enabled = showPrompt;
            if (worldPrompt != null)
                worldPrompt.text = showPrompt ? "[F] Stop The Ball" : "";

            if (_playerInRange && lanternOn && Input.GetKeyDown(KeyCode.F))
                OpenMinigame();
        }
        else
        {
            if (_outline != null) _outline.enabled = false;
            if (worldPrompt != null) worldPrompt.text = "";
        }

        if (!gameActive) return;

        ballPos += ballDir * L.ballSpeed * Time.deltaTime;
        if (ballPos >= L.barLength) { ballPos = L.barLength; ballDir = -1f; }
        if (ballPos <= 0f) { ballPos = 0f; ballDir = 1f; }
        ball.anchoredPosition = new Vector2(ballPos, 0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            float greenEnd = L.greenStartX + L.greenWidth;
            bool onGreen = ballPos >= L.greenStartX && ballPos <= greenEnd;
            if (onGreen) HandleSuccess();
            else HandleFail();
        }
    }

    void OpenMinigame()
    {
        uiOpen = true;
        currentLevel = 0;
        MinigameManager.Instance.StartMinigame(minigamePanel, false);
        if (completedPanel != null) completedPanel.SetActive(false);
        LoadLevel(0);
    }

    void CloseMinigame()
    {
        uiOpen = false;
        gameActive = false;
        MinigameManager.Instance.EndMinigame(minigamePanel);
    }

    void LoadLevel(int idx)
    {
        gameActive = false;
        L = levels[idx];
        barBackground.sizeDelta = new Vector2(L.barLength, barBackground.sizeDelta.y);
        greenZone.sizeDelta = new Vector2(L.greenWidth, greenZone.sizeDelta.y);
        greenZone.anchoredPosition = new Vector2(L.greenStartX, 0f);
        ball.sizeDelta = new Vector2(L.ballSize, L.ballSize);
        ball.anchoredPosition = new Vector2(0f, 0f);
        ballPos = 0f;
        ballDir = 1f;
        levelText.text = $"Level {idx + 1} / {levels.Length}";
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

    void HandleSuccess()
    {
        gameActive = false;
        bool isLast = currentLevel >= levels.Length - 1;
        if (isLast)
        {
            if (completedPanel != null) completedPanel.SetActive(true);
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
        instructText.text = "Nice! Next level...";
        yield return new WaitForSeconds(0.8f);
        LoadLevel(currentLevel);
    }

    IEnumerator FailRestart()
    {
        instructText.text = "Missed! Restarting...";
        yield return new WaitForSeconds(0.8f);
        currentLevel = 0;
        LoadLevel(0);
    }

    IEnumerator AutoClose(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseMinigame();
    }

    protected virtual void OnAllLevelsComplete()
    {
        _completed = true;
        TaskManager.Instance?.TaskCompleted();
        Debug.Log("[TimingBar] Completed! Task counted.");
    }
}