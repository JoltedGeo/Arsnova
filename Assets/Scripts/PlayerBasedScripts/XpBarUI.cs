using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XpBarUI : MonoBehaviour
{
    public Slider xpSlider; // Assign this in the Inspector
    private LevelingManager levelingManager;
    public Gradient gradient;
    public Image fill;

    void Start()
    {
        levelingManager = FindObjectOfType<LevelingManager>();
        levelingManager.onXpChanged += UpdateXpBar;

        UpdateXpBar();
    }

    void UpdateXpBar()
    {
        int maxXp = levelingManager.GetXpForNextLevel();

        xpSlider.maxValue = maxXp;
        xpSlider.value = levelingManager.currentXp;

        float normalizedValue = maxXp > 0 ? (float)xpSlider.value / maxXp : 0f;

        if (maxXp > 0)
            normalizedValue = (float)xpSlider.value / maxXp;

        fill.color = gradient.Evaluate(normalizedValue);
    }
}
