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
        None
    }
    
    public static class SaveManager
    {
        private static string userSaveFileName = "data";
        
        private static string userID = "1";
        private static string userName = "user";
        private static UserIdentifier userIdentifier { get;  set; } = new UserIdentifier(userName, userID);
        private static UserSaveData currentUserData { get;  set; }
        public static PuzzleDataHolder currentPuzzle { get; private set; }
        private static bool HasCreatedPuzzleData => currentPuzzle != null;
        
        private static GridGenerationType generationType;

        public static UnityAction<SaveRequestLocation> OnRequestFirstSave;
        public static UnityAction<SaveRequestLocation> OnSuccessfulSave;
        public static UnityAction OnPuzzleSaveCreated;
        public static UnityAction<PuzzleDataHolder> OnPuzzleDeleted;
        public static UnityAction<PuzzleDataHolder> OnPuzzleReset;

        private static List<ILoadPuzzleData> loadDatas = new List<ILoadPuzzleData>();
        private static List<IPopulatePuzzleData> populateDatas = new List<IPopulatePuzzleData>();

        private static bool compress = true;
        
        public static bool AddLoadDataListener(ILoadPuzzleData data)
        {
            if (!loadDatas.Contains(data))
            {
                loadDatas.Add(data);
                return true;
            }
            else
            {
                Debug.LogWarning($"Cannot add {data} since list already contains it!");
                return false;
            }
        }
        
        public static bool AddPopulateDataListener(IPopulatePuzzleData data)
        {
            if (!populateDatas.Contains(data))
            {
                populateDatas.Add(data);
                return true;
            }
            else
            {
                Debug.LogWarning($"Cannot add {data} since list already contains it!");
                return false;
            }
        }
        
        public static bool RemoveLoadDataListener(ILoadPuzzleData data)
        {
            if (loadDatas.Contains(data))
            {
                loadDatas.Remove(data);
                return true;
            }
            else
            {
                Debug.LogWarning($"Cannot remove {data} since list doesn't contains it!");
                return false;
            }
        }
        
        public static bool RemovePopulateDataListener(IPopulatePuzzleData data)
        {
            if (populateDatas.Contains(data))
            {
                populateDatas.Remove(data);
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

            // can't save progress if there are no puzzles
            Assert.IsFalse(currentUserData.puzzles == null);
            Assert.IsFalse(currentPuzzle == null);

            // populate save datas from all puzzleDatas
            foreach (var puzzleData in populateDatas)
            {
                puzzleData.PopulateSaveData(currentPuzzle, false);
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

            if (WriteUserDataToFile(location))
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

        private static bool WriteUserDataToFile(SaveRequestLocation location)
        {
            var binary = currentUserData.ToBinary();

            if (FileManager.WriteAllBytes(userSaveFileName, binary, compress))
            {
                OnSuccessfulSave?.Invoke(location);
                return true;
            }
            else 
            {
                return false; 
            }

            
            string jsonString = currentUserData.ToJson();
            if (FileManager.WriteToFile(userSaveFileName, jsonString))
            {
                OnSuccessfulSave?.Invoke(location);
                return true;
            }
            else 
            {
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

            if (WriteUserDataToFile(SaveRequestLocation.SaveButton))
            {
                Debug.Log("Successfully created save file!");
                return true;
            }
            
            Debug.LogWarning("First file could not be created!");
            return false;
        }

        private static bool TryLoadCurrentUserData()
        {
            if (FileManager.ReadAllBytes(userSaveFileName, out byte[] bytes, compress))
            {
                currentUserData = new UserSaveData();
                currentUserData.LoadFromBinary(bytes);
                return true; 
            }
            
            // if (FileManager.LoadFromFile(userSaveFileName, out string dataAsJson))
            // {
            //     currentUserData = new UserSaveData();
            //     currentUserData.LoadFromJson(dataAsJson);
            //     return true; 
            // }

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

            bool selfCreated = generationType == GridGenerationType.empty;
            if (selfCreated)
                currentPuzzle.selfCreated = true;

            // populate save datas from all puzzleDatas
            foreach (var puzzleData in populateDatas)
            {
                puzzleData.PopulateSaveData(currentPuzzle, selfCreated);
                Debug.Log("Populate save data...");
            }
            
            // Add puzzle data to user data
            currentUserData.puzzles.Add(currentPuzzle);

            if (WriteUserDataToFile(SaveRequestLocation.SaveButton))
            {
                Debug.Log("Successfully created puzzle!");
                OnPuzzleSaveCreated?.Invoke();
                return true;
            }
            else
            {
                Debug.LogError("Failed to write to file, puzzle was not created!");
                return false;
            }
        }

        public static void SetCurrentPuzzleToNull()
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
            
            DeletePuzzle(puzzleToRemove);
            return true;
        }

        private static void DeletePuzzle(PuzzleDataHolder puzzleToRemove)
        {
            currentUserData.puzzles.Remove(puzzleToRemove);
            WriteUserDataToFile(SaveRequestLocation.None);
            OnPuzzleDeleted?.Invoke(puzzleToRemove);
        }

        public static void SetCurrentPuzzle(PuzzleDataHolder puzzle)
        {
            currentPuzzle = puzzle;
        }

        public static void LoadCurrentPuzzle()
        {
            foreach (var listener in loadDatas)
            {
                listener.LoadFromSaveData(currentPuzzle);
            }
        }

        public static void RestartPuzzle(PuzzleDataHolder puzzleDataHolder)
        {
            currentPuzzle = puzzleDataHolder;
            currentPuzzle.Reset();
            OnPuzzleReset?.Invoke(currentPuzzle);
        }
    }
}
