using UnityEngine;
public class LanternToggle : MonoBehaviour
{
    public static LanternToggle instance;
    public Light lanternLight;
    public bool isOn = false;

    void Awake()
    {
        instance = this;
        isOn = false;
        lanternLight.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            isOn = !isOn;
            lanternLight.enabled = isOn;
        }
    }
}