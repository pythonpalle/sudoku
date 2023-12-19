using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class EnumUtility
{
    public static T GetEnumValue<T>(int value) where T : Enum
    {
        return (T)Enum.ToObject(typeof(T), value);
    }
}

public class ProgressionIcon : MonoBehaviour
{
    [SerializeField] ExplanationText _explanationText;
    
    [SerializeField] Image circleFillSlider;
    [SerializeField] Transform pointerRotator;
    
    private Color startColor = Color.red;
    private Color completeColor = Color.green;
    private Color currentColor = Color.red;

    public void SetProgression(float progression)
    {
        int progressionPercentage = (int)(progression * 100);
        
        _explanationText.SetText($"Progression: {progressionPercentage}%");

        StartCoroutine(PlayDifficultyBarAnimation(progression));
    }

    private IEnumerator PlayDifficultyBarAnimation(float progression)
    {
        // slider setup
        float fillStart = circleFillSlider.fillAmount;
        float fillEnd = progression;
        
        // // color setup
        Color targetColor = Color.Lerp(startColor, completeColor, progression);
        
        // rotation setup
        float rotationDegrees = -180f * progression + 90;
        Quaternion rotStart = pointerRotator.rotation;
        Quaternion rotEnd = Quaternion.AngleAxis(rotationDegrees, Vector3.forward);
        
        float speed = 1f;

        float t = 0;
        while (t < 1)
        {
            float deltaTime = Time.deltaTime;

            Color lerpColor = Color.Lerp(currentColor, targetColor, t);
            circleFillSlider.color = lerpColor;

            circleFillSlider.fillAmount = Mathf.Lerp(fillStart, fillEnd, t);
            pointerRotator.rotation = Quaternion.Slerp(rotStart, rotEnd, t);

            t += deltaTime;
            yield return new WaitForEndOfFrame();
        }

        currentColor = targetColor;
        circleFillSlider.color = targetColor;
        
        circleFillSlider.fillAmount = fillEnd;
        pointerRotator.rotation = rotEnd;
    }
}
