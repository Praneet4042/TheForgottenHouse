using UnityEngine;

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

        // Constant health drain
        currentHealth -= drainRate * Time.deltaTime;

        // Extra drain when ghost is nearby
        if (ghost != null)
        {
            float dist = Vector3.Distance(transform.position, ghost.position);

            // Kill player when lantern ON and ghost very close
            if (LanternToggle.instance.isOn && dist <= killRadius)
            {
                Die("The ghost got you...");
                return;
            }

            // Extra drain when ghost nearby regardless of lantern
            if (dist <= ghostProximityRadius)
            {
                float extra = Mathf.Lerp(ghostProximityDrainRate, 0f,
                    dist / ghostProximityRadius);
                currentHealth -= extra * Time.deltaTime;
            }
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0) Die("You ran out of time...");
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