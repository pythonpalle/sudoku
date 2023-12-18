using UnityEngine;
using UnityEngine.Assertions;

namespace Saving
{
    public class PuzzleSaveInfo
    {
        private WFCGridSolver _gridSolver = new WFCGridSolver(PuzzleDifficulty.Extreme);

        public PuzzleDifficulty GetDifficultySuggestion(GridGenerationType type, SudokuGrid9x9 grid, PuzzleDifficulty difficulty)
        {
            // if its an empty grid, suggest a difficulty
            if (type == GridGenerationType.empty)
            {
                return GetDifficultySuggestion(grid);
            }
            // else, use the current puzzles difficulty
            else
            {
                return difficulty;
            }
        }
        
        public PuzzleDifficulty GetDifficultySuggestion(SudokuGrid9x9 grid)
        {
            if (_gridSolver.HasOneSolution(grid, true))
            {
                if (_gridSolver.HumanlySolvable(grid, out PuzzleDifficulty difficulty))
                {
                    return difficulty;
                }
                else
                {
                    return PuzzleDifficulty.Extreme;
                }
            }
            
            switch (_gridSolver.SolutionsState)
            {
                case SolutionsState.Multiple:
                    return PuzzleDifficulty.Simple;
                
                case SolutionsState.None:
                    return PuzzleDifficulty.Impossible;
            }
            
            Debug.LogError("Failed to generate a reasonable difficulty!");
            return PuzzleDifficulty.Impossible;
        }

        public string GetNameSuggestion(GridGenerationType generationType, PuzzleDifficulty difficulty)
        {
            int puzzleCount = 0;
        
            switch (generationType)
            {
                case GridGenerationType.empty:
                    puzzleCount = SaveManager.GetTotalPuzzleCount() + 1;
                    return $"Puzzle {puzzleCount}";
            
                case GridGenerationType.random:
                    puzzleCount = SaveManager.GetPuzzleCount(difficulty) + 1;
                    return  $"{difficulty} {puzzleCount}";
            }

            return "";
        }

        
    }
}