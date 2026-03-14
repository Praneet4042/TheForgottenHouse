using UnityEngine;
using TMPro;

public class TextFlicker : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float minAlpha = 0.6f;
    public float maxAlpha = 1f;
    public float speed = 2f;

    void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * speed, 1));
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}