using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadLevelAfterTimeline : MonoBehaviour
{
    [Header("Level Settings")]
    public string nextLevelName = "Menu";
    public float nextLevelDelay = 1f;

    [Header("Optional Timeline")]
    public PlayableDirector timeline;

    void Awake()
    {
        if (timeline != null)
        {
            timeline.stopped += OnTimelineFinished;
            timeline.Play();
        }
        else
        {
            Debug.LogWarning("PlayableDirector not assigned!");
        }
    }

    private void OnTimelineFinished(PlayableDirector pd)
    {
        timeline.stopped -= OnTimelineFinished;
        StartCoroutine(LoadLevelAfterDelay());
    }

    private IEnumerator LoadLevelAfterDelay()
    {
        yield return new WaitForSeconds(nextLevelDelay);
        SceneManager.LoadScene(nextLevelName);
    }
}
