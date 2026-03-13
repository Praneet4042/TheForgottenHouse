using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

// ═══════════════════════════════════════════════════════════════
//  OuijaBoardPuzzle_FINAL — Forgotten House
//
//  SETUP CHECKLIST:
//  1. Rename this class to OuijaBoardPuzzle (remove _FINAL)
//  2. Rename the file to match: OuijaBoardPuzzle.cs
//  3. Attach to OuijaBoardCanvas in your main scene
//  4. Wire all Inspector references
//  5. Search ⚠️ HOOK to find every external dependency
//     and verify method/variable names match your teammates' scripts
//
//  DECISION TREE:
//  ARE YOU ALONE
//  ├── YES → DID THEY LEAVE YOU
//  │         ├── YES → PRANK    (health → 1% then restores)
//  │         └── NO  → CURSE    (2x health reduction)
//  └── NO  → DID YOU INVITE US
//            ├── YES → SPARED   (no damage)
//            └── NO  → JUMPSCARE (screen flash + -10% health)
// ═══════════════════════════════════════════════════════════════

public class OuijaBoardPuzzle_TEST : MonoBehaviour, IInteractable
{
    // ─────────────────────────────────────────
    //  INSPECTOR REFERENCES
    // ─────────────────────────────────────────

    [Header("Panel References")]
    [Tooltip("OuijaBoardPanel GameObject")]
    public GameObject boardPanel;

