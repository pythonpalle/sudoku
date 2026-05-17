using UnityEngine;

public class HintController : MonoBehaviour
{
    [SerializeField] private PopupDataObject popupData;

    public void OpenHintPopup()
    {
        PopupWindowManager.instance.CreatePopupWindow(popupData); 
    }
}
