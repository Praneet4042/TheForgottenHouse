using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public bool isMinigameActive = false;
    public static MinigameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartMinigame(GameObject panel, bool showCursor = true)
    {
        isMinigameActive = true;

        if (panel != null) panel.SetActive(true);
        Cursor.lockState = showCursor ?
            CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = showCursor;

        var fpp = FindObjectOfType<HorrorFPPController>();
        if (fpp != null) fpp.enabled = false;

        if (PlayerHealth.instance != null)
            PlayerHealth.instance.SetInvincible(true);

        GhostAI ghost = FindObjectOfType<GhostAI>();
        if (ghost != null)
            ghost.SetActive(false);
    }

    public void EndMinigame(GameObject panel)
    {
        isMinigameActive = false;

        if (panel != null) panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        var fpp = FindObjectOfType<HorrorFPPController>();
        if (fpp != null) fpp.enabled = true;

        if (PlayerHealth.instance != null)
            PlayerHealth.instance.SetInvincible(false);

        GhostAI ghost = FindObjectOfType<GhostAI>();
        if (ghost != null)
            ghost.SetActive(true);
    }
}