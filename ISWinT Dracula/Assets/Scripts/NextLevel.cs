using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class NextLevel : MonoBehaviour
{
    [Header("Level Settings")]
    public string nextLevelName;

    [Header("Optional Timeline")]
    public PlayableDirector timeline;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;

            if (timeline != null)
            {
                timeline.stopped += OnTimelineFinished;
                timeline.Play();
            }
            else
            {
                LoadNextLevel();
            }
        }
    }

    private void OnTimelineFinished(PlayableDirector pd)
    {
        timeline.stopped -= OnTimelineFinished;
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogWarning("Next level name is not set.");
        }
    }
}
