using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGate : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 15f;

    [Header("Prompt")]
    public TextMeshPro worldPrompt;

    [Header("Outline")]
    public Outline outline;

    [Header("End Scene")]
    public string endSceneName = "EndScene";

    private Transform _player;
    private bool _active = false;
    private bool _exited = false;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        if (outline == null)
            outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.OutlineColor = new Color(1f, 0.8f, 0f);
            outline.OutlineWidth = 8f;
            outline.enabled = false;
        }

        if (worldPrompt != null) worldPrompt.text = "";
    }

    public void Activate()
    {
        _active = true;
        if (outline != null) outline.enabled = true;
    }

    void Update()
    {
        if (!_active || _exited || _player == null) return;

        if (worldPrompt != null && Camera.main != null)
            worldPrompt.transform.rotation = Camera.main.transform.rotation;

        float dist = Vector3.Distance(_player.position, transform.position);
        bool inRange = dist <= interactRange;

        if (worldPrompt != null)
            worldPrompt.text = inRange ? "[E] Exit the house" : "";

        if (inRange && Input.GetKeyDown(KeyCode.E))
            Exit();
    }

    public void Exit()
    {
        _exited = true;
        if (worldPrompt != null) worldPrompt.text = "";
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
    }
}