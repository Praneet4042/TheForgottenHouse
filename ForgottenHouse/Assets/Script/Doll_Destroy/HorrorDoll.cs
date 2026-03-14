using UnityEngine;
using TMPro;

public class HorrorDoll : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 10f;

    [Header("Prompt")]
    public TextMeshPro worldPrompt;

    private Outline _outline;
    private Transform _player;
    private bool _pickedUp = false;
    private bool _glowing = false;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        _outline = GetComponent<Outline>();
        if (_outline == null)
            _outline = gameObject.AddComponent<Outline>();

        _outline.OutlineColor = new Color(1f, 0.3f, 0f); // evil orange
        _outline.OutlineWidth = 6f;
        _outline.enabled = false;

        if (worldPrompt != null)
            worldPrompt.text = "";
    }

    void Update()
    {
        if (worldPrompt != null && Camera.main != null)
        {
            worldPrompt.transform.LookAt(Camera.main.transform.position);
            worldPrompt.transform.Rotate(0, 180f, 0);
        }

        
        if (_pickedUp || _player == null) return;

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

        // Only glow when lantern on and task started
        SetGlow(lanternOn && BonfireManager.Instance != null &&
                BonfireManager.Instance.taskStarted);

        // Show prompt
        if (worldPrompt != null)
            worldPrompt.text = inRange && lanternOn &&
                               !BonfireManager.Instance.isCarryingDoll &&
                               BonfireManager.Instance.taskStarted ?
                "[M] Pick up" : "";

        // Pick up
        if (inRange && lanternOn && Input.GetKeyDown(KeyCode.M) &&
            !BonfireManager.Instance.isCarryingDoll &&
            BonfireManager.Instance.taskStarted)
        {
            PickUp();
        }
    }

    void PickUp()
    {
        _pickedUp = true;
        SetGlow(false);
        if (worldPrompt != null) worldPrompt.text = "";
        gameObject.SetActive(false);
        BonfireManager.Instance.OnDollPickedUp(this);
    }

    void SetGlow(bool glow)
    {
        if (_glowing == glow) return;
        _glowing = glow;
        if (_outline != null) _outline.enabled = glow;
    }

}