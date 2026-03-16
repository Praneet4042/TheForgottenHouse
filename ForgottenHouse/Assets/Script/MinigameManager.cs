using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }
    public bool isMinigameActive = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartMinigame(GameObject panel, bool showCursor = true)
    {
        isMinigameActive = true;
        if (panel != null)
            panel.SetActive(true);

        Cursor.lockState = showCursor ?
            CursorLockMode.None : CursorLockMode.Locked;

        Cursor.visible = showCursor;

        // disable player controller
        var fpp = FindObjectOfType<HorrorFPPController>();
        if (fpp != null)
            fpp.enabled = false;

        // stop health drain
        if (PlayerHealth.instance != null)
            PlayerHealth.instance.SetInvincible(true);

        // stop ghost AI
        GhostAI ghost = FindObjectOfType<GhostAI>();
        if (ghost != null)
            ghost.SetActive(false);
    }

    public void EndMinigame(GameObject panel)
    {
        isMinigameActive = true;
        if (panel != null)
            panel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // enable player controller
        var fpp = FindObjectOfType<HorrorFPPController>();
        if (fpp != null)
            fpp.enabled = true;

        // resume health drain
        if (PlayerHealth.instance != null)
            PlayerHealth.instance.SetInvincible(false);

        // resume ghost AI
        GhostAI ghost = FindObjectOfType<GhostAI>();
        if (ghost != null)
            ghost.SetActive(true);
    }
}