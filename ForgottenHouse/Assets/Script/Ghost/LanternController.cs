using UnityEngine;

public class LanternController : MonoBehaviour
{
    public static LanternController instance;
    public Light lanternLight;
    public bool isOn = false;
    public KeyCode toggleKey = KeyCode.X;

    void Awake()
    {
        instance = this;
        isOn = false;
        lanternLight.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn;
            lanternLight.enabled = isOn;
        }
    }
}