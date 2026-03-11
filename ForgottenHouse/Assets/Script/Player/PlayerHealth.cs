using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float drainRate = 2f;
    public float ghostProximityRadius = 30f;
    public float ghostProximityDrainRate = 10f;
    public float killRadius = 5f;
    public bool isInvincible = false;
    public Slider healthBar;
    bool dead = false;
    Transform ghost;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ghost = GameObject.Find("Ghost").transform;
    }

    void Update()
    {
        if (dead || isInvincible) return;
        currentHealth -= drainRate * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar(); // ADD THIS LINE

        if (ghost != null && LanternToggle.instance.isOn)
        {
            float dist = Vector3.Distance(transform.position, ghost.position);
            if (dist <= killRadius)
            {
                Die("The ghost got you...");
                return;
            }
        }
        if (currentHealth <= 0) Die("You ran out of time...");
    }
    void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    public void TakeDamage(float amt)
    {
        if (dead || isInvincible) return;
        currentHealth -= amt;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0) Die("You ran out of time...");
    }

    public void Heal(float amt)
    {
        currentHealth = Mathf.Min(currentHealth + amt, maxHealth);
    }

    public void SetInvincible(bool val)
    {
        isInvincible = val;
    }

    void Die(string msg)
    {
        dead = true;
        Debug.Log("PLAYER DEAD: " + msg);
        GameOverManager.instance.ShowGameOver(msg);
    }
}