using UnityEngine;

namespace Saving
{
    public class PuzzleSaveInfo : MonoBehaviour
    {
        [SerializeField] private GeneratorPort _generatorPort;
        [SerializeField] private DifficultyObject difficultyObject;
        public PuzzleDifficulty GetDifficulty()
        {
            return PuzzleDifficulty.Easy;
        }

        public string GetNameSuggestion()
        {
            int puzzleCount = 0;
        
            switch (_generatorPort.GenerationType)
            {
                case GridGenerationType.empty:
                    puzzleCount = SaveManager.GetTotalPuzzleCount() + 1;
                    return $"Puzzle {puzzleCount}";
            
                case GridGenerationType.random:
                    puzzleCount = SaveManager.GetPuzzleCount(difficultyObject.Difficulty) + 1;
                    return  $"{difficultyObject.Difficulty} {puzzleCount}";
            }

            return "";
        }
    }
}