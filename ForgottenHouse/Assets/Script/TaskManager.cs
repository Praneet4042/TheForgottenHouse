using UnityEngine;
using TMPro;
using System.Collections;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI taskCounterText;
    public int totalTasks = 8;

    [Header("Door 1 — LR to Hall")]
    public DoorUnlock door1;
    public PillBox pillBox1;
    public int door1UnlockAt = 2;

    [Header("Door 2 — Hall to Yard")]
    public DoorUnlock door2Left;
    public DoorUnlock door2Right;
    public PillBox pillBox2;
    public int door2UnlockAt = 4;

    [Header("Exit Gate")]
    public DoorUnlock exitGateLeft;
    public DoorUnlock exitGateRight;
    public ExitGate exitGateTrigger;
    public GameObject completionUI;
    public int exitUnlockAt = 8;

    private int _tasksCompleted = 0;

    private bool _door1Opened = false;
    private bool _door2Opened = false;
    private bool _exitOpened = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public int TasksCompleted => _tasksCompleted;

    public void TaskCompleted()
    {
        _tasksCompleted++;

        UpdateUI();

        Debug.Log($"[TaskManager] {_tasksCompleted}/{totalTasks}");

        // Door 1 unlock
        if (!_door1Opened && _tasksCompleted >= door1UnlockAt)
        {
            _door1Opened = true;

            if (door1 != null)
                door1.UnlockDoor();

            if (pillBox1 != null)
                pillBox1.Appear();

            Debug.Log("Door 1 unlocked");
        }

        // Door 2 unlock
        if (!_door2Opened && _tasksCompleted >= door2UnlockAt)
        {
            _door2Opened = true;

            if (door2Left != null)
                door2Left.UnlockDoor();

            if (door2Right != null)
                door2Right.UnlockDoor();

            if (pillBox2 != null)
                pillBox2.Appear();

            Debug.Log("Door 2 unlocked");
        }

        // Exit unlock
        if (!_exitOpened && _tasksCompleted >= exitUnlockAt)
        {
            _exitOpened = true;

            if (exitGateLeft != null)
                exitGateLeft.UnlockDoor();

            if (exitGateRight != null)
                exitGateRight.UnlockDoor();

            if (exitGateTrigger != null)
                exitGateTrigger.Activate();

            if (completionUI != null)
                StartCoroutine(ShowCompletionUI());

            Debug.Log("Exit unlocked");
        }
    }

    void UpdateUI()
    {
        if (taskCounterText != null)
        {
            taskCounterText.text = $"TASKS COMPLETED - {_tasksCompleted}";
        }
    }

    IEnumerator ShowCompletionUI()
    {
        yield return new WaitForSeconds(4f);

        completionUI.SetActive(true);

        yield return new WaitForSeconds(5f);

        completionUI.SetActive(false);
    }
}