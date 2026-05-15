using UnityEngine;
using UnityEngine.Events;

public class PopupWindowManager : MonoBehaviour
{
    public static PopupWindowManager instance;

    [SerializeField] private RectTransform popupParent;
    [SerializeField] private PopupWindowNewBehaviour genericPopupWindowPrefab;
    
    [Header("Confirm Popup")]
    [SerializeField] private GameObject confirmationRootPrefab;
   // [SerializeField] private PopupContentsBehaviour confirmPopupData;
    //[SerializeField] private PopupDataObject confirmPopupData;
    
    void Awake()
    {
        instance = this;
    }

    public void CreatePopupWindow(PopupContentsBehaviour popupData)
    {
        CreateAndInitializePopupWindow(popupData);
    }

    private PopupWindowNewBehaviour CreateAndInitializePopupWindow(PopupContentsBehaviour popupData)
    {
        PopupWindowNewBehaviour popupWindow = Instantiate(genericPopupWindowPrefab, popupParent);
        popupWindow.Initialize(popupData);
        return popupWindow;
    }

    public void CreateConfirmPopupWindow(PopupDataObject popupData, UnityAction confirmAction)
    {
        // all confirm popups share same content
        popupData.PopupContent = confirmationRootPrefab;
        
        var popupWindow = Instantiate(genericPopupWindowPrefab, popupParent);
        popupWindow.Initialize(popupData, confirmAction);
    }
}
