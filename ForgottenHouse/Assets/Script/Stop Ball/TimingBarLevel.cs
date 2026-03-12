using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "TimingBar/Level Data")]
public class TimingBarLevel : ScriptableObject
{
    [Header("Bar Settings")]
    public float barLength     = 600f;   // Total bar width in pixels

    [Header("Green Zone")]
    public float greenWidth    = 150f;   // Width of the green zone
    public float greenStartX   = 200f;   // Distance from left edge of bar

    [Header("Ball Settings")]
    public float ballSpeed     = 250f;   // Pixels per second
    public float ballSize      = 40f;    // Ball diameter in pixels
}