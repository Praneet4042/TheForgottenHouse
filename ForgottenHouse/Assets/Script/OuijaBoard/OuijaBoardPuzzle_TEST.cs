using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────────────────────
//  OuijaBoardPuzzle — STANDALONE TEST VERSION
//
//  Zero external dependencies. No GameManager, no PlayerHealth,
//  no IInteractable, no GhostAI, no LanternSystem.
//
//  HOW TO TEST:
//  Press SPACE in Play Mode to open the board.
//  Click YES or NO when buttons appear.
//  Watch planchette glide + typewriter text.
//
//  When moving to main game, replace this file with
//  OuijaBoardPuzzle_FINAL.cs (which has all game hooks).
// ─────────────────────────────────────────────────────────────

public class OuijaBoardPuzzle_TEST : MonoBehaviour
{
    // ─────────────────────────────────────────
    //  INSPECTOR REFERENCES
    // ─────────────────────────────────────────

    [Header("Panel References")]
    [Tooltip("OuijaBoardPanel GameObject")]
    public GameObject boardPanel;

    [Tooltip("DimOverlay Image (black, alpha 0)")]
    public Image dimOverlay;

    [Tooltip("DialogueText — TMP text at top of board")]
    public TextMeshProUGUI dialogueText;

    [Header("Planchette")]
    [Tooltip("Planchette RectTransform")]
    public RectTransform planchette;

    [Tooltip("Lower = slower/eerier. Default 3.5")]
    public float planchetteSpeed = 3.5f;

    [Header("Letter Anchors")]
    [Tooltip(
        "Drag Anchor_ objects IN THIS ORDER:\n" +
        "0-25  = Anchor_A to Anchor_Z\n" +
        "26    = Anchor_YES\n" +
        "27    = Anchor_NO\n" +
        "28    = Anchor_0\n" +
        "29-37 = Anchor_1 to Anchor_9"
    )]
    public RectTransform[] letterAnchors;

    [Header("Choice Buttons")]
    public Button yesButton;
    public Button noButton;

    [Header("Audio")]
    [Tooltip("Short scrape sound per letter (optional)")]
    public AudioClip planchetteMoveSFX;

    [Tooltip("Good outcome sound (optional)")]
    public AudioClip goodOutcomeSFX;

    [Tooltip("Bad outcome sound (optional)")]
    public AudioClip badOutcomeSFX;

    // ─────────────────────────────────────────
    //  PRIVATE STATE
    // ─────────────────────────────────────────

    private AudioSource audioSource;
    private bool isOpen = false;
    private bool awaitingChoice = false;
    private bool choiceResult = false;
    private Dictionary<char, int> charToAnchor;

    // ─────────────────────────────────────────
    //  UNITY LIFECYCLE
    // ─────────────────────────────────────────

    void Awake()
    {
        BuildCharMap();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        boardPanel.SetActive(false);
        dimOverlay.color = new Color(0f, 0f, 0f, 0f);
    }

    void Update()
    {
        // Press SPACE to open board during testing
        if (Input.GetKeyDown(KeyCode.Space) && !isOpen)
            StartCoroutine(OpenBoard());
    }

    // ─────────────────────────────────────────
    //  OPEN / CLOSE
    // ─────────────────────────────────────────

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

