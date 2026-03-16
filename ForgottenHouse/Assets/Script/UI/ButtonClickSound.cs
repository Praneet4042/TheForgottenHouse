using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ButtonClickSound.cs
/// Attach to any Button to play click AND hover sounds.
/// Implements IPointerEnterHandler for mouse hover detection.
/// </summary>
public class ButtonClickSound : MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler
{
    [Header("── Audio Source ──")]
    public AudioSource audioSource;

    [Header("── Sounds ──")]
    public AudioClip clickSound;
    public AudioClip hoverSound;

    [Header("── Volume ──")]
    [Range(0f, 1f)] public float clickVolume = 1f;
    [Range(0f, 1f)] public float hoverVolume = 0.6f;

    void Start()
    {
        if (audioSource == null)
            audioSource = Camera.main.GetComponent<AudioSource>();

        if (audioSource == null)
            Debug.LogWarning("[ButtonSound] No AudioSource found!");
    }

    // Triggered when mouse ENTERS the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource && hoverSound)
            audioSource.PlayOneShot(hoverSound, hoverVolume);
    }

    // Triggered when mouse CLICKS the button
    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound, clickVolume);
    }
}