    [Tooltip("DimOverlay Image")]
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
        "27    = Anchor_NO"
    )]
    public RectTransform[] letterAnchors;

    [Header("Choice Buttons")]
    public Button yesButton;
    public Button noButton;

    [Header("Audio")]
    public AudioClip planchetteMoveSFX;
    public AudioClip goodOutcomeSFX;
    public AudioClip badOutcomeSFX;

    [Header("Ghost Reference")]
    // ⚠️ HOOK: Drag your GhostAI GameObject here in Inspector
    public GameObject ghostObject;

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

    // ─────────────────────────────────────────
    //  IInteractable — player presses E on board
    // ─────────────────────────────────────────

    public void OnInteract()
    {
        if (!isOpen)
            StartCoroutine(OpenBoard());
    }

    // ─────────────────────────────────────────
    //  OPEN / CLOSE
    // ─────────────────────────────────────────

    IEnumerator OpenBoard()
    {
        isOpen = true;

        // ⚠️ HOOK: Prevents ghost from killing player during puzzle
        // Class: GameManager | Method: SetInvincible(bool)
        GameManager.instance.SetInvincible(true);

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

        // ⚠️ HOOK: Re-enables health drain and ghost chasing
        // Class: GameManager | Method: SetInvincible(bool)
        GameManager.instance.SetInvincible(false);
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
        yield return StartCoroutine(TypewriterSpell("ARE YOU ALONE"));
        yield return StartCoroutine(WaitForChoice());
        bool aloneYes = choiceResult;

        if (aloneYes)
        {
            yield return StartCoroutine(TypewriterSpell("DID THEY LEAVE YOU"));
            yield return StartCoroutine(WaitForChoice());

            if (choiceResult)
                yield return StartCoroutine(FatePrank());   // YES → prank
            else
                yield return StartCoroutine(FateCurse());   // NO  → curse
        }
        else
        {
            yield return StartCoroutine(TypewriterSpell("DID YOU INVITE US"));
            yield return StartCoroutine(WaitForChoice());

            if (choiceResult)
                yield return StartCoroutine(FateSpared());      // YES → spared
            else
                yield return StartCoroutine(FateJumpscare());   // NO  → jumpscare
        }
    }

    // ─────────────────────────────────────────
    //  FATES
    // ─────────────────────────────────────────

    // NOT ALONE + YES (invited) → SPARED
    IEnumerator FateSpared()
    {
        PlayAudio(goodOutcomeSFX);
        yield return StartCoroutine(TypewriterSpell("THE SPIRITS GRANT YOU PEACE"));

        // ⚠️ HOOK: Mark puzzle as solved
        // Class: GameManager | Method: OnPuzzleSolved(string)
        GameManager.instance.OnPuzzleSolved("OuijaBoard");

        yield return StartCoroutine(CloseBoard());
    }

    // NOT ALONE + NO (not invited) → JUMPSCARE + -10% health
    IEnumerator FateJumpscare()
    {
        PlayAudio(badOutcomeSFX);
        yield return StartCoroutine(ScreenFlash());

        // ⚠️ HOOK: Deal 10 damage to player
        // Class: PlayerHealth | Method: TakeDamage(float)
        // Variable: instance (singleton)
        // If your method is named differently e.g. DealDamage(), change it here
        PlayerHealth.instance.TakeDamage(10f);

        yield return StartCoroutine(CloseBoard(1f));
    }

    // ALONE + YES (they left) → PRANK (health to 1% then restores)
    IEnumerator FatePrank()
    {
        PlayAudio(badOutcomeSFX);
        yield return StartCoroutine(TypewriterSpell("WE LIKE YOU"));

        // ⚠️ HOOK: Read current health, drain to near zero, then restore
        // Class: PlayerHealth | Variable: currentHealth (float)
        // Class: PlayerHealth | Method: TakeDamage(float)
        // If your restore method is named differently e.g. HealDamage(), change it below
        float originalHealth = PlayerHealth.instance.currentHealth;
        float drainAmount = originalHealth - 1f;
        if (drainAmount > 0f)
            PlayerHealth.instance.TakeDamage(drainAmount);

        // Wait for dramatic effect then restore
        yield return new WaitForSeconds(2f);

        // ⚠️ HOOK: Restore health back to original
        // If PlayerHealth has a Heal(float) or SetHealth(float) method, use that instead
        // Currently using TakeDamage with negative value as a workaround —
        // replace the line below with your actual heal method
        PlayerHealth.instance.TakeDamage(-originalHealth + 1f); // negative = heal (only if your TakeDamage supports negative)
        // --- ALTERNATIVE if your script has a Heal method: ---
        // PlayerHealth.instance.Heal(originalHealth - 1f);
        // --- ALTERNATIVE if you set health directly: ---
        // PlayerHealth.instance.currentHealth = originalHealth;

        yield return StartCoroutine(CloseBoard());
    }

    // ALONE + NO (they didn't leave) → CURSE (2x health reduction)
    IEnumerator FateCurse()
    {
        PlayAudio(badOutcomeSFX);
        yield return StartCoroutine(TypewriterSpell("YOU SHOULD NOT HAVE COME"));

        // ⚠️ HOOK: Deal damage equal to half of current health (2x reduction = halve health)
        // Class: PlayerHealth | Variable: currentHealth (float)
        // Class: PlayerHealth | Method: TakeDamage(float)
        float halfHealth = PlayerHealth.instance.currentHealth * 0.5f;
        PlayerHealth.instance.TakeDamage(halfHealth);

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

        yield return new WaitForSeconds(0.6f);
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

    IEnumerator ScreenFlash()
    {
        // Flash white then fade back to dark overlay
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

    // ─────────────────────────────────────────
    //  TYPEWRITER FLICKER
    // ─────────────────────────────────────────

    IEnumerator FlickerLetter(char c)
    {
        int flickers = Random.Range(2, 4);
        for (int i = 0; i < flickers; i++)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
            if (dialogueText.text.Length > 0)
                dialogueText.text = dialogueText.text.Substring(0, dialogueText.text.Length - 1);
            yield return new WaitForSeconds(0.02f);
        }
        dialogueText.text += c;
        yield return new WaitForSeconds(0.06f);
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
        yesButton.onClick.AddListener(() => { choiceResult = true; awaitingChoice = false; });
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