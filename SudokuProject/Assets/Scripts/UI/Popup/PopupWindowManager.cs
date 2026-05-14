using UnityEngine;

public class PopupWindowManager : MonoBehaviour
{
    public static PopupWindowManager instance;

    [SerializeField] private RectTransform popupParent;
    [SerializeField] private PopupWindowNewBehaviour genericPopupWindowPrefab;
    
    void Awake()
    {
        instance = this;
    }

    public void CreatePopupWindow(PopupContentsBehaviour popupData)
    {
        PopupWindowNewBehaviour popupWindow = Instantiate(genericPopupWindowPrefab, popupParent);
        popupWindow.Initialize(popupData);
    }
}
