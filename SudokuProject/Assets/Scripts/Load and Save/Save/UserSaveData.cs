using System.Collections.Generic;
using UnityEngine;

namespace Saving
{
    [System.Serializable]
    public class UserSaveData
    {
        public string ID;
        public List<PuzzleDataHolder> puzzles { get; private set; }= new List<PuzzleDataHolder>();

        /// <summary>
        /// Converts the public fields of this game object to a JSON representation and returns it as a string.
        /// </summary>
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    
        /// <summary>
        /// Overrides the public fields in this object from the JSON string representation.
        /// </summary>
        public void LoadFromJson(string jsonString)
        {
            JsonUtility.FromJsonOverwrite(jsonString, this);
        } 
    }
    
    


    public interface IHasPuzzleData
    {
        void PopulateSaveData(PuzzleDataHolder dataHolder);
        void LoadFromSaveData(PuzzleDataHolder dataHolder);
    }
}

