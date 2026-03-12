using UnityEngine;
using TMPro;

public class FuseBoxInteract : MonoBehaviour
{
    public FuseBoxMinigame minigame;
    public TextMeshProUGUI interactPrompt;
    private bool _inRange;
    private bool _completed; // ADD THIS

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _inRange = true;
            if (interactPrompt != null)
                interactPrompt.text = _completed ? "" : "[E] Inspect Fuse Box";
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _inRange = false;
            if (interactPrompt != null)
                interactPrompt.text = "";
        }
    }

    void Update()
    {
        if (_inRange && !_completed && Input.GetKeyDown(KeyCode.E))
            minigame.OpenMinigame();
    }

    public void SetCompleted()
    {
        _completed = true;
        if (interactPrompt != null)
            interactPrompt.text = "";
    }
}