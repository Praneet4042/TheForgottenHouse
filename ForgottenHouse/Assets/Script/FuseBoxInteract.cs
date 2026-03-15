using UnityEngine;
using TMPro;

public class FuseBoxInteract : MonoBehaviour
{
    [Header("Asset to Glow")]
    public Outline assetOutline;
    [Header("Settings")]
    public float interactRange = 10f;

    [Header("Prompt + Glow")]
    public TextMeshPro worldPrompt;
    private Outline _outline;

    private bool _inRange = false;
    private bool _completed = false;
    private Transform _player;
    private FuseBoxMinigame _minigame;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _minigame = GetComponentInParent<FuseBoxMinigame>();
        if (_minigame == null)
            _minigame = FindObjectOfType<FuseBoxMinigame>();

        _outline = assetOutline;
        if (_outline != null) _outline.enabled = false;

        if (worldPrompt != null) worldPrompt.text = "";
    }

    void Update()
    {
        if (_completed || _player == null) return;

        if (worldPrompt != null && Camera.main != null)
            worldPrompt.transform.rotation = Camera.main.transform.rotation;

        bool lanternOn = LanternToggle.instance != null &&
                         LanternToggle.instance.isOn;
        float dist = Vector3.Distance(_player.position, transform.position);
        _inRange = dist <= interactRange;

        bool showPrompt = _inRange && lanternOn;
        if (_outline != null) _outline.enabled = showPrompt;
        if (worldPrompt != null)
            worldPrompt.text = showPrompt ? "[F] Inspect Fuse Box" : "";

        if (_inRange && lanternOn && Input.GetKeyDown(KeyCode.F))
            if (_minigame != null)
                _minigame.StartMinigameFromInteract();
    }

    public void SetCompleted()
    {
        _completed = true;
        if (_outline != null) _outline.enabled = false;
        if (worldPrompt != null) worldPrompt.text = "";
    }
}