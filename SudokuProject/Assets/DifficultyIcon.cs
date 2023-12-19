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

public class DifficultyIcon : MonoBehaviour
{
    [SerializeField] TileColors difficultyColors;
    [SerializeField] ExplanationText _explanationText;
    
    [SerializeField] Image circleFillSlider;
    [SerializeField] Image circleFillGap;
    [SerializeField] Transform pointerRotator;

    private Color currentColor = Color.white;
    
    public void SetDifficulty(int difficultyInt)
    {
        var difficulty = EnumUtility.GetEnumValue<PuzzleDifficulty>(difficultyInt);
        SetDifficulty(difficulty);
    }
    
    public void SetDifficulty(PuzzleDifficulty difficulty)
    {
        string difficultyAsText = difficulty.ToString() ;
        _explanationText.SetText(difficultyAsText);

        StartCoroutine(PlayDifficultyBarAnimation(difficulty));
    }

    private IEnumerator PlayDifficultyBarAnimation(PuzzleDifficulty difficulty)
    {
        // ratio setup
        int maxDifficulty = (int) PuzzleDifficulty.Extreme;
        int currentDifficulty = (int) difficulty;
        float ratioToMax = (float) currentDifficulty / maxDifficulty;
        
        // slider setup
        float fillStart = circleFillSlider.fillAmount;
        float fillEnd = ratioToMax * 0.5f;
        
        // color setup
        Color colorStart = currentColor;
        Color colorEnd = difficultyColors.Colors[currentDifficulty];
        
        // rotation setup
        float rotationDegrees = -180f * ratioToMax + 90;
        Quaternion rotStart = pointerRotator.rotation;
        Quaternion rotEnd = Quaternion.AngleAxis(rotationDegrees, Vector3.forward);
        
        float speed = 1f;

        float t = 0;
        while (t < 1)
        {
            float deltaTime = Time.deltaTime;

            Color lerpColor = Color.Lerp(colorStart, colorEnd, t);
            circleFillSlider.color = lerpColor;
            circleFillGap.color = circleFillSlider.color;
            //pointerImage.color = lerpColor;

            circleFillSlider.fillAmount = Mathf.Lerp(fillStart, fillEnd, t);
            pointerRotator.rotation = Quaternion.Slerp(rotStart, rotEnd, t);

            t += deltaTime;
            yield return new WaitForEndOfFrame();
        }

        currentColor = colorEnd;
        circleFillSlider.color = colorEnd;
        circleFillGap.color = circleFillSlider.color;
        //pointerImage.color = colorEnd;
        
        circleFillSlider.fillAmount = fillEnd;
        pointerRotator.rotation = rotEnd;
    }
}
