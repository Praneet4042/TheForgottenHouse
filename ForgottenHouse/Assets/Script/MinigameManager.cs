using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartMinigame(GameObject panel, bool showCursor = true)
    {
        if (panel != null) panel.SetActive(true);
        Cursor.lockState = showCursor ?
            CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = showCursor;

        // Freeze player movement
        var fpp = FindObjectOfType<HorrorFPPController>();
        if (fpp != null) fpp.enabled = false;

        // Stop health drain
        if (PlayerHealth.instance != null)
            PlayerHealth.instance.SetInvincible(true);
    }

    public void EndMinigame(GameObject panel)
    {
        if (panel != null) panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Unfreeze player
        var fpp = FindObjectOfType<HorrorFPPController>();
        if (fpp != null) fpp.enabled = true;

        // Resume health drain
        if (PlayerHealth.instance != null)
            PlayerHealth.instance.SetInvincible(false);
    }
}