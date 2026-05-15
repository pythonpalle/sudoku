using Saving;
using UnityEngine;

public class DeleteSavePopupContents : PopupContentsBehaviour
{
    public int saveNumber = 2;
    
    public void DeleteSaveFile()
    {
        Debug.Log($"Trying to delete save {saveNumber}...");
        if (SaveManager.TryDeleteUserSave(saveNumber))
        {
            Debug.Log($"Successful delete!");
            transform.parent.gameObject.SetActive(false); 
            transform.parent.gameObject.SetActive(true);
        } 
    }
}
