using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [Header("References")]
    public PlayerHealth playerHealth;
    public GhostAI ghostAI;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartMinigame(GameObject ui = null)
    {
        if (ui != null) ui.SetActive(true);

        if (playerHealth != null)
            playerHealth.isInvincible = true;

        if (ghostAI != null)
            ghostAI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Minigame Started");
    }

    public void EndMinigame(GameObject ui = null)
    {
        if (ui != null) ui.SetActive(false);

        if (playerHealth != null)
            playerHealth.isInvincible = false;

        if (ghostAI != null)
            ghostAI.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Minigame Ended");
    }
}