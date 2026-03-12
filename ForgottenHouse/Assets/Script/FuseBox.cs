using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuseBoxMinigame : MonoBehaviour
{
    [Header("UI")]
    public GameObject minigamePanel;
    public Button[] fuseButtons;
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI resultText;

    [Header("Settings")]
    public int sequenceLength = 4;
    public float showStepDelay = 0.6f;

    private Color[] _originalColors;
    private List<int> _sequence = new();
    private List<int> _playerInput = new();
    private bool _accepting;

    void Start()
    {
        _originalColors = new Color[fuseButtons.Length];
        for (int i = 0; i < fuseButtons.Length; i++)
            _originalColors[i] = fuseButtons[i].image.color;
    }

    public void OpenMinigame()
    {
        MinigameManager.Instance.StartMinigame(minigamePanel);
        StartCoroutine(RunMinigame());
    }

    IEnumerator RunMinigame()
    {
        _sequence.Clear();
        _playerInput.Clear();
        _accepting = false;
        resultText.text = "";
        promptText.text = "Watch the sequence...";
        SetButtonsInteractable(false);

        for (int i = 0; i < sequenceLength; i++)
            _sequence.Add(Random.Range(0, fuseButtons.Length));

        yield return new WaitForSeconds(0.8f);

        for (int i = 0; i < _sequence.Count; i++)
        {
            HighlightButton(_sequence[i], true);
            yield return new WaitForSeconds(showStepDelay);
            HighlightButton(_sequence[i], false);
            yield return new WaitForSeconds(0.2f);
        }

        promptText.text = "Repeat the sequence!";
        SetButtonsInteractable(true);
        _accepting = true;
    }

    public void OnFusePressed(int idx)
    {
        if (!_accepting) return;

        _playerInput.Add(idx);
        StartCoroutine(FlashButton(idx));

        int step = _playerInput.Count - 1;

        if (_playerInput[step] != _sequence[step])
        {
            StartCoroutine(OnFail());
            return;
        }

        if (_playerInput.Count == _sequence.Count)
            StartCoroutine(OnSuccess());
    }

    IEnumerator FlashButton(int idx)
    {
        HighlightButton(idx, true);
        yield return new WaitForSeconds(0.15f);
        HighlightButton(idx, false);
    }

    void HighlightButton(int idx, bool on)
    {
        fuseButtons[idx].image.color = on ? Color.white : _originalColors[idx];
    }

    void SetButtonsInteractable(bool value)
    {
        foreach (var b in fuseButtons) b.interactable = value;
    }

    IEnumerator OnFail()
    {
        _accepting = false;
        SetButtonsInteractable(false);
        resultText.text = "✗ Wrong! Try Again...";
        resultText.color = Color.red;
        yield return new WaitForSeconds(1.5f);
        resultText.text = "";
        StartCoroutine(RunMinigame());
    }

    IEnumerator OnSuccess()
    {
        _accepting = false;
        SetButtonsInteractable(false);
        resultText.text = "✓ Fuses aligned. Something stirs.";
        resultText.color = Color.green;
        yield return new WaitForSeconds(1.5f);
        MinigameManager.Instance.EndMinigame(minigamePanel);
    }
}