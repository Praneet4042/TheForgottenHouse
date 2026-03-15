using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CupGamePuzzle : MonoBehaviour
{
    [Header("Asset to Glow")]
    public Outline assetOutline;
    [Header("Canvas & Overlay")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image dimOverlay;
    [SerializeField] private GameObject cupGamePanel;

    [Header("Cup RectTransforms (Left / Mid / Right)")]
    [SerializeField] private RectTransform cup_Left;
    [SerializeField] private RectTransform cup_Mid;
    [SerializeField] private RectTransform cup_Right;

    [Header("Cup Images (for visual state changes)")]
    [SerializeField] private Image cupImage_Left;
    [SerializeField] private Image cupImage_Mid;
    [SerializeField] private Image cupImage_Right;

    [Header("Cup Sprites")]
    [SerializeField] private Sprite cupDownSprite;
    [SerializeField] private Sprite cupUpSprite;

    [Header("Coin & Spider")]
    [SerializeField] private RectTransform coinImage;
    [SerializeField] private Image spiderFullscreen;
    [SerializeField] private Image screenFlash;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI instructionText;

    [Header("Level 1 — Easy")]
    [SerializeField] private float level1ShuffleSpeed = 0.6f;
    [SerializeField] private int level1ShuffleCount = 4;
    [SerializeField] private float level1Damage = 10f;

    [Header("Level 2 — Medium")]
    [SerializeField] private float level2ShuffleSpeed = 0.3f;
    [SerializeField] private int level2ShuffleCount = 6;
    [SerializeField] private float level2Damage = 25f;

    [Header("Level 3 — Impossible")]
    [SerializeField] private float level3ShuffleSpeed = 0.12f;
    [SerializeField] private int level3ShuffleCount = 10;
    [SerializeField] private float level3Damage = 50f;

    [Header("Cup Animation")]
    [SerializeField] private float cupLiftHeight = 120f;
    [SerializeField] private float cupArcHeight = 60f;
    [SerializeField] private float cupRevealHold = 0.8f;

    [Header("Perspective Scaling")]
    [SerializeField] private float backPositionScale = 0.82f;
    [SerializeField] private float backPositionYOffset = -60f;

    [Header("Panel Fade")]
    [SerializeField] private float fadeDuration = 0.4f;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip cupSlideClip;
    [SerializeField] private AudioClip cupLiftClip;
    [SerializeField] private AudioClip cupSlamClip;
    [SerializeField] private AudioClip spiderJumpscareClip;
    [SerializeField] private AudioClip winClip;

    [Header("Interact")]
    public float interactRange = 15f;

    [Header("Prompt + Glow")]
    public TextMeshPro worldPrompt;
    private Outline _outline;

    // private
    private int currentLevel = 1;
    private int coinCupIndex = 0;
    private bool isInteractable = false;
    private bool puzzleActive = false;
    private bool _completed = false;
    private bool _playerInRange = false;
    private Transform _player;

    private Vector2[] slotPositions = new Vector2[3];
    private int[] cupToSlot = new int[3];
    private RectTransform[] cups;
    private Image[] cupImages;

    private void Awake()
    {
        cups = new RectTransform[] { cup_Left, cup_Mid, cup_Right };
        cupImages = new Image[] { cupImage_Left, cupImage_Mid, cupImage_Right };

        for (int i = 0; i < 3; i++)
            slotPositions[i] = cups[i].anchoredPosition;

        cupToSlot = new int[] { 0, 1, 2 };

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        HideAllExtras();
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        // Make sure panel starts hidden
        if (cupGamePanel != null) cupGamePanel.SetActive(false);
        _outline = assetOutline;
        if (_outline != null) _outline.enabled = false;
        if (worldPrompt != null) worldPrompt.text = "";
    }

    private void Update()
    {
        if (worldPrompt != null && Camera.main != null)
            worldPrompt.transform.rotation = Camera.main.transform.rotation;

        if (_player == null || puzzleActive || _completed) return;

        float dist = Vector3.Distance(_player.position, transform.position);
        _playerInRange = dist <= interactRange;

        bool lanternOn = LanternToggle.instance != null &&
                         LanternToggle.instance.isOn;

        bool showPrompt = _playerInRange && lanternOn;
        if (_outline != null) _outline.enabled = showPrompt;
        if (worldPrompt != null)
            worldPrompt.text = showPrompt ? "[F] Play Cup Game" : "";

        if (_playerInRange && lanternOn && Input.GetKeyDown(KeyCode.F))
            OnInteract();
    }

    public void OnInteract()
    {
        if (puzzleActive || _completed) return;
        StartCoroutine(OpenPuzzle());
    }

    private IEnumerator OpenPuzzle()
    {
        puzzleActive = true;

        if (_outline != null) _outline.enabled = false;
        if (worldPrompt != null) worldPrompt.text = "";

        currentLevel = 1;

        MinigameManager.Instance.StartMinigame(cupGamePanel, true);

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        yield return StartCoroutine(RunLevel(currentLevel));
    }

    private IEnumerator ClosePuzzle(bool solved)
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeCanvas(1f, 0f));

        puzzleActive = false;

        // Unfreeze player via MinigameManager
        MinigameManager.Instance.EndMinigame(cupGamePanel);

        if (solved)
        {
            if (!_completed)
            {
                _completed = true;
                TaskManager.Instance?.TaskCompleted();
                Debug.Log("[CupGame] Solved! Task counted.");
            }
        }
    }

    private IEnumerator RunLevel(int level)
    {
        HideAllExtras();
        ResetCupPositions();

        levelText.text = level < 3 ?
            $"LEVEL {level}" : "LEVEL 3 — IMPOSSIBLE";
        instructionText.text = level < 3 ?
            "FIND THE COIN" : "CHOOSE WISELY...";

        bool hasCoin = level < 3;
        if (hasCoin)
        {
            coinCupIndex = Random.Range(0, 3);
            coinImage.anchoredPosition =
                cups[coinCupIndex].anchoredPosition + new Vector2(0, -20f);
            coinImage.gameObject.SetActive(false);
        }
        else
        {
            coinCupIndex = -1;
            coinImage.gameObject.SetActive(false);
        }

        if (level < 3)
        {
            coinImage.anchoredPosition =
                cups[coinCupIndex].anchoredPosition + new Vector2(0, -20f);
            coinImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.9f);
            coinImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }

        float speed = GetShuffleSpeed(level);
        int count = GetShuffleCount(level);
        yield return StartCoroutine(ShuffleCups(count, speed));

        instructionText.text = "PICK A CUP";
        isInteractable = true;

        yield return new WaitUntil(() => !isInteractable);
    }

    public void OnCupClicked_Left() { if (isInteractable) StartCoroutine(HandleCupChoice(0)); }
    public void OnCupClicked_Mid() { if (isInteractable) StartCoroutine(HandleCupChoice(1)); }
    public void OnCupClicked_Right() { if (isInteractable) StartCoroutine(HandleCupChoice(2)); }

    private IEnumerator HandleCupChoice(int cupIndex)
    {
        isInteractable = false;

        bool correct = (currentLevel < 3) && (cupToSlot[cupIndex] == coinCupIndex);

        if (currentLevel == 3)
        {
            yield return StartCoroutine(LiftCup(cupIndex, false, true));
            yield return StartCoroutine(TriggerJumpscare());
            yield return new WaitForSeconds(0.8f);
            // Level 3 always counts task — win or lose
            if (!_completed)
            {
                _completed = true;
                TaskManager.Instance?.TaskCompleted();
                Debug.Log("[CupGame] Level 3 done! Task counted.");
            }
            yield return StartCoroutine(ClosePuzzle(false));
        }
        else if (correct)
        {
            yield return StartCoroutine(LiftCup(cupIndex, true, false));
            PlaySFX(winClip);
            instructionText.text = currentLevel == 1 ?
                "YOU FOUND IT!" : "WELL DONE... FOR NOW";
            yield return new WaitForSeconds(1.2f);

            currentLevel++;

            if (currentLevel <= 3)
                yield return StartCoroutine(RunLevel(currentLevel));
            else
                yield return StartCoroutine(ClosePuzzle(true));
        }
        else
        {
            yield return StartCoroutine(LiftCup(cupIndex, false, true));
            yield return StartCoroutine(TriggerJumpscare());
            // Apply damage on wrong pick
            if (PlayerHealth.instance != null)
                PlayerHealth.instance.TakeDamage(10f);

            yield return new WaitForSeconds(0.6f);
            yield return StartCoroutine(SlamCupDown(cupIndex));
            yield return new WaitForSeconds(0.4f);
            yield return StartCoroutine(RunLevel(currentLevel));
        }
    }

    private IEnumerator ShuffleCups(int shuffleCount, float speed)
    {
        instructionText.text = "WATCH CAREFULLY...";

        for (int s = 0; s < shuffleCount; s++)
        {
            int a = Random.Range(0, 3);
            int b;
            do { b = Random.Range(0, 3); } while (b == a);

            if (sfxSource && cupSlideClip)
            {
                sfxSource.pitch = Mathf.Clamp(0.4f / speed, 0.8f, 2.5f);
                sfxSource.PlayOneShot(cupSlideClip);
                sfxSource.pitch = 1f;
            }
            yield return StartCoroutine(SwapCupsAnimated(a, b, speed));

            int tmp = cupToSlot[a];
            cupToSlot[a] = cupToSlot[b];
            cupToSlot[b] = tmp;

            yield return new WaitForSeconds(speed * 0.1f);
        }

        if (coinCupIndex >= 0)
            yield return StartCoroutine(AnimateCupsToHome(speed));
    }

    private IEnumerator AnimateCupsToHome(float speed)
    {
        Vector2[] startPositions = new Vector2[3];
        for (int i = 0; i < 3; i++)
            startPositions[i] = cups[i].anchoredPosition;

        if (sfxSource && cupSlideClip)
        {
            sfxSource.pitch = Mathf.Clamp(0.4f / speed, 0.8f, 2.5f);
            sfxSource.PlayOneShot(cupSlideClip);
            sfxSource.pitch = 1f;
        }

        float elapsed = 0f;
        while (elapsed < speed)
        {
            float t = elapsed / speed;
            float et = EaseInOut(t);

            for (int i = 0; i < 3; i++)
            {
                cups[i].anchoredPosition = Vector2.Lerp(
                    startPositions[i], slotPositions[i], et);
                ApplyPerspectiveScale(cups[i]);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < 3; i++)
        {
            cups[i].anchoredPosition = slotPositions[i];
            cups[i].localScale = Vector3.one;
            cupToSlot[i] = i;
        }

        coinImage.anchoredPosition =
            cups[coinCupIndex].anchoredPosition + new Vector2(0, -20f);
    }

    private IEnumerator SwapCupsAnimated(int a, int b, float speed)
    {
        Vector2 startA = cups[a].anchoredPosition;
        Vector2 startB = cups[b].anchoredPosition;

        float elapsed = 0f;
        while (elapsed < speed)
        {
            float t = elapsed / speed;
            float et = EaseInOut(t);
            float arc = Mathf.Sin(t * Mathf.PI) * cupArcHeight;

            cups[a].anchoredPosition =
                Vector2.Lerp(startA, startB, et) + new Vector2(0, arc);
            cups[b].anchoredPosition =
                Vector2.Lerp(startB, startA, et) + new Vector2(0, arc);

            ApplyPerspectiveScale(cups[a]);
            ApplyPerspectiveScale(cups[b]);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cups[a].anchoredPosition = startB;
        cups[b].anchoredPosition = startA;
        ApplyPerspectiveScale(cups[a]);
        ApplyPerspectiveScale(cups[b]);
    }

    private void ApplyPerspectiveScale(RectTransform cup)
    {
        float yNorm = Mathf.InverseLerp(
            slotPositions[0].y,
            slotPositions[0].y + backPositionYOffset,
            cup.anchoredPosition.y);
        float s = Mathf.Lerp(1f, backPositionScale, yNorm);
        cup.localScale = new Vector3(s, s, 1f);
    }

    private IEnumerator LiftCup(int cupIndex, bool hasCoin, bool hasSpider)
    {
        PlaySFX(cupLiftClip);
        if (cupUpSprite != null)
            cupImages[cupIndex].sprite = cupUpSprite;

        Vector2 origin = cups[cupIndex].anchoredPosition;
        Vector2 lifted = origin + new Vector2(0, cupLiftHeight);
        float elapsed = 0f;
        float liftDur = 0.25f;

        while (elapsed < liftDur)
        {
            cups[cupIndex].anchoredPosition = Vector2.Lerp(
                origin, lifted, EaseOut(elapsed / liftDur));
            elapsed += Time.deltaTime;
            yield return null;
        }
        cups[cupIndex].anchoredPosition = lifted;

        if (hasCoin)
        {
            coinImage.gameObject.SetActive(true);
            PositionCoinUnderCup(cupIndex);
        }

        yield return new WaitForSeconds(cupRevealHold);
    }

    private IEnumerator SlamCupDown(int cupIndex)
    {
        PlaySFX(cupSlamClip);

        Vector2 current = cups[cupIndex].anchoredPosition;
        Vector2 target = slotPositions[cupToSlot[cupIndex]];
        float elapsed = 0f;
        float dur = 0.18f;

        while (elapsed < dur)
        {
            cups[cupIndex].anchoredPosition = Vector2.Lerp(
                current, target, EaseIn(elapsed / dur));
            elapsed += Time.deltaTime;
            yield return null;
        }

        cups[cupIndex].anchoredPosition = target;
        if (cupDownSprite != null)
            cupImages[cupIndex].sprite = cupDownSprite;
        coinImage.gameObject.SetActive(false);
    }

    private IEnumerator TriggerJumpscare()
    {
        PlaySFX(spiderJumpscareClip);

        screenFlash.color = new Color(1f, 0f, 0f, 0f);
        screenFlash.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < 0.05f)
        {
            screenFlash.color = new Color(1f, 0f, 0f,
                Mathf.Lerp(0f, 0.85f, elapsed / 0.05f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        spiderFullscreen.gameObject.SetActive(true);
        spiderFullscreen.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
        elapsed = 0f;
        while (elapsed < 0.12f)
        {
            float s = Mathf.Lerp(1.4f, 1f, EaseOut(elapsed / 0.12f));
            spiderFullscreen.transform.localScale = new Vector3(s, s, 1f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.7f);

        elapsed = 0f;
        Color startFlash = screenFlash.color;
        Color startSpider = spiderFullscreen.color;
        while (elapsed < 0.35f)
        {
            float t = elapsed / 0.35f;
            screenFlash.color = Color.Lerp(
                startFlash, new Color(1f, 0f, 0f, 0f), t);
            spiderFullscreen.color = Color.Lerp(
                startSpider, new Color(1f, 1f, 1f, 0f), t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        screenFlash.gameObject.SetActive(false);
        spiderFullscreen.gameObject.SetActive(false);
        spiderFullscreen.color = Color.white;
    }

    private void HideAllExtras()
    {
        if (spiderFullscreen) spiderFullscreen.gameObject.SetActive(false);
        if (screenFlash) screenFlash.gameObject.SetActive(false);
        if (coinImage) coinImage.gameObject.SetActive(false);
    }

    private void ResetCupPositions()
    {
        for (int i = 0; i < 3; i++)
        {
            cups[i].anchoredPosition = slotPositions[i];
            cups[i].localScale = Vector3.one;
            if (cupDownSprite != null) cupImages[i].sprite = cupDownSprite;
            cupToSlot[i] = i;
        }
    }

    private void PositionCoinUnderCup(int cupIndex)
    {
        coinImage.anchoredPosition =
            cups[cupIndex].anchoredPosition + new Vector2(0, -20f);
    }

    private IEnumerator FadeCanvas(float from, float to)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;

        if (to > 0f)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;

        if (to <= 0f)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private float GetShuffleSpeed(int level) =>
        level switch { 1 => level1ShuffleSpeed, 2 => level2ShuffleSpeed, _ => level3ShuffleSpeed };

    private int GetShuffleCount(int level) =>
        level switch { 1 => level1ShuffleCount, 2 => level2ShuffleCount, _ => level3ShuffleCount };

    private float GetDamage(int level) =>
        level switch { 1 => level1Damage, 2 => level2Damage, _ => level3Damage };

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource && clip) sfxSource.PlayOneShot(clip);
    }

    private static float EaseInOut(float t) =>
        t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
    private static float EaseOut(float t) => 1 - (1 - t) * (1 - t);
    private static float EaseIn(float t) => t * t;
}