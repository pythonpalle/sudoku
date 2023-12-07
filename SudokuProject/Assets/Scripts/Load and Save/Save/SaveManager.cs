using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Saving
{
    public static class SaveManager
    {
        public static IDContainer CurrentID { get; private set; }
        
        private static UserSaveData currentUserData = new UserSaveData();
        private static PuzzleDataHolder currentPuzzle;

        public static bool TryGetCurrentPuzzle(out PuzzleDataHolder puzzle)
        {
            puzzle = null;

            if (currentPuzzle != null)
            {
                puzzle = currentPuzzle;
                return true;
            }

            return false;
        }
    }

    public struct IDContainer
    {
        public string userID;
        public string puzzleID;
    }
}
