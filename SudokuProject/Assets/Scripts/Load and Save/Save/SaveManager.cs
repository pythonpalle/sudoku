using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Saving
{
    public static class SaveManager
    {
        private static SaveData saveData = new SaveData();

        public static UnityAction<SaveData> OnPopulateSaveData;
        public static UnityAction<SaveData> OnLoadFromData;

        public static void PopulateSaveData()
        {
            OnPopulateSaveData?.Invoke(saveData);
        }
    }
}
