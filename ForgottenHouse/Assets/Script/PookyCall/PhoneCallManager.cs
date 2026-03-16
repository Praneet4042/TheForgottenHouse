using System.Collections;
using UnityEngine;
using TMPro;

public class PhoneCallManager : MonoBehaviour
{
    [Header("── Audio Sources ──")]
    public AudioSource voiceAudioSource;
    public AudioSource ambienceAudioSource;

    [Header("── Audio Clips ──")]
    public AudioClip mainCallClip;
    public AudioClip hintCallClip;
    public AudioClip staticClip;

    [Header("── UI ──")]
    public GameObject callOverlayUI;
    public TextMeshProUGUI subtitleText;
    public GameObject keypadUI;

    [Header("── References ──")]
    public PhoneKeypadUI keypadManager;
    public PhoneRinging phoneRinging;

    private bool callActive = false;
    private bool replayActive = false;

    private readonly (string text, float duration)[] mainCallSubtitles =
    {
        ("...hello?", 1.0f),
        ("please... please help me...", 2.8f),
        ("i've been here so long... it's so dark...", 2.5f),
        ("they said... if someone escapes... i can go home...", 4.9f),
        ("please... you have to remember this...", 3.2f),
        ("one... four...", 2.5f),
        ("[beep]", 2.8f),
        ("then one... four... again...", 4.5f),
        ("no... NO... i have to go...", 3.9f),
        ("remember... one four... [beep] one four...", 6f),
    };

    private readonly (string text, float duration)[] hintSubtitles =
    {
        ("are you still there...?", 1.3f),
        ("the secret... i scratched it into the wall...", 3.7f),
        ("how many days... in eleven weeks...", 4.1f),
        ("seven... fourteen... twenty one...", 4f),
        ("keep going... seventy... seventy seven...", 4.6f),
        ("seventy... seven...", 2.5f),
        ("seven... seven...", 2.0f),
        ("please get ou...", 2.0f),
        (" ", 1.3f),
    };

    void Start()
    {
        if (callOverlayUI) callOverlayUI.SetActive(false);
        if (keypadUI) keypadUI.SetActive(false);
        if (subtitleText) subtitleText.text = "";
    }

    public void StartCall()
    {
        if (callActive) return;
        callActive = true;
        StartCoroutine(CallSequence());
    }

    public void ReplayHint()
    {
        if (replayActive) return;
        StartCoroutine(ReplayHintSequence());
    }

    IEnumerator ReplayHintSequence()
    {
        replayActive = true;
        if (keypadUI) keypadUI.SetActive(false);
        if (callOverlayUI) callOverlayUI.SetActive(true);

        if (ambienceAudioSource && !ambienceAudioSource.isPlaying)
        {
            if (staticClip != null)
            {
                ambienceAudioSource.clip = staticClip;
                ambienceAudioSource.loop = true;
                ambienceAudioSource.Play();
            }
            else StartCoroutine(PlayStaticAmbience());
        }

        if (mainCallClip != null)
            voiceAudioSource.PlayOneShot(mainCallClip);
        else StartCoroutine(PlayProceduralVoice(false));

        yield return StartCoroutine(ShowSubtitles(mainCallSubtitles));
        yield return StartCoroutine(PlayHintSequence());

        if (ambienceAudioSource) ambienceAudioSource.Stop();
        if (callOverlayUI) callOverlayUI.SetActive(false);
        if (subtitleText) subtitleText.text = "";
        if (keypadUI) keypadUI.SetActive(true);

        replayActive = false;
    }

    IEnumerator CallSequence()
    {
        // Use MinigameManager to freeze player
        MinigameManager.Instance.StartMinigame(callOverlayUI, false);

        if (callOverlayUI) callOverlayUI.SetActive(true);

        if (ambienceAudioSource)
        {
            if (staticClip != null)
            {
                ambienceAudioSource.clip = staticClip;
                ambienceAudioSource.loop = true;
                ambienceAudioSource.Play();
            }
            else StartCoroutine(PlayStaticAmbience());
        }

        if (mainCallClip != null)
            voiceAudioSource.PlayOneShot(mainCallClip);
        else StartCoroutine(PlayProceduralVoice(false));

        yield return StartCoroutine(ShowSubtitles(mainCallSubtitles));
        yield return StartCoroutine(PlayHintSequence());

        EndCall();
    }

    IEnumerator PlayHintSequence()
    {
        if (hintCallClip != null)
            voiceAudioSource.PlayOneShot(hintCallClip);
        else StartCoroutine(PlayProceduralVoice(true));

        yield return StartCoroutine(ShowSubtitles(hintSubtitles));
    }

    void EndCall()
    {
        callActive = false;
        if (ambienceAudioSource) ambienceAudioSource.Stop();

        if (callOverlayUI) callOverlayUI.SetActive(false);
        if (subtitleText) subtitleText.text = "";

        // Unfreeze player via MinigameManager
        MinigameManager.Instance.StartMinigame(keypadUI, true);

        
        if (keypadManager) keypadManager.OpenKeypad();
    }

    IEnumerator ShowSubtitles((string text, float duration)[] lines)
    {
        foreach (var line in lines)
        {
            if (subtitleText) subtitleText.text = line.text;
            yield return new WaitForSeconds(line.duration);
        }
        if (subtitleText) subtitleText.text = "";
    }

    IEnumerator PlayProceduralVoice(bool isHint)
    {
        int sampleRate = AudioSettings.outputSampleRate;
        float duration = isHint ? 18f : 22f;
        int samples = Mathf.RoundToInt(sampleRate * duration);
        float[] data = new float[samples];
        float baseFreq = isHint ? 320f : 280f;

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float waver = Mathf.Sin(2f * Mathf.PI * 5.5f * t) * 18f;
            float freq = baseFreq + waver;
            float tremolo = 0.6f + 0.4f * Mathf.Sin(2f * Mathf.PI * 7f * t);
            float noise = Random.Range(-0.08f, 0.08f);
            float envelope = Mathf.Clamp01(
                Mathf.Min(t / 0.3f, (duration - t) / 0.5f));
            data[i] = (Mathf.Sin(2f * Mathf.PI * freq * t)
                * tremolo * 0.4f + noise) * envelope;
        }

        AudioClip clip = AudioClip.Create(
            "VoicePlaceholder", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        voiceAudioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(duration);
    }

    IEnumerator PlayStaticAmbience()
    {
        int sampleRate = AudioSettings.outputSampleRate;
        float duration = 60f;
        int samples = Mathf.RoundToInt(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float crackle = Random.Range(-1f, 1f) * 0.12f;
            float hum = Mathf.Sin(2f * Mathf.PI * 60f * t) * 0.04f;
            data[i] = crackle + hum;
        }

        AudioClip clip = AudioClip.Create(
            "Static", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        ambienceAudioSource.clip = clip;
        ambienceAudioSource.loop = true;
        ambienceAudioSource.Play();
        yield return null;
    }
}