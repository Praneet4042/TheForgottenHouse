using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// PhoneRinging.cs
/// Place on the invisible trigger GameObject near the phone.
/// Plays ringing sound when player is in range, shows "Press C" prompt.
/// When C is pressed, hands off to PhoneCallManager.
/// </summary>
public class PhoneRinging : MonoBehaviour
{
    [Header("── Range ──")]
    public float ringRange      = 6f;   // Distance to start ringing
    public float interactRange  = 2.5f; // Distance to show "Press C"
    public Transform player;

    [Header("── Audio ──")]
    public AudioSource audioSource;
    public AudioClip   ringClip;        // Assign a phone ring sound (or leave null for beep)

    [Header("── UI ──")]
    public GameObject  promptUI;        // Panel with "Press C to answer"
    public TextMeshProUGUI promptText;

    [Header("── References ──")]
    public PhoneCallManager callManager;

    // ── Private ──
    private bool  isRinging    = false;
    private bool  callAnswered = false;
    private float beepTimer    = 0f;
    private float beepInterval = 2.2f;  // Ring every 2.2 seconds

    void Start()
    {
        if (promptUI) promptUI.SetActive(false);
    }

    void Update()
    {
        if (callAnswered) return;

        float dist = player
            ? Vector3.Distance(player.position, transform.position)
            : float.MaxValue;

        // Start / stop ringing based on range
        if (dist <= ringRange && !isRinging)
        {
            isRinging = true;
            StartCoroutine(RingLoop());
        }
        else if (dist > ringRange && isRinging)
        {
            isRinging = false;
            StopRinging();
        }

        // Show / hide "Press C" prompt
        if (promptUI)
            promptUI.SetActive(isRinging && dist <= interactRange);

        // Answer call
        if (isRinging && dist <= interactRange && Input.GetKeyDown(KeyCode.C))
        {
            AnswerCall();
        }
    }

    IEnumerator RingLoop()
    {
        while (isRinging)
        {
            PlayRingBeep();
            yield return new WaitForSeconds(beepInterval);
        }
    }

    void PlayRingBeep()
    {
        if (!audioSource) return;

        if (ringClip != null)
        {
            audioSource.PlayOneShot(ringClip);
        }
        else
        {
            // Procedural ring beep — no clip needed
            StartCoroutine(ProceduralRing());
        }
    }

    IEnumerator ProceduralRing()
    {
        // Two short beeps like a real phone ring
        yield return StartCoroutine(PlayTone(480f, 0.4f, 0.6f));
        yield return new WaitForSeconds(0.15f);
        yield return StartCoroutine(PlayTone(480f, 0.4f, 0.6f));
    }

    IEnumerator PlayTone(float frequency, float duration, float volume)
    {
        int sampleRate  = AudioSettings.outputSampleRate;
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t        = (float)i / sampleRate;
            float envelope = Mathf.Clamp01(Mathf.Min(t / 0.01f, (duration - t) / 0.01f));
            samples[i]     = Mathf.Sin(2f * Mathf.PI * frequency * t) * volume * envelope;
        }

        AudioClip clip = AudioClip.Create("RingTone", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(duration);
    }

    void StopRinging()
    {
        StopAllCoroutines();
        if (audioSource) audioSource.Stop();
        if (promptUI) promptUI.SetActive(false);
    }

    void AnswerCall()
    {
        callAnswered = true;
        StopRinging();
        if (callManager) callManager.StartCall();
        else Debug.LogError("[PhoneRinging] PhoneCallManager not assigned!");
    }

    // Call this from PhoneCallManager when minigame fully ends
    public void ResetPhone()
    {
        callAnswered = false;
        isRinging    = false;
    }
}
