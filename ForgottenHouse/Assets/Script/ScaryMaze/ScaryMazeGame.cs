using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScaryMazeGame : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject mazeGamePanel;

    [Header("Level Objects")]
    public GameObject level1Maze;
    public GameObject level2Maze;
    public GameObject level3Maze;

    [Header("Goal Zones")]
    public RectTransform goalZone_L1;
    public RectTransform goalZone_L2;
    public RectTransform goalZone_L3;

    [Header("Walls")]
    public Image[] level1Walls;
    public Image[] level2Walls;
    public Image[] level3Walls;

    [Header("Player")]
    public RectTransform playerDot;
    public Canvas parentCanvas;

    [Header("UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI instructionText;

    [Header("Jumpscare")]
    public GameObject jumpscarePanel;
    public AudioSource audioSource;
    public AudioClip jumpscareSound;

    [Header("Interact")]
    public float interactRange = 15f;

    // private
    int currentLevel = 1;
    bool gameActive = false;
    bool jumpscared = false;
    bool _completed = false;
    bool _playerInRange = false;
    RectTransform currentGoal;
    Transform _player;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        jumpscarePanel.SetActive(false);
        mazeGamePanel.SetActive(false);
    }

    void Update()
    {
        // Interact trigger
        if (!_completed && !gameActive &&
            mazeGamePanel != null && !mazeGamePanel.activeSelf)
        {
            if (_player != null)
            {
                float dist = Vector3.Distance(
                    _player.position, transform.position);
                _playerInRange = dist <= interactRange;
            }

            if (_playerInRange && Input.GetKeyDown(KeyCode.P))
                OnInteract();
        }

        if (jumpscared) return;

        if (gameActive)
        {
            MoveDotToMouse();
            CheckWallHit();
            CheckGoal();
        }
        else
        {
            if (mazeGamePanel.activeSelf)
                CheckMouseOnSpawn();
        }
    }

    public void OnInteract()
    {
        mazeGamePanel.SetActive(true);
        MinigameManager.Instance.StartMinigame(mazeGamePanel, true);
        StartMaze();
    }

    void StartMaze()
    {
        currentLevel = 1;
        jumpscared = false;
        LoadLevel(1);
        Cursor.visible = true;
        gameActive = false;
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        playerDot.anchoredPosition = new Vector2(-380, 360);
        gameActive = false;
        instructionText.text = "Place your cursor on the blue dot to begin...";
        yield return null;
    }

    void LoadLevel(int level)
    {
        level1Maze.SetActive(false);
        level2Maze.SetActive(false);
        level3Maze.SetActive(false);

        currentLevel = level;
        levelText.text = "Level " + level;
        playerDot.anchoredPosition = new Vector2(-380, 360);

        if (level == 1)
        {
            level1Maze.SetActive(true);
            currentGoal = goalZone_L1;
        }
        else if (level == 2)
        {
            level2Maze.SetActive(true);
            currentGoal = goalZone_L2;
        }
        else if (level == 3)
        {
            level3Maze.SetActive(true);
            currentGoal = goalZone_L3;
        }
    }

    void CheckMouseOnSpawn()
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mazeGamePanel.GetComponent<RectTransform>(),
            Input.mousePosition,
            null,
            out mousePos
        );

        float distance = Vector2.Distance(mousePos, new Vector2(-380, 360));

        if (distance < 20f)
        {
            gameActive = true;
            instructionText.text = currentLevel == 1 ?
                "Reach the end - beware of what awaits you" :
                currentLevel == 2 ?
                "Stay focused. The path gets narrower..." :
                "STAND BACK AND WATCH...";
        }
    }

    void MoveDotToMouse()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mazeGamePanel.GetComponent<RectTransform>(),
            Input.mousePosition,
            null,
            out pos
        );
        playerDot.anchoredPosition = pos;
    }

    void CheckWallHit()
    {
        Image[] walls = currentLevel == 1 ? level1Walls :
                        currentLevel == 2 ? level2Walls : level3Walls;

        Rect dot = GetRect(playerDot);

        foreach (Image wall in walls)
        {
            if (dot.Overlaps(GetRect(wall.rectTransform)))
            {
                TriggerJumpscareOrRestart();
                return;
            }
        }
    }

    void CheckGoal()
    {
        if (GetRect(playerDot).Overlaps(GetRect(currentGoal)))
        {
            if (currentLevel < 3)
                StartCoroutine(GoNextLevel());
            else
                StartCoroutine(WinGame());
        }
    }

    IEnumerator GoNextLevel()
    {
        gameActive = false;
        instructionText.text = "Level " + (currentLevel + 1) + "...";
        yield return new WaitForSeconds(1.5f);
        LoadLevel(currentLevel + 1);
        playerDot.anchoredPosition = new Vector2(-380, 360);
        gameActive = false;
        instructionText.text = "Place your cursor on the blue dot to begin...";
    }

    IEnumerator WinGame()
    {
        gameActive = false;
        yield return new WaitForSeconds(0.5f);
        TriggerJumpscareOrRestart();
    }

    void TriggerJumpscareOrRestart()
    {
        if (currentLevel == 3)
        {
            // Count task regardless of win or fail on level 3
            if (!_completed)
            {
                _completed = true;
                TaskManager.Instance?.TaskCompleted();
                Debug.Log("[ScaryMaze] Completed! Task counted.");
            }
            TriggerJumpscare();
        }
        else
        {
            // Silent restart from level 1
            jumpscared = false;
            gameActive = false;
            LoadLevel(1);
            instructionText.text = "Place your cursor on the blue dot to begin...";
        }
    }

    void TriggerJumpscare()
    {
        if (jumpscared) return;
        jumpscared = true;
        gameActive = false;

        jumpscarePanel.SetActive(true);
        Cursor.visible = true;

        if (jumpscareSound != null)
            audioSource.PlayOneShot(jumpscareSound);

        StartCoroutine(AutoCloseAfterJumpscare());
    }

    IEnumerator AutoCloseAfterJumpscare()
    {
        yield return new WaitForSeconds(2.5f);
        CloseUI();
    }

    public void CloseUI()
    {
        gameActive = false;
        jumpscarePanel.SetActive(false);
        mazeGamePanel.SetActive(false);
        Cursor.visible = false;
        MinigameManager.Instance.EndMinigame(mazeGamePanel);
    }

    Rect GetRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(
            corners[0].x,
            corners[0].y,
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y
        );
    }
}