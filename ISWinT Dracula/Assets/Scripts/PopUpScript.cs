using System.Collections;
using TMPro;
using UnityEngine;


public class PopUpScript : MonoBehaviour
{
    [Header("Pop Up Script Settings")]
    public TextMeshProUGUI messageText;
    public SpriteRenderer messageBackground;
    public float fadeDuration = 0.4f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (messageText == null || messageBackground == null)
        {
            Debug.LogError("PopUpScript: Message Text or Message Background reference is missing.");
        }

        // Hide after game initialization
        Color textColor = messageText.color;
        Color bgColor = messageBackground.color;
        messageText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        messageBackground.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0f);
    }

    /// <summary>
    /// Shows a message with fade in, stays visible for the duration, then fades out.
    /// If called again while active, it resets the timer and updates the text.
    /// </summary>
    public void ShowMessage(string message, float displayDuration)
    {
        bool skipPreviousMessage = false;
        if (currentRoutine != null)
        {
            skipPreviousMessage = true;
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(ShowMessageRoutine(message, displayDuration, skipPreviousMessage));
    }

    private IEnumerator ShowMessageRoutine(string message, float displayDuration, bool skipPreviousMessage)
    {
        if (skipPreviousMessage) {
            yield return new WaitForSeconds(fadeDuration);
        }
        messageText.text = message;
        yield return FadeCanvasGroup(0f, 1f, fadeDuration);
        yield return new WaitForSeconds(displayDuration);
        yield return FadeCanvasGroup(1f, 0f, fadeDuration);
        currentRoutine = null;
    }

    private IEnumerator FadeCanvasGroup(float from, float to, float duration)
    {
        float elapsed = 0f;

        Color startTextColor = messageText.color;
        Color startBackgroundColor = messageBackground.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);

            messageText.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, alpha);
            messageBackground.color = new Color(startBackgroundColor.r, startBackgroundColor.g, startBackgroundColor.b, alpha);

            yield return null;
        }

        messageText.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, to);
        messageBackground.color = new Color(startBackgroundColor.r, startBackgroundColor.g, startBackgroundColor.b, to);
    }

}
