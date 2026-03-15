using UnityEngine;
using TMPro;

public class PillBox : MonoBehaviour
{
    [Header("Settings")]
    public float healPercent = 50f;
    public float interactRange = 10f;

    [Header("Prompt")]
    public TextMeshPro worldPrompt;

    private Transform _player;
    private bool _used = false;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        
    }

    public void Appear()
    {
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (_used || _player == null) return;

        if (worldPrompt != null && Camera.main != null)
            worldPrompt.transform.rotation = Camera.main.transform.rotation;

        float dist = Vector3.Distance(_player.position, transform.position);
        bool inRange = dist <= interactRange;

        if (worldPrompt != null)
            worldPrompt.text = inRange ? "[C] Take pill (+75% health)" : "";

        if (inRange && Input.GetKeyDown(KeyCode.C))
            UsePill();
    }

    void UsePill()
    {
        _used = true;
        if (worldPrompt != null) worldPrompt.text = "";

        if (PlayerHealth.instance != null)
        {
            float heal = PlayerHealth.instance.maxHealth * (healPercent / 100f);
            PlayerHealth.instance.Heal(heal);
            Debug.Log("[PillBox] Healed " + heal + " HP!");
        }

        gameObject.SetActive(false);
    }
}