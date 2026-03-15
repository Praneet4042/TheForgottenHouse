using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
 
public class CutsceneSceneManager : MonoBehaviour
{
    public PlayableDirector timeline;
 
    void Start()
    {
        // Auto play timeline when scene loads
        if (timeline)
        {
            timeline.Play();
            timeline.stopped += OnTimelineEnd;
        }
    }
 
    void OnTimelineEnd(PlayableDirector dir)
    {
        // Load the NEXT scene in Build Settings (current index + 1)
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
    }
}
 