using UnityEngine;
using TMPro;

public class CollectableInfo : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI bloodVailsText;
    public TextMeshProUGUI umbrellasText;

    /// <summary>
    /// Updates the UI with the current amount of BloodVails and Umbrellas.
    /// </summary>
    public void UpdateValues(int bloodVails, int umbrellas)
    {
        if (bloodVailsText != null)
            bloodVailsText.text = bloodVails.ToString();
        else
            Debug.LogWarning("bloodVailsText is not assigned.");

        if (umbrellasText != null)
            umbrellasText.text = umbrellas.ToString();
        else
            Debug.LogWarning("umbrellasText is not assigned.");
    }
}
