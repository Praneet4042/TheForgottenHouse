using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    public FuseBoxMinigame minigame;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            minigame.OpenMinigame();
    }
}