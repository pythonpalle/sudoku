using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyIcon : MonoBehaviour
{
    [SerializeField] private List<StarIcon> stars;
    [SerializeField] private ExplanationText _explanationText;
    [SerializeField] private TileColors difficultyColors;
    [SerializeField] private ColorObject backgroundColor;
    [SerializeField] private ColorObject impossibleColor;

    public void SetDifficulty(int difficulty)
    {
        string explanationText = $"Difficulty: {EnumUtility.GetEnumValue<PuzzleDifficulty>(difficulty)}";
        _explanationText.SetText(explanationText);
        
        int maxDifficulty = 4;
        if (difficulty > maxDifficulty)
        {
            foreach (var star in stars)
            {
                star.SetColor(impossibleColor.Color);
            }

            return;
        }
        
        for (int i = 0; i < stars.Count; i++)
        {
            bool activate = difficulty >= i;
            var star = stars[i];

            if (activate)
            {
                star.SetColor(difficultyColors.Colors[difficulty]);
            }
            else
            {
                star.SetColor(backgroundColor.Color);
            }
        }
    }
}
