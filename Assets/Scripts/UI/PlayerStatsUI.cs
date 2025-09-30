using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private float scrollSpeed;
    [SerializeField] private Sprite[] healthStatus; // 0 = full, 1 = mid, 2 = low
    [SerializeField] private Image statusImage;     // Drag & drop directly in Inspector
    [SerializeField] private Image colorBarImage;
    [SerializeField] private Slider sanitySlider;

    private Sprite h_Status;

    void Start()
    {
        statusImage.sprite = healthStatus[0];
        colorBarImage.color = new Color32(0, 255, 0, 255);
        h_Status = healthStatus[0];
    }

    void Update()
    {
        // scroll texture (using unscaled time for UI independent of Time.timeScale)
        colorBarImage.material.mainTextureOffset += new Vector2((-scrollSpeed / 10f) * Time.unscaledDeltaTime, 0f);

        UpdateHealthUI(PlayerStats.Instance.currentHealth, PlayerStats.Instance.maxHealth);
        UpdateSanityUI(PlayerStats.Instance.currentSanity, PlayerStats.Instance.maxSanity);
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        float healthPercent = currentHealth / maxHealth;

        if (healthPercent < 0.25f)
        {
            colorBarImage.color = new Color32(255, 0, 0, 255);
            h_Status = healthStatus[2];
        }
        else if (healthPercent < 0.5f)
        {
            colorBarImage.color = new Color32(255, 255, 0, 255);
            h_Status = healthStatus[1];
        }
        else
        {
            colorBarImage.color = new Color32(0, 255, 0, 255);
            h_Status = healthStatus[0];
        }

        if (statusImage != null && statusImage.sprite != h_Status)
        {
            statusImage.sprite = h_Status;
        }
    }

    private void UpdateSanityUI(float currentSanity, float maxSanity)
    {
        sanitySlider.maxValue = maxSanity;
        sanitySlider.value = currentSanity;
    }
}
