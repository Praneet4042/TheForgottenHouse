using UnityEngine;
using TMPro;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI taskCounterText;

    [Header("Settings")]
    public int totalTasksToUnlockDoor = 3;

    private int _completedTasks = 0;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void TaskCompleted()
    {
        _completedTasks++;
        UpdateUI();
        Debug.Log("Tasks completed: " + _completedTasks);
    }

    void UpdateUI()
    {
        if (taskCounterText != null)
            taskCounterText.text = "TASKS COMPLETED - " + _completedTasks;
    }
}