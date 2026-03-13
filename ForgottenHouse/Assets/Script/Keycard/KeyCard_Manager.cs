using UnityEngine;

public class KeycardManager : MonoBehaviour
{
    public static KeycardManager Instance { get; private set; }

    [Header("Search Objects - drag all 6 here")]
    public SearchObject[] searchObjects;

    private int _keycardIndex;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Shuffle properly
        _keycardIndex = Random.Range(0, searchObjects.Length);

        // Reset all first
        foreach (var obj in searchObjects)
            obj.SetHasKeycard(false);

        // Then assign to random one
        searchObjects[_keycardIndex].SetHasKeycard(true);
        Debug.Log("Keycard hidden in: " + searchObjects[_keycardIndex].objectName);
    }

    public void OnKeycardFound()
    {
        // Disable all remaining search objects
        foreach (var obj in searchObjects)
            obj.DisableObject();

        Debug.Log("Keycard found! Task completed.");
    }
}