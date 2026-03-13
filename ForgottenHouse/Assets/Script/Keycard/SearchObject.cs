using UnityEngine;
using TMPro;

public class SearchObject : MonoBehaviour
{
    [Header("Settings")]
    public string objectName = "Drawer";
    public float interactRange = 5f;

    [Header("UI")]
    public TextMeshPro worldPrompt;

    [Header("Search Messages")]
    public string[] emptyMessages = {
        "Nothing here...",
        "Just dust and cobwebs...",
        "Empty.",
        "Nothing useful here...",
        "Only darkness inside..."
    };

    // Internal
    private Transform _player;
    private Camera _camera;
    private bool _inRange;
    private bool _searched;
    private bool _hasKeycard;
    private bool _glowing;
    private Outline _outline;

    void Start()
    {
        
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _camera = Camera.main;
        // Setup outline
        _outline = GetComponent<Outline>();
        if (_outline == null)
            _outline = gameObject.AddComponent<Outline>();

        _outline.OutlineColor = new Color(1f, 0.7f, 0f); // golden
        _outline.OutlineWidth = 5f;
        _outline.enabled = false;

        // Hide prompt
        if (worldPrompt != null)
            worldPrompt.text = "";
    }

    public void SetHasKeycard(bool value)
    {
        _hasKeycard = value;
    }

    void Update()
    {
        if (worldPrompt != null && _camera != null)
            worldPrompt.transform.LookAt(
                worldPrompt.transform.position +
                _camera.transform.rotation * Vector3.forward,
                _camera.transform.rotation * Vector3.up
            );
        if (_searched || _player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        _inRange = dist <= interactRange;

        // Check lantern state
        bool lanternOn = LanternToggle.instance != null && LanternToggle.instance.isOn;

        if (_inRange && lanternOn)
        {
            SetGlow(true);
            ShowPrompt(true);

            if (Input.GetKeyDown(KeyCode.C))
                Search();
        }
        else
        {
            SetGlow(false);
            ShowPrompt(false);
        }
    }

    void Search()
    {
        _searched = true;
        SetGlow(false);

        Debug.Log(objectName + " searched. Has keycard: " + _hasKeycard);


        if (_hasKeycard)
        {
            ShowWorldMessage("You found the keycard!");
            TaskManager.Instance?.TaskCompleted();
            KeycardManager.Instance?.OnKeycardFound();
            Invoke(nameof(HideMessage), 3f);
        }
        else
        {
            ShowPrompt(false);
            string msg = emptyMessages[Random.Range(0, emptyMessages.Length)];
            ShowWorldMessage(msg);
            Invoke(nameof(HideMessage), 2f);
        }
    }

    void ShowPrompt(bool show)
    {
        if (worldPrompt == null) return;
        worldPrompt.text = show ? $"[C] Search {objectName}" : "";
    }

    void ShowWorldMessage(string msg)
    {
        if (worldPrompt != null)
            worldPrompt.text = msg;
    }

    void HideMessage()
    {
        if (worldPrompt != null)
            worldPrompt.text = "";
    }

    void SetGlow(bool glow)
    {
        if (_glowing == glow) return;
        _glowing = glow;
        if (_outline != null)
            _outline.enabled = glow;
    }

    public void DisableObject()
    {
        if (_hasKeycard) return;
        _searched = true;
        SetGlow(false);
        ShowPrompt(false);
        if (worldPrompt != null)
            worldPrompt.text = "";
    }
}