using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// PhoneCallManager.cs
/// Manages the full phone call sequence:
///   1. Freeze player
///   2. Play main call audio (crying child + code message)
///   3. Play hint audio (clue for the 77)
///   4. Unfreeze + open keypad UI
/// All audio is procedurally generated — no clips needed.
/// Assign real AudioClips in Inspector once you have them.
/// </summary>
public class PhoneCallManager : MonoBehaviour
{
    [Header("── Player ──")]
    public MonoBehaviour playerMovementScript;  // Your HorrorFPPController
    public GameObject    playerObject;

    [Header("── Audio Sources ──")]
    public AudioSource   voiceAudioSource;      // For call voices
    public AudioSource   ambienceAudioSource;   // For static / background

    [Header("── Real Audio Clips (assign when you have them) ──")]
    [Tooltip("Child voice: 'This is your code to escape: 1 4 _ _ 1 4'")]
    public AudioClip mainCallClip;

    [Tooltip("Child hint: clue for the two missing numbers (77)")]
    public AudioClip hintCallClip;

    [Tooltip("Phone static / ambience during call")]
    public AudioClip staticClip;

    [Header("── UI ──")]
    public GameObject      callOverlayUI;       // Dark overlay during call
    public TextMeshProUGUI subtitleText;         // Subtitles shown during call
    public GameObject      keypadUI;             // Opened after call ends

    [Header("── References ──")]
    public PhoneKeypadUI   keypadManager;
    public PhoneRinging    phoneRinging;

    // ── Private ──
    private bool callActive = false;

    // ── Subtitle script shown during main call ──
    private readonly (string text, float duration)[] mainCallSubtitles = new (string, float)[]
    {
        ("...hello?", 1.0f),
        ("please... please help me...", 2.8f),
        ("i've been here so long... it's so dark...", 2.5f),
        ("they said... they said if someone escapes... i can go home...", 4.9f),
        ("please... you have to remember this...", 3.2f),
        ("one... four...", 2.5f),
        ("[beep]", 2.8f),
        ("then one... four... again...", 4.5f),
        ("no... NO... i have to go...", 3.9f),
        ("remember... one four... [beep] one four...", 6f),
    };

    // ── Hint subtitles for the secret number (77) ──
    private readonly (string text, float duration)[] hintSubtitles = new (string, float)[]
        {
            ("are you still there...?", 1.3f),
            ("the secret... i scratched it into the wall to remember...", 3.7f),
            ("how many days... in eleven weeks...", 4.1f),
            ("seven... fourteen... twenty one...", 4f),
            ("keep going... seventy... seventy seven...", 4.6f),
            ("seventy... seven...", 2.5f),
            ("seven... seven...", 2.0f),
            (" please get ou...", 2.0f),
            (" ", 1.3f),
        };

    void Start()
    {
        if (callOverlayUI) callOverlayUI.SetActive(false);
        if (keypadUI)      keypadUI.SetActive(false);
        if (subtitleText)  subtitleText.text = "";
    }

    // ── Called by PhoneRinging when C is pressed ─────────────
    public void StartCall()
    {
        if (callActive) return;
        callActive = true;
        StartCoroutine(CallSequence());
    }

    // ── Called by PhoneKeypadUI when C is pressed after wrong code ──
    public void ReplayHint()
    {
        if (replayActive) return;
        StartCoroutine(ReplayHintSequence());
    }

    private bool replayActive = false;

    IEnumerator ReplayHintSequence()
    {
        replayActive = true;

        // Hide keypad while replaying
        if (keypadUI) keypadUI.SetActive(false);

        // Show call overlay + static
        if (callOverlayUI) callOverlayUI.SetActive(true);
        if (ambienceAudioSource && !ambienceAudioSource.isPlaying)
        {
            if (staticClip != null)
            {
                ambienceAudioSource.clip = staticClip;
                ambienceAudioSource.loop = true;
                ambienceAudioSource.Play();
            }
            else
                StartCoroutine(PlayStaticAmbience());
        }

        // Play full call subtitles first, then hint
        if (mainCallClip != null)
            voiceAudioSource.PlayOneShot(mainCallClip);
        else
            StartCoroutine(PlayProceduralVoice(isHint: false));

        yield return StartCoroutine(ShowSubtitles(mainCallSubtitles));

        // Then hint
        yield return StartCoroutine(PlayHintSequence());

        // Stop static, hide overlay
        if (ambienceAudioSource) ambienceAudioSource.Stop();
        if (callOverlayUI) callOverlayUI.SetActive(false);
        if (subtitleText)  subtitleText.text = "";

        // Restore keypad
        if (keypadUI) keypadUI.SetActive(true);

        replayActive = false;
    }

