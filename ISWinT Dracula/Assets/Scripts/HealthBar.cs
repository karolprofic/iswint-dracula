using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;

    private int maxHealth = 100;
    private float targetFillAmount;
    private float fillSpeed = 0.5f;

    private void Update()
    {
        AnimateHealthBar();
    }

    /// <summary>
    /// Sets the maximum health and initializes the bar.
    /// </summary>
    public void Initialize(int health)
    {
        maxHealth = health;
        SetHealthInstantly(health);
    }

    /// <summary>
    /// Updates the health bar to reflect current health with animation.
    /// </summary>
    public void UpdateHealth(int currentHealth)
    {
        float normalizedHealth = (float)currentHealth / maxHealth;
        targetFillAmount = Mathf.Clamp01(normalizedHealth);
    }

    /// <summary>
    /// Immediately sets the health bar fill amount without animation.
    /// </summary>
    public void SetHealthInstantly(int currentHealth)
    {
        float normalizedHealth = (float)currentHealth / maxHealth;
        fillImage.fillAmount = Mathf.Clamp01(normalizedHealth);
        targetFillAmount = fillImage.fillAmount;
    }

    /// <summary>
    /// Smoothly animates the health bar toward the target fill amount.
    /// </summary>
    private void AnimateHealthBar()
    {
        if (Mathf.Approximately(fillImage.fillAmount, targetFillAmount))
            return;

        fillImage.fillAmount = Mathf.MoveTowards(
            fillImage.fillAmount,
            targetFillAmount,
            fillSpeed * Time.deltaTime
        );
    }
}
