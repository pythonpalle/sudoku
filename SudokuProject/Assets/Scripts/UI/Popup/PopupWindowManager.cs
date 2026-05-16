using System.Collections.Generic;
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
    
    // todo: readonly attribute. Stack<> instead?
    [SerializeField]  List<PopupWindowNewBehaviour> activePopupWindows;
    
    void Awake()
    {
        instance = this;
    }

    public void CreatePopupWindow(PopupContentsBehaviour popupData)
    {
        PopupWindowNewBehaviour popupWindow = Instantiate(genericPopupWindowPrefab, popupParent);
        InitializeAndAddToActive(popupData, popupWindow);
    }

    private void InitializeAndAddToActive(PopupContentsBehaviour popupData, PopupWindowNewBehaviour popupWindow)
    {
        popupWindow.Initialize(popupData);
        activePopupWindows.Add(popupWindow);
    }


    public void CreateConfirmPopupWindow(PopupDataObject popupData, UnityAction confirmAction)
    {
        // all confirm popups share same content
        popupData.PopupContent = confirmationRootPrefab;
        
        var popupWindow = Instantiate(genericPopupWindowPrefab, popupParent);
        InitializeAndAddToActive(popupData, confirmAction, popupWindow);
    }

    private void InitializeAndAddToActive(PopupDataObject popupData, UnityAction confirmAction,
        PopupWindowNewBehaviour popupWindow)
    {
        popupWindow.Initialize(popupData, confirmAction);
        activePopupWindows.Add(popupWindow);
    }

    public void CreatePopupWindow(PopupDataObject popupData)
    {
        PopupWindowNewBehaviour popupWindow = Instantiate(genericPopupWindowPrefab, popupParent);
        // popupWindow.Initialize(popupData, null); 
        // activePopupWindows.Add(popupWindow);
        
        InitializeAndAddToActive(popupData, null, popupWindow);

    }

    public void ClosePopup(GameObject o)
    {
        PopupWindowNewBehaviour popupWindow = GetPopupInGameObject(o);
        if (popupWindow != null && activePopupWindows.Contains(popupWindow))
        {
            popupWindow.Close();
        }
    }

    private static PopupWindowNewBehaviour GetPopupInGameObject(GameObject o)
    {
        var popupWindow = o.GetComponentInParent<PopupWindowNewBehaviour>();
        if (popupWindow == null)
        {
            popupWindow = o.GetComponent<PopupWindowNewBehaviour>();
        }
        
        return popupWindow;
    }

    public void OnPopupWindowClose(PopupWindowNewBehaviour popupWindow)
    {
        if (popupWindow != null && activePopupWindows.Contains(popupWindow))
        {
            activePopupWindows.Remove(popupWindow);
        }    
    }
}
