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

        private static List<IHasPuzzleData> puzzleDatas = new List<IHasPuzzleData>();
        
        public static bool AddPuzzleDataListener(IHasPuzzleData data)
        {
            if (!puzzleDatas.Contains(data))
            {
                puzzleDatas.Add(data);
                return true;
            }
            else
            {
                Debug.LogWarning($"Cannot add {data} since list already contains it!");
                return false;
            }
        }
        
        public static bool RemovePuzzleDataListener(IHasPuzzleData data)
        {
            if (puzzleDatas.Contains(data))
            {
                puzzleDatas.Remove(data);
                return true;
            }
            else
            {
                Debug.LogWarning($"Cannot remove {data} since list doesn't contains it!");
                return false;
            }
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
            // check to see if user data exists or can be created
            if (!TryGetCurrentUserData(out _))
            {
                Debug.LogError("User data not found, puzzle can't be created!");
                return false;
            }

            // can't save progrees if there are no puzzles
            Assert.IsFalse(currentUserData.puzzles == null);
            Assert.IsFalse(currentPuzzle == null);

            // populate save datas from all puzzleDatas
            foreach (var puzzleData in puzzleDatas)
            {
                puzzleData.PopulateSaveData(currentPuzzle, generationType);
                Debug.Log("Populate save data...");
            }

            // find puzzle with id that matches with current puzzle id, overwrite it
            for (int i = 0; i < currentUserData.puzzles.Count; i++)
            {
                if (currentUserData.puzzles[i].id == currentPuzzle.id)
                {
                    currentUserData.puzzles[i] = currentPuzzle;
                    break;
                }
            }

            // onvert data to json format
            string jsonString = currentUserData.ToJson();
            
            // Write data to file
            if (FileManager.WriteToFile(userSaveFileName, jsonString))
            {
                Debug.Log($"Successfully saved progress for {currentPuzzle.name}!");
                return true;
            }
            else
            {
                Debug.LogError("Failed to write to file, progress was not saved!");
                return false;
            }
            
        }

        private static bool TryCreateFirstSaveForCurrentPuzzle(SaveRequestLocation location, bool forceSave = false)
        {
            // if saving a puzzle that is loaded, it should have already been created
            Assert.IsTrue(generationType != GridGenerationType.loaded);

            // if forceSave, save the data without asking the user
            if (forceSave)
            {
                Debug.Log("Trying to force save the game...");
            }
            // else, ask user if they want to save
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
            TryGetCurrentUserData(out _);
            
            if (currentUserData == null || currentUserData.puzzles == null)
            {
                return 0;
            }

            return currentUserData.puzzles.Count;
        }

        public static int GetPuzzleCount(PuzzleDifficulty difficulty)
        {
            TryGetCurrentUserData(out _);

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
            
            // check to see if user data exists or can be created
            if (!TryGetCurrentUserData(out _))
            {
                Debug.LogError("User data not found, puzzle can't be created!");
                return false;
            }

            // check to see if user has puzzle list, else create one
            if (currentUserData.puzzles == null)
            {
                currentUserData.puzzles = new List<PuzzleDataHolder>();
                Debug.LogWarning("No puzzle list found for user, creating new one.");
            }

            // check if user already has puzzle with id _id_
            foreach (var puzzle in currentUserData.puzzles)
            {
                if (puzzle.id == id)
                {
                    Debug.LogError($"Puzzle with id {id} already exists!");
                    return false;
                }
            }

            // NOTE: might cause reference problems
            currentPuzzle = new PuzzleDataHolder();

            // assign identifier values
            currentPuzzle.name = puzzleSaveName;
            currentPuzzle.id = id;
            currentPuzzle.difficulty = (int)difficulty;

            if (generationType == GridGenerationType.empty)
                currentPuzzle.selfCreated = true;

            // populate save datas from all puzzleDatas
            foreach (var puzzleData in puzzleDatas)
            {
                puzzleData.PopulateSaveData(currentPuzzle, generationType);
                Debug.Log("Populate save data...");
            }
            
            // Add puzzle data to user data
            currentUserData.puzzles.Add(currentPuzzle);

            // onvert data to json format
            string jsonString = currentUserData.ToJson();
            
            // Write data to file
            if (FileManager.WriteToFile(userSaveFileName, jsonString))
            {
                Debug.Log("Successfully created puzzle!");
                return true;
            }
            else
            {
                Debug.LogError("Failed to write to file, puzzle was not created!");
                return false;
            }
        }

        public static void ResetCurrentPuzzle()
        {
            currentPuzzle = null;
        }

        public static bool TryDeletePuzzle(PuzzleDataHolder puzzleToRemove)
        {
            if (!currentUserData.puzzles.Contains(puzzleToRemove))
            {
                Debug.LogError($"Couldn't remove {puzzleToRemove.name} because it's not in the puzzle list!");
                return false;
            }
            
            currentUserData.puzzles.Remove(puzzleToRemove);
            string jsonString = currentUserData.ToJson();
            FileManager.WriteToFile(userSaveFileName, jsonString);
            return true;
        }
    }

    
}
