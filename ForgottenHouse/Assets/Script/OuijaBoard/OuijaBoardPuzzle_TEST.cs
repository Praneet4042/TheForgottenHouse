using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OuijaBoardPuzzle_TEST : MonoBehaviour
{
    [Header("Asset to Glow")]
    public Outline assetOutline;

    [Header("Prank UI")]
    public GameObject prankPanel;
    [Header("Panel References")]
    public GameObject boardPanel;
    public Image dimOverlay;
    public TextMeshProUGUI dialogueText;

    [Header("Planchette")]
    public RectTransform planchette;
    public float planchetteSpeed = 3.5f;

    [Header("Letter Anchors")]
    public RectTransform[] letterAnchors;

    [Header("Choice Buttons")]
    public Button yesButton;
    public Button noButton;

    [Header("Audio")]
    public AudioClip planchetteMoveSFX;
    public AudioClip goodOutcomeSFX;
    public AudioClip badOutcomeSFX;

    [Header("Interact")]
    public float interactRange = 15f;

    [Header("Prompt + Glow")]
    public TextMeshPro worldPrompt;
    private Outline _outline;
    

    // private
    private AudioSource audioSource;
    private bool isOpen = false;
    private bool awaitingChoice = false;
    private bool choiceResult = false;
    private bool _completed = false;
    private bool _playerInRange = false;
    private Transform _player;
    private Dictionary<char, int> charToAnchor;

    void Awake()
    {
        BuildCharMap();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        if (boardPanel != null) boardPanel.SetActive(false);
        if (dimOverlay != null)
            dimOverlay.color = new Color(0f, 0f, 0f, 0f);
        _outline = GetComponentInChildren<Outline>();
        if (_outline == null)
            _outline = GetComponentInParent<Outline>();
        if (_outline != null) _outline.enabled = false;
        if (worldPrompt != null) worldPrompt.text = "";
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _outline = assetOutline;
        if (_outline != null) _outline.enabled = false;
    }

    void Update()
    {
        if (worldPrompt != null && Camera.main != null)
            worldPrompt.transform.rotation = Camera.main.transform.rotation;

        if (_player == null) return;

        if (!isOpen && !_completed)
        {
            float dist = Vector3.Distance(_player.position, transform.position);
            _playerInRange = dist <= interactRange;

            bool lanternOn = LanternToggle.instance != null &&
                             LanternToggle.instance.isOn;

            bool showPrompt = _playerInRange && lanternOn;
            if (_outline != null) _outline.enabled = showPrompt;
            if (worldPrompt != null)
                worldPrompt.text = showPrompt ? "[F] Use Ouija Board" : "";

            if (_playerInRange && lanternOn && Input.GetKeyDown(KeyCode.F))
                OnInteract();
        }
        else
        {
            if (_outline != null) _outline.enabled = false;
            if (worldPrompt != null) worldPrompt.text = "";
        }
    }

    public void OnInteract()
    {
        if (isOpen || _completed) return;
        MinigameManager.Instance.StartMinigame(boardPanel, true);
        StartCoroutine(OpenBoard());
    }

    IEnumerator OpenBoard()
    {
        isOpen = true;
        boardPanel.SetActive(true);
        dialogueText.text = "";
        yield return StartCoroutine(FadeDim(0f, 0.75f, 0.6f));
        yield return StartCoroutine(RunConversation());
    }

    IEnumerator CloseBoard(float delay = 1.5f)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(FadeDim(0.75f, 0f, 0.6f));
        boardPanel.SetActive(false);
        isOpen = false;
        MinigameManager.Instance.EndMinigame(boardPanel);
    }

    IEnumerator FadeDim(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            dimOverlay.color = new Color(0f, 0f, 0f,
                Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        dimOverlay.color = new Color(0f, 0f, 0f, to);
    }

    IEnumerator RunConversation()
    {
        yield return StartCoroutine(TypewriterSpell("ARE YOU ALONE"));
        yield return StartCoroutine(WaitForChoice());
        bool aloneYes = choiceResult;

        if (aloneYes)
        {
            yield return StartCoroutine(
                TypewriterSpell("DID THEY LEAVE YOU"));
            yield return StartCoroutine(WaitForChoice());
            if (choiceResult)
                yield return StartCoroutine(FatePrank());
            else
                yield return StartCoroutine(FateCurse());
        }
        else
        {
            yield return StartCoroutine(
                TypewriterSpell("DID YOU INVITE US"));
            yield return StartCoroutine(WaitForChoice());
            if (choiceResult)
                yield return StartCoroutine(FateSpared());
            else
                yield return StartCoroutine(FateJumpscare());
        }
    }

    IEnumerator FateSpared()
    {
        PlayAudio(goodOutcomeSFX);
        yield return StartCoroutine(
            TypewriterSpell("THE SPIRITS GRANT YOU PEACE"));
        PlayerHealth ph = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerHealth>();
        if (ph != null)
            ph.currentHealth = Mathf.Min(
                ph.currentHealth + 20f, ph.maxHealth);
        yield return StartCoroutine(FlashScreen(Color.green, 0.5f));
        CountTask();
        yield return StartCoroutine(CloseBoard());
    }

    IEnumerator FateJumpscare()
    {
        PlayAudio(badOutcomeSFX);
        yield return StartCoroutine(ScreenFlash());
        yield return StartCoroutine(FlashScreen(Color.red, 0.5f));
        PlayerHealth ph = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerHealth>();
        if (ph != null) ph.currentHealth -= 10f;
        CountTask();
        yield return StartCoroutine(CloseBoard(1f));
    }

    IEnumerator FatePrank()
    {
        PlayAudio(badOutcomeSFX);
        yield return StartCoroutine(TypewriterSpell("WE LIKE YOU"));

        PlayerHealth ph = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerHealth>();

        if (ph != null)
        {
            float savedHealth = ph.currentHealth;

            // Set invincible so real health stays safe
            ph.SetInvincible(true);

            // Show fake prank panel
            if (prankPanel != null) prankPanel.SetActive(true);

            // Red flash
            yield return StartCoroutine(FlashScreen(Color.red, 0.8f));

            // Wait so player panics
            yield return new WaitForSeconds(2.5f);

            // Hide prank panel
            if (prankPanel != null) prankPanel.SetActive(false);

            // Restore health
            ph.currentHealth = savedHealth;
            ph.SetInvincible(false);

            // Green flash - relief!
            yield return StartCoroutine(FlashScreen(Color.green, 0.5f));
        }

        CountTask();
        yield return StartCoroutine(CloseBoard());
    }

    IEnumerator FateCurse()
    {
        PlayAudio(badOutcomeSFX);
        yield return StartCoroutine(
            TypewriterSpell("YOU SHOULD NOT HAVE COME"));
        PlayerHealth ph = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.currentHealth *= 0.5f;
            yield return StartCoroutine(FlashScreen(Color.red, 1.2f));
        }
        CountTask();
        yield return StartCoroutine(PlanchetteFliesOff());
        yield return StartCoroutine(CloseBoard(2f));
    }

    void CountTask()
    {
        if (_completed) return;
        _completed = true;
        TaskManager.Instance?.TaskCompleted();
        Debug.Log("[OuijaBoard] Task counted.");
    }

    IEnumerator FlashScreen(Color flashColor, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (duration * 0.3f);
            dimOverlay.color = new Color(
                flashColor.r, flashColor.g, flashColor.b,
                Mathf.Lerp(0f, 0.6f, t));
            yield return null;
        }
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (duration * 0.7f);
            dimOverlay.color = new Color(
                flashColor.r, flashColor.g, flashColor.b,
                Mathf.Lerp(0.6f, 0f, t));
            yield return null;
        }
        dimOverlay.color = new Color(0f, 0f, 0f, 0f);
    }

    IEnumerator TypewriterSpell(string message)
    {
        dialogueText.text = "";
        foreach (char c in message)
        {
            if (c == ' ')
            {
                dialogueText.text += ' ';
                yield return new WaitForSeconds(0.15f);
                continue;
            }
            int idx = GetAnchorIndex(c);
            if (idx >= 0 && idx < letterAnchors.Length)
                yield return StartCoroutine(
                    GlidePlanchette(letterAnchors[idx].anchoredPosition));
            PlayAudio(planchetteMoveSFX);
            yield return StartCoroutine(FlickerLetter(c));
        }
        yield return new WaitForSeconds(1.2f);
        dialogueText.text = "";
    }

    IEnumerator GlidePlanchette(Vector2 targetPos)
    {
        Vector2 startPos = planchette.anchoredPosition;
        float distance = Vector2.Distance(startPos, targetPos);
        float duration = Mathf.Clamp(
            distance / (planchetteSpeed * 100f), 0.25f, 1.2f);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            planchette.anchoredPosition = Vector2.Lerp(
                startPos, targetPos, EaseInOut(t));
            float wobble = Mathf.Sin(t * Mathf.PI * 6f) * 4f * (1f - t);
            planchette.localRotation = Quaternion.Euler(0f, 0f, wobble);
            yield return null;
        }
        planchette.anchoredPosition = targetPos;
        yield return StartCoroutine(PlanchetteSettle());
    }

    IEnumerator PlanchetteSettle()
    {
        float duration = 0.25f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float shake = Mathf.Sin(t / duration * Mathf.PI * 8f)
                * 2f * (1f - t / duration);
            planchette.localRotation = Quaternion.Euler(0f, 0f, shake);
            yield return null;
        }
        planchette.localRotation = Quaternion.identity;
    }

    IEnumerator ScreenFlash()
    {
        dimOverlay.color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.1f);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.4f;
            dimOverlay.color = new Color(1f, 1f, 1f,
                Mathf.Lerp(1f, 0.75f, t));
            yield return null;
        }
        dimOverlay.color = new Color(0f, 0f, 0f, 0.75f);
    }

    IEnumerator PlanchetteFliesOff()
    {
        float duration = 0.5f;
        Vector2 start = planchette.anchoredPosition;
        Vector2 flyTarget = start + new Vector2(
            Random.Range(-600f, 600f),
            Random.Range(300f, 500f));
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            planchette.anchoredPosition = Vector2.Lerp(
                start, flyTarget, EaseIn(t));
            planchette.localRotation = Quaternion.Euler(0f, 0f, t * 720f);
            yield return null;
        }
    }

    IEnumerator FlickerLetter(char c)
    {
        int flickers = Random.Range(2, 4);
        for (int i = 0; i < flickers; i++)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f);
            if (dialogueText.text.Length > 0)
                dialogueText.text = dialogueText.text.Substring(
                    0, dialogueText.text.Length - 1);
            yield return new WaitForSeconds(0.04f);
        }
        dialogueText.text += c;
        yield return new WaitForSeconds(0.12f);
    }

    IEnumerator WaitForChoice()
    {
        awaitingChoice = true;
        choiceResult = false;
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => {
            choiceResult = true; awaitingChoice = false;
        });
        noButton.onClick.AddListener(() => {
            choiceResult = false; awaitingChoice = false;
        });
        while (awaitingChoice)
            yield return null;
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
    }

    void PlayAudio(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    float EaseInOut(float t)
    {
        t = Mathf.Clamp01(t);
        return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
    }

    float EaseIn(float t) => t * t * t;

    void BuildCharMap()
    {
        charToAnchor = new Dictionary<char, int>();
        for (int i = 0; i < 26; i++)
            charToAnchor[(char)('A' + i)] = i;
        charToAnchor['0'] = 28;
        for (int i = 1; i <= 9; i++)
            charToAnchor[(char)('0' + i)] = 28 + i;
    }

    int GetAnchorIndex(char c)
    {
        c = char.ToUpper(c);
        return charToAnchor.TryGetValue(c, out int idx) ? idx : -1;
    }
}