        Debug.Log("[TEST] Board closed. Press SPACE to reopen.");
    }

    IEnumerator FadeDim(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            dimOverlay.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        dimOverlay.color = new Color(0f, 0f, 0f, to);
    }

    // ─────────────────────────────────────────
    //  CONVERSATION TREE
    // ─────────────────────────────────────────

    IEnumerator RunConversation()
    {
        // Question 1
        yield return StartCoroutine(TypewriterSpell("ARE YOU ALONE"));
        yield return StartCoroutine(WaitForChoice());
        bool aloneYes = choiceResult;

        if (aloneYes)
        {
            // Alone → DID THEY LEAVE YOU
            yield return StartCoroutine(TypewriterSpell("DID THEY LEAVE YOU"));
            yield return StartCoroutine(WaitForChoice());

            if (choiceResult)
                yield return StartCoroutine(FatePrank());      // YES → prank
            else
                yield return StartCoroutine(FateCurse());      // NO  → curse
        }
        else
        {
            // Not alone → DID YOU INVITE US
            yield return StartCoroutine(TypewriterSpell("DID YOU INVITE US"));
            yield return StartCoroutine(WaitForChoice());

            if (choiceResult)
                yield return StartCoroutine(FateSpared());     // YES → spared
            else
                yield return StartCoroutine(FateJumpscare());  // NO  → jumpscare + -10% health
        }
    }

    // ─────────────────────────────────────────
    //  FATES — visuals only, no game effects
    // ─────────────────────────────────────────

    // ALONE + NO (they didn't leave) → SPARED
    IEnumerator FateSpared()
    {
        PlayAudio(goodOutcomeSFX);
        Debug.Log("[TEST] FATE: SPARED");
        yield return StartCoroutine(TypewriterSpell("THE SPIRITS GRANT YOU PEACE"));
        Debug.Log("[TEST] Would call: good outcome — no damage");
        yield return StartCoroutine(CloseBoard());
    }

    // ALONE + YES (they left) → JUMPSCARE + -10% health
    IEnumerator FateJumpscare()
    {
        PlayAudio(badOutcomeSFX);
        Debug.Log("[TEST] FATE: JUMPSCARE");
        // Instant flash — no dialogue
        yield return StartCoroutine(ScreenFlash());
        Debug.Log("[TEST] Would call: PlayerHealth.TakeDamage(10)");
        yield return StartCoroutine(CloseBoard(1f));
    }

    // NOT ALONE + YES (invited) → PRANK
    IEnumerator FatePrank()
    {
        PlayAudio(badOutcomeSFX);
        Debug.Log("[TEST] FATE: PRANK");
        yield return StartCoroutine(TypewriterSpell("WE LIKE YOU"));
        Debug.Log("[TEST] Would call: health drops to 1% then restores");
        yield return StartCoroutine(CloseBoard());
    }

    // NOT ALONE + NO (not invited) → CURSE
    IEnumerator FateCurse()
    {
        PlayAudio(badOutcomeSFX);
        Debug.Log("[TEST] FATE: CURSE");
        yield return StartCoroutine(TypewriterSpell("YOU SHOULD NOT HAVE COME"));
        Debug.Log("[TEST] Would call: 2x health reduction");
        yield return StartCoroutine(PlanchetteFliesOff());
        yield return StartCoroutine(CloseBoard(2f));
    }

    // ─────────────────────────────────────────
    //  TYPEWRITER SPELL
    // ─────────────────────────────────────────

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
                yield return StartCoroutine(GlidePlanchette(letterAnchors[idx].anchoredPosition));

            PlayAudio(planchetteMoveSFX);
            yield return StartCoroutine(FlickerLetter(c));
        }

        yield return new WaitForSeconds(1.2f);
        dialogueText.text = "";
    }

    // ─────────────────────────────────────────
    //  PLANCHETTE ANIMATION
    // ─────────────────────────────────────────

    IEnumerator GlidePlanchette(Vector2 targetPos)
    {
        Vector2 startPos = planchette.anchoredPosition;
        float distance = Vector2.Distance(startPos, targetPos);
        float duration = Mathf.Clamp(distance / (planchetteSpeed * 100f), 0.25f, 1.2f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            planchette.anchoredPosition = Vector2.Lerp(startPos, targetPos, EaseInOut(t));
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
            float shake = Mathf.Sin(t / duration * Mathf.PI * 8f) * 2f * (1f - t / duration);
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
            dimOverlay.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0.75f, t));
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
            Random.Range(300f, 500f)
        );

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            planchette.anchoredPosition = Vector2.Lerp(start, flyTarget, EaseIn(t));
            planchette.localRotation = Quaternion.Euler(0f, 0f, t * 720f);
            yield return null;
        }
    }

    // ─────────────────────────────────────────
    //  TYPEWRITER FLICKER
    // ─────────────────────────────────────────

    IEnumerator FlickerLetter(char c)
    {
        int flickers = Random.Range(2, 4);
        for (int i = 0; i < flickers; i++)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f);
            if (dialogueText.text.Length > 0)
                dialogueText.text = dialogueText.text.Substring(0, dialogueText.text.Length - 1);
            yield return new WaitForSeconds(0.04f);
        }
        dialogueText.text += c;
        yield return new WaitForSeconds(0.12f);
    }

    // ─────────────────────────────────────────
    //  PLAYER CHOICE
    // ─────────────────────────────────────────

    IEnumerator WaitForChoice()
    {
        awaitingChoice = true;
        choiceResult = false;

        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => { choiceResult = true;  awaitingChoice = false; });
        noButton.onClick.AddListener(() => { choiceResult = false; awaitingChoice = false; });

        while (awaitingChoice)
            yield return null;

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.3f);
    }

    // ─────────────────────────────────────────
    //  AUDIO
    // ─────────────────────────────────────────

    void PlayAudio(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    // ─────────────────────────────────────────
    //  EASING
    // ─────────────────────────────────────────

    float EaseInOut(float t)
    {
        t = Mathf.Clamp01(t);
        return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
    }

    float EaseIn(float t) => t * t * t;

    // ─────────────────────────────────────────
    //  CHAR TO ANCHOR INDEX
    // ─────────────────────────────────────────

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
