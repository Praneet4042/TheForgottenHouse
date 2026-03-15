using System.Collections;
using UnityEngine;
using TMPro;

public class PhoneRinging : MonoBehaviour
{
    [Header("Asset to Glow")]
    public Outline assetOutline;
    [Header("── Range ──")]
    public float ringRange = 15f;
    public float interactRange = 15f;

    [Header("── Audio ──")]
    public AudioSource audioSource;
    public AudioClip ringClip;

    [Header("── UI ──")]
    public GameObject promptUI;
    public TextMeshProUGUI promptText;

    [Header("── References ──")]
    public PhoneCallManager callManager;

    [Header("Prompt + Glow")]
    public TextMeshPro worldPrompt;
    private Outline _outline;

    // private
    private bool isRinging = false;
    private bool callAnswered = false;
    private float beepInterval = 2.2f;
    private Transform _player;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        if (promptUI != null) promptUI.SetActive(false);

        _outline = assetOutline;
        if (_outline != null) _outline.enabled = false;

        if (worldPrompt != null) worldPrompt.text = "";
    }

    void Update()
    {
        if (worldPrompt != null && Camera.main != null)
            worldPrompt.transform.rotation = Camera.main.transform.rotation;

        if (callAnswered) return;

        float dist = _player != null ?
            Vector3.Distance(_player.position, transform.position) :
            float.MaxValue;

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

        bool lanternOn = LanternToggle.instance != null &&
                         LanternToggle.instance.isOn;

        bool showPrompt = isRinging && dist <= interactRange && lanternOn;
        if (_outline != null) _outline.enabled = showPrompt;
        if (worldPrompt != null)
            worldPrompt.text = showPrompt ? "[F] Answer Phone" : "";

        if (promptUI) promptUI.SetActive(false);

        if (isRinging && dist <= interactRange && lanternOn &&
            Input.GetKeyDown(KeyCode.F))
            AnswerCall();
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
            audioSource.PlayOneShot(ringClip);
        else
            StartCoroutine(ProceduralRing());
    }

    IEnumerator ProceduralRing()
    {
        yield return StartCoroutine(PlayTone(480f, 0.4f, 0.6f));
        yield return new WaitForSeconds(0.15f);
        yield return StartCoroutine(PlayTone(480f, 0.4f, 0.6f));
    }

    IEnumerator PlayTone(float frequency, float duration, float volume)
    {
        int sampleRate = AudioSettings.outputSampleRate;
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = Mathf.Clamp01(
                Mathf.Min(t / 0.01f, (duration - t) / 0.01f));
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t)
                * volume * envelope;
        }
        AudioClip clip = AudioClip.Create(
            "RingTone", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(duration);
    }

    void StopRinging()
    {
        StopAllCoroutines();
        if (audioSource) audioSource.Stop();
        if (promptUI) promptUI.SetActive(false);
        if (_outline != null) _outline.enabled = false;
        if (worldPrompt != null) worldPrompt.text = "";
    }

    void AnswerCall()
    {
        callAnswered = true;
        StopRinging();
        if (callManager) callManager.StartCall();
    }

    public void ResetPhone()
    {
        callAnswered = false;
        isRinging = false;
    }
}