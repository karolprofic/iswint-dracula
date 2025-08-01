using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;

    private int maxHealth = 100;

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        SetHealth(health);
    }

    public void SetHealth(int currentHealth)
    {
        float fillAmount = (float)currentHealth / maxHealth;
        fillImage.fillAmount = Mathf.Clamp01(fillAmount);
    }
}
