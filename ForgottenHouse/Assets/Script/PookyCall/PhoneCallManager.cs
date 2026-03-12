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
        ("...",                                              0.8f),
        ("*static crackle*",                                1.2f),
        ("...hello?",                                       1.5f),
        ("*soft crying*",                                   2.0f),
        ("They won't let me go...",                         2.5f),
        ("...but I can help you.",                          2.0f),
        ("Your code to escape is...",                       2.2f),
        ("1  4  _  _  1  4",                                3.5f),
        ("Remember... one, four... then the secret...",     2.5f),
        ("...then one, four again.",                        2.5f),
        ("*whispering* listen carefully for the secret...", 2.0f),
        ("*line crackles*",                                 1.0f),
    };

    // ── Hint subtitles for the secret number (77) ──
    private readonly (string text, float duration)[] hintSubtitles = new (string, float)[]
    {
        ("*crying intensifies*",                            1.5f),
        ("The secret... it's how many days...",             2.5f),
        ("...in eleven weeks.",                             2.2f),
        ("Count them... seven days...",                     2.0f),
        ("...times eleven weeks...",                        2.0f),
        ("*sobbing* seventy seven...",                      3.0f),
        ("7  7",                                            2.5f),
        ("Please... hurry...",                              2.0f),
        ("*line goes dead*",                                1.5f),
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
                ambienceAudioSource.clip = staticClip;
            else
                StartCoroutine(PlayStaticAmbience());
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
