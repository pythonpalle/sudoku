using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Saving
{
    [System.Serializable]
    public class UserSaveData
    {
        public UserSaveData()
        {
            puzzles = new List<PuzzleDataHolder>();
        }

        public List<PuzzleDataHolder> puzzles;

        public string ToJson()
        {
            return JsonUtility.ToJson(this, false);
        }

        public void LoadFromJson(string jsonString)
        {
            JsonUtility.FromJsonOverwrite(jsonString, this);
        }

        public byte[] ToBinary()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            
            formatter.Serialize(memoryStream, this); 

            // Get the serialized data as a byte array
            byte[] serializedData = memoryStream.ToArray();
            memoryStream.Close();
            return serializedData;
        }
    
        public void LoadFromBinary(byte[] bytes)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream(bytes);
        
            // deserializing
            UserSaveData deserializedObject = (UserSaveData)formatter.Deserialize(memoryStream);
            LoadFromDeserialized(deserializedObject);
            memoryStream.Close();
        }

        private void LoadFromDeserialized(UserSaveData deserialized)
        {
            puzzles = deserialized.puzzles;
            //identifier = deserialized.identifier;
        }

        public int GetTotalPuzzleCount()
        {
            if (puzzles == null)
                return 0;

            return puzzles.Count;
        }

        public void SetDataFrom(UserSaveData currentUserData)
        {
            //this.identifier = currentUserData.identifier;
            puzzles = new List<PuzzleDataHolder>(currentUserData.puzzles);
        }
    }
}

