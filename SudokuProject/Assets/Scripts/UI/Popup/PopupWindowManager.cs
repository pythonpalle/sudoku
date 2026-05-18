using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupWindowManager : MonoBehaviour
{
    public static PopupWindowManager instance;

    [SerializeField] private RectTransform popupParent;
    [SerializeField] private PopupWindowNewBehaviour genericPopupWindowPrefab;
    
    [Header("Confirm Popup")]
    [SerializeField] private PopupContentsBehaviour confirmationRootPrefab;
    
    // todo: readonly attribute. Stack<> instead?
    [SerializeField]  List<PopupWindowNewBehaviour> activePopupWindows;
    
    void Awake()
    {
        instance = this;
    }
    
    public void CreateConfirmPopupWindow(PopupContentsBehaviour popupData, UnityAction confirmAction)
    {
        // all confirm popups share same content
        confirmationRootPrefab.InjectConfirmContent(popupData);
        
        InitializeAndAddToActive(popupData, confirmAction);
    }
    
    public void CreatePopupWindow(PopupContentsBehaviour popupContent)
    {
        InitializeAndAddToActive(popupContent, null);
    }
    
    private void InitializeAndAddToActive(PopupContentsBehaviour popupContent, UnityAction confirmAction)
    {
        PopupWindowNewBehaviour popupWindow = Instantiate(genericPopupWindowPrefab, popupParent);
        popupWindow.Initialize(popupContent, confirmAction);
        activePopupWindows.Add(popupWindow);
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
