using UnityEngine;

namespace Saving
{
    [System.Serializable]
    public class SaveData
    {
        /// <summary>
        /// Converts the public fields of this game object to a JSON representation and returns it as a string.
        /// </summary>
        public string ToJson()
        {
            //return JsonUtility.ToJson(this);
            return JsonUtility.ToJson(this, false);
        }
    
        /// <summary>
        /// Overrides the public fields in this object from the JSON string representation.
        /// </summary>
        public void LoadFromJson(string jsonString)
        {
            JsonUtility.FromJsonOverwrite(jsonString, this);
        } 
    }
}