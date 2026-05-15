using Saving;
using UnityEngine;

public class DeleteSavePopupContents : PopupContentsBehaviour
{
    [SerializeField] private UserSavePort savePort;
    [SerializeField] ScenePort _scenePort;
    
    public void DeleteSaveFile()
    {
        int saveNumber = savePort.SelectedIndexForDelete;
        Debug.Log($"Trying to delete save {saveNumber}...");
        if (SaveManager.TryDeleteUserSave(saveNumber))
        {
            _scenePort.CallLoadPuzzleSelectScene();
            
            Debug.Log($"Successful delete!");
            Close();
        } 
    }
}
