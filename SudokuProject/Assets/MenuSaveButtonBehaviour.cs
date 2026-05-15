using UnityEngine;

public class MenuSaveButtonBehaviour : MonoBehaviour
{
    [SerializeField] private PopupDataObject poupData;

    public void OnButtonClick()
    {
        PopupWindowManager.instance.CreatePopupWindow(poupData);
    }
}
