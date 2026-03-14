using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BonfireManager : MonoBehaviour
{
    public static BonfireManager Instance { get; private set; }

    [Header("Settings")]
    public float interactRange = 15f;
    public int totalDolls = 8;
    public float healPerDoll = 5f;

    [Header("Dolls - drag all 8 here")]
    public HorrorDoll[] dolls;

    [Header("World Space Prompt")]
    public TextMeshPro worldPrompt;

    [Header("Screen UI")]
    public TextMeshProUGUI progressText; // "3/8 DOLLS DESTROYED"

    // Internal
    public bool taskStarted = false;
    public bool isCarryingDoll = false;
    private int _dollsDestroyed = 0;
    private bool _completed = false;
    private Transform _player;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        if (progressText != null) progressText.text = "";
        if (worldPrompt != null) worldPrompt.text = "";
    }

    void Update()
    {
        if (worldPrompt != null && Camera.main != null)
        {
            Vector3 dirToCamera = Camera.main.transform.position -
                                  worldPrompt.transform.position;
            worldPrompt.transform.rotation = Quaternion.LookRotation(-dirToCamera);
            Debug.Log("Rotating prompt: " + worldPrompt.transform.rotation.eulerAngles);
        }
        if (worldPrompt != null && Camera.main != null)
        {
            worldPrompt.transform.LookAt(Camera.main.transform.position);
            worldPrompt.transform.Rotate(0, 180f, 0);
        }

        
        if (_completed || _player == null) return;

        // Face camera
        // Face camera
        if (worldPrompt != null && Camera.main != null)
            worldPrompt.transform.LookAt(
                2 * worldPrompt.transform.position - Camera.main.transform.position);

        bool lanternOn = LanternToggle.instance != null &&
                         LanternToggle.instance.isOn;
        float dist = Vector3.Distance(
            _player.position, transform.position);
        bool inRange = dist <= interactRange;

        // Update world prompt
        if (worldPrompt != null)
        {
            if (!taskStarted && inRange && lanternOn)
                worldPrompt.text = "[K] Learn your task";
            else if (taskStarted && isCarryingDoll && inRange)
                worldPrompt.text = "[O] Throw doll into fire";
            else if (taskStarted && !isCarryingDoll && inRange)
                worldPrompt.text = "Find a doll and bring it here";
            else
                worldPrompt.text = "";
        }

        // K to start task
        if (!taskStarted && inRange && lanternOn &&
            Input.GetKeyDown(KeyCode.K))
            StartTask();

        // O to throw doll
        if (taskStarted && isCarryingDoll && inRange &&
            Input.GetKeyDown(KeyCode.O))
            ThrowDoll();
    }

    void StartTask()
    {
        taskStarted = true;
        if (progressText != null)
            progressText.text = $"0/{totalDolls} DOLLS DESTROYED";
        if (worldPrompt != null)
            worldPrompt.text = "Find a doll and bring it here";
        Debug.Log("[Bonfire] Task started! Find the dolls.");
    }

    public void OnDollPickedUp(HorrorDoll doll)
    {
        isCarryingDoll = true;
        Debug.Log("[Bonfire] Doll picked up! Return to bonfire.");
    }


    void ThrowDoll()
    {
        isCarryingDoll = false;
        _dollsDestroyed++;

        // Heal player
        if (PlayerHealth.instance != null)
            PlayerHealth.instance.Heal(healPerDoll);

        // Update progress
        if (progressText != null)
            progressText.text = $"{_dollsDestroyed}/{totalDolls} DOLLS DESTROYED";

        Debug.Log($"[Bonfire] {_dollsDestroyed}/{totalDolls} dolls destroyed!");

        // Check if all done
        if (_dollsDestroyed >= totalDolls)
        {
            _completed = true;
            TaskManager.Instance?.TaskCompleted();
            if (worldPrompt != null) worldPrompt.text = "It is done...";
            if (progressText != null)
                progressText.text = $"{totalDolls}/{totalDolls} DOLLS DESTROYED!";
            Debug.Log("[Bonfire] All dolls destroyed! Task counted.");
            StartCoroutine(HideCompletionText());
        }
    }
    IEnumerator HideCompletionText()
    {
        yield return new WaitForSeconds(3f);
        if (worldPrompt != null) worldPrompt.text = "";
        if (progressText != null) progressText.text = "";
    }
}