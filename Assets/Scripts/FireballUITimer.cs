using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireballUITimer : MonoBehaviour
{
    public Slider fireballTimerSlider; // Assign your UI Slider in the Inspector
    private Casting castingScript;
    private float currentTime;
    public Gradient gradient;
    public Image fill;
    private CanvasGroup canvasGroup; // Use this for fade/visibility

    void Start()
    {
        GameObject castingGameObject = GameObject.Find("RotatePoint");

        if (castingGameObject != null)
            castingScript = castingGameObject.GetComponent<Casting>();

        if (castingScript != null && fireballTimerSlider != null)
        {
            fireballTimerSlider.maxValue = castingScript.fireBallDelay;
            fireballTimerSlider.value = castingScript.fireBallDelay;
        }
        else
        {
            Debug.LogWarning("FireballUITimer: Missing reference — check castingScript or fireballTimerSlider!");
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Update()
    {
        if (castingScript == null || fireballTimerSlider == null)
            return;

        // Just READ the value — do not modify it
        fireballTimerSlider.maxValue = castingScript.fireBallDelay;
        fireballTimerSlider.value = castingScript.fireBallDelayTimer;

        fill.color = gradient.Evaluate(fireballTimerSlider.normalizedValue);

        // Hide when full, show when charging
        if (Mathf.Approximately(fireballTimerSlider.value, fireballTimerSlider.maxValue))
        {
            HideBar();
        }
        else
        {
            ShowBar();
        }
    }

    public void SetMaxFireValue(float fireBallDelay)
    {
        fireballTimerSlider.maxValue = fireBallDelay;
        fireballTimerSlider.value = fireBallDelay;

        fill.color = gradient.Evaluate(1f); //1f shows the gradient at 100%
    }

    public void SetFireValue(float fireBallDelayTimer)
    {
        fireballTimerSlider.value = fireBallDelayTimer;
        fill.color = gradient.Evaluate(fireballTimerSlider.normalizedValue);
    }

     private void HideBar()
    {
        // Instantly hide — or fade if you want
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    private void ShowBar()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
}
