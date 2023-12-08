using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;



namespace Saving
{
    public enum SaveRequestLocation
    {
        SaveButton,
        ExitGameButton,
    }
    
    public static class SaveManager
    {
        private static string userSaveFileName = "data";
        
        private static string userID = "1";
        private static string userName = "user";
        private static UserIdentifier userIdentifier { get;  set; } = new UserIdentifier(userName, userID);
        private static UserSaveData currentUserData { get;  set; }
        private static PuzzleDataHolder currentPuzzle;
        private static bool HasCreatedPuzzleData => currentPuzzle != null;
        
        private static GridGenerationType generationType;

        public static UnityAction<SaveRequestLocation> OnRequestFirstSave;

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

        public static bool TrySave(SaveRequestLocation location, bool forceSave = false)
        {
            generationType = GenereratorTypeHolder.instance.GetType();
            
            Debug.Log($"Generation type: {generationType}");
            
            if (HasCreatedPuzzleData)
            {
                return TrySaveProgressForCurrentPuzzle(location, forceSave);
            }
            else
            {
                return TryCreateFirstSaveForCurrentPuzzle(location, forceSave);
            }
            
            
        }

        private static bool TrySaveProgressForCurrentPuzzle(SaveRequestLocation location, bool forceSave)
        {
            throw new NotImplementedException();
        }

        private static bool TryCreateFirstSaveForCurrentPuzzle(SaveRequestLocation location, bool forceSave = false)
        {
            // if saving a puzzle that is loaded, it should have already been created
            Assert.IsTrue(generationType != GridGenerationType.loaded);

            if (forceSave)
            {
                // Create suitable name, id
                // Populate all save data
                // Write to file
                // return writeSuccessul
                Debug.Log("Trying to force save the game...");
            }
            else
            {
                OnRequestFirstSave?.Invoke(location);
                return false;
            }
            
            
            
            return true;
        }

        public static bool TryGetCurrentUserData(out UserSaveData saveData)
        {
            saveData = null;

            // user data is already stored, return it
            if (currentUserData != null)
            {
                saveData = currentUserData;
                return true;
            }

            // if the file exists, try to load the data from the file
            if (FileManager.FileExists(userSaveFileName))
            {
                if (TryLoadCurrentUserData() && currentUserData != null)
                {
                    saveData = currentUserData;
                    return true;
                }
            }

            // if file doesn't exist, create a new file
            if (TryCreateUserSaveFile() && currentUserData != null)
            {
                saveData = currentUserData;
                return true;
            }
            
            Debug.LogError("User data could not be loaded or created!");
            return false;
        }

        private static bool TryCreateUserSaveFile()
        {
            currentUserData = new UserSaveData(userIdentifier);
            string dataAsJson = currentUserData.ToJson();
            
            Debug.Log("identifier: (SM)" + userIdentifier.id);
            Debug.Log("identifier: (userData)" + currentUserData.identifier.id);
            Debug.Log("json string: " + dataAsJson);

            if (FileManager.WriteToFile(userSaveFileName, dataAsJson))
            {
                Debug.Log("Successfully created save file!");
                return true;
            }

            return false;
        }

        private static bool TryLoadCurrentUserData()
        {
            if (FileManager.LoadFromFile(userSaveFileName, out string dataAsJson))
            {
                currentUserData = new UserSaveData();
                currentUserData.LoadFromJson(dataAsJson);
                return true; 
            }

            return false;
        }

        public static void SetGenerationType(GridGenerationType generationType)
        {
            SaveManager.generationType = generationType;
        }

        public static int GetTotalPuzzleCount()
        {
            if (currentUserData == null || currentUserData.puzzles == null)
            {
                return 0;
            }

            return currentUserData.puzzles.Count;
        }

        public static int GetPuzzleCount(PuzzleDifficulty difficulty)
        {
            if (currentUserData == null || currentUserData.puzzles == null)
            {
                return 0;
            }

            int count = 0;
            foreach (var puzzle in currentUserData.puzzles)
            {
                if (puzzle.difficulty == (int)difficulty)
                    count++;
            }

            return count;
        }

        public static bool TryCreateNewPuzzleSave(string puzzleSaveName, PuzzleDifficulty difficulty, GridGenerationType generationType)
        {
            Debug.Log($"Save and create puzzle with name {puzzleSaveName} and difficulty {difficulty} and type {generationType}!");
            string id = Guid.NewGuid().ToString(); 

            switch (generationType) 
            {
                case GridGenerationType.empty:
                    return TryCreateNewSelfMadePuzzleSave(puzzleSaveName, difficulty);
                
                case GridGenerationType.random:
                    return TryCreateNewRandomlyGenPuzzleSave(puzzleSaveName, difficulty);
            }

            return false;
        }

        private static bool TryCreateNewSelfMadePuzzleSave(string puzzleSaveName, PuzzleDifficulty difficulty)
        {
            throw new NotImplementedException();
        }
        
        private static bool TryCreateNewRandomlyGenPuzzleSave(string puzzleSaveName, PuzzleDifficulty difficulty)
        {
            throw new NotImplementedException();
        }
    }

    
}
