using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Saving
{
    public static class SaveManager
    {
        private static string userSaveFileName = "data";
        
        private static string userID = "1";
        private static string userName = "user";
        private static UserIdentifier userIdentifier { get;  set; } = new UserIdentifier(userName, userID);
        
        private static UserSaveData currentUserData { get;  set; } 
        
        
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
    }

    
}