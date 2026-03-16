using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// MainMenuBGM.cs
/// Plays background music with fade in on start.
/// Fades out when scene changes.
/// Attach to Main Camera in Title Scene.
/// </summary>
public class MainMenuBGM : MonoBehaviour
{
    [Header("── Audio ──")]
    public AudioSource audioSource;
    public AudioClip   bgmClip;

    [Header("── Fade Settings ──")]
    public float fadeInDuration  = 2f;   // Seconds to fade in
    public float fadeOutDuration = 1f;   // Seconds to fade out on scene change
    public float targetVolume    = 0.5f; // Max volume

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (bgmClip != null)
        {
            audioSource.clip   = bgmClip;
            audioSource.loop   = true;
            audioSource.volume = 0f;    // Start silent
            audioSource.Play();
        }

        // Fade in
        StartCoroutine(FadeIn());

        // Listen for scene changes
        SceneManager.sceneUnloaded += OnSceneUnload;
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed            += Time.deltaTime;
            audioSource.volume  = Mathf.Lerp(0f, targetVolume,
                                  elapsed / fadeInDuration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }

    IEnumerator FadeOut()
    {
        float elapsed      = 0f;
        float startVolume  = audioSource.volume;

        while (elapsed < fadeOutDuration)
        {
            elapsed            += Time.deltaTime;
            audioSource.volume  = Mathf.Lerp(startVolume, 0f,
                                  elapsed / fadeOutDuration);
            yield return null;
        }

        audioSource.Stop();
    }

    void OnSceneUnload(Scene scene)
    {
        StartCoroutine(FadeOut());
    }

    void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnload;
    }
}