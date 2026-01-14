using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaFocusUITimer : MonoBehaviour
{
    public Slider focusTimerSlider; // Assign this in the Inspector
    private Casting castingScript;
    public Gradient gradient;
    public Image fill;

    void Start()
    {
        // Find your casting object (same as FireballUITimer)
        GameObject castingGameObject = GameObject.Find("RotatePoint");

        if (castingGameObject != null)
            castingScript = castingGameObject.GetComponent<Casting>();

        if (castingScript != null && focusTimerSlider != null)
        {
            focusTimerSlider.maxValue = castingScript.maxManaFocusTime;
            focusTimerSlider.value = castingScript.maxManaFocusTime;
        }
        else
        {
            Debug.LogWarning("ManaFocusUITimer: Missing reference — check castingScript or focusTimerSlider!");
        }
    }

    void Update()
    {
        if (castingScript == null || focusTimerSlider == null)
            return;

        // Update the slider to reflect how much "focus time" you have left
        focusTimerSlider.maxValue = castingScript.maxManaFocusTime;
        focusTimerSlider.value = castingScript.manaFocusTimer;

        fill.color = gradient.Evaluate(focusTimerSlider.normalizedValue);
    }

    public void SetMaxFocusValue(float maxFocusTime)
    {
        focusTimerSlider.maxValue = maxFocusTime;
        focusTimerSlider.value = maxFocusTime;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetFocusValue(float focusTime)
    {
        focusTimerSlider.value = focusTime;
        fill.color = gradient.Evaluate(focusTimerSlider.normalizedValue);
    }
}