    // ═════════════════════════════════════════════════════════
    //  MAIN CALL SEQUENCE
    // ═════════════════════════════════════════════════════════
    IEnumerator CallSequence()
    {
        // 1. Freeze player
        FreezePlayer(true);

        // 2. Show call overlay
        if (callOverlayUI) callOverlayUI.SetActive(true);

        // 3. Play static ambience
        if (ambienceAudioSource)
        {
            if (staticClip != null)
            {
                ambienceAudioSource.clip = staticClip;
                ambienceAudioSource.loop = true;
                ambienceAudioSource.Play();
            }
            else
            {
                StartCoroutine(PlayStaticAmbience());
            }
        }

        // 4. Play main call audio + subtitles
        if (mainCallClip != null)
            voiceAudioSource.PlayOneShot(mainCallClip);
        else
            StartCoroutine(PlayProceduralVoice(isHint: false));

        yield return StartCoroutine(ShowSubtitles(mainCallSubtitles));

        // 5. Play hint audio + subtitles
        yield return StartCoroutine(PlayHintSequence());

        // 6. End call
        EndCall();
    }

    IEnumerator PlayHintSequence()
    {
        if (hintCallClip != null)
            voiceAudioSource.PlayOneShot(hintCallClip);
        else
            StartCoroutine(PlayProceduralVoice(isHint: true));

        yield return StartCoroutine(ShowSubtitles(hintSubtitles));
    }

    void EndCall()
    {
        callActive = false;

        // Stop ambience
        if (ambienceAudioSource) ambienceAudioSource.Stop();
        StopAllCoroutines();

        // Hide overlay, clear subtitle
        if (callOverlayUI) callOverlayUI.SetActive(false);
        if (subtitleText)  subtitleText.text = "";

        // Unfreeze player
        FreezePlayer(false);

        // Open keypad
        if (keypadUI)      keypadUI.SetActive(true);
        if (keypadManager) keypadManager.OpenKeypad();
    }

    // ═════════════════════════════════════════════════════════
    //  SUBTITLE DISPLAY
    // ═════════════════════════════════════════════════════════
    IEnumerator ShowSubtitles((string text, float duration)[] lines)
    {
        foreach (var line in lines)
        {
            if (subtitleText) subtitleText.text = line.text;
            yield return new WaitForSeconds(line.duration);
        }
        if (subtitleText) subtitleText.text = "";
    }

    // ═════════════════════════════════════════════════════════
    //  PLAYER FREEZE
    // ═════════════════════════════════════════════════════════
    void FreezePlayer(bool freeze)
    {
        if (playerMovementScript)
            playerMovementScript.enabled = !freeze;

        // Also lock cursor during call
        Cursor.lockState = freeze ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible   = freeze;
    }

    // ═════════════════════════════════════════════════════════
    //  PROCEDURAL AUDIO (placeholder until real clips added)
    // ═════════════════════════════════════════════════════════

    // Generates a haunting, wavering tone to simulate a child's voice
    IEnumerator PlayProceduralVoice(bool isHint)
    {
        int    sampleRate = AudioSettings.outputSampleRate;
        float  duration   = isHint ? 18f : 22f;
        int    samples    = Mathf.RoundToInt(sampleRate * duration);
        float[] data      = new float[samples];

        float baseFreq  = isHint ? 320f : 280f;  // Higher = child-like

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;

            // Waver to simulate emotional/crying voice
            float waver    = Mathf.Sin(2f * Mathf.PI * 5.5f * t) * 18f;
            float freq     = baseFreq + waver;

            // Tremolo (volume flutter = crying effect)
            float tremolo  = 0.6f + 0.4f * Mathf.Sin(2f * Mathf.PI * 7f * t);

            // Breathy noise layer
            float noise    = Random.Range(-0.08f, 0.08f);

            // Envelope: fade in/out at start and end
            float envelope = Mathf.Clamp01(Mathf.Min(t / 0.3f, (duration - t) / 0.5f));

            data[i] = (Mathf.Sin(2f * Mathf.PI * freq * t) * tremolo * 0.4f + noise)
                      * envelope;
        }

        AudioClip clip = AudioClip.Create("VoicePlaceholder", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        voiceAudioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(duration);
    }

    // Generates soft crackling static noise
    IEnumerator PlayStaticAmbience()
    {
        int   sampleRate = AudioSettings.outputSampleRate;
        float duration   = 60f; // Long loop
        int   samples    = Mathf.RoundToInt(sampleRate * duration);
        float[] data     = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t       = (float)i / sampleRate;
            float crackle = Random.Range(-1f, 1f) * 0.12f;
            float hum     = Mathf.Sin(2f * Mathf.PI * 60f * t) * 0.04f; // 60Hz hum
            data[i]       = crackle + hum;
        }

        AudioClip clip = AudioClip.Create("Static", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        ambienceAudioSource.clip = clip;
        ambienceAudioSource.loop = true;
        ambienceAudioSource.Play();
        yield return null;
    }
}
