using UnityEngine;

public class PopupWindowManager : MonoBehaviour
{
    public static PopupWindowManager instance;

    [SerializeField] private RectTransform popupParent;
    [SerializeField] private GameObject genericPopupWindowPrefab;
    
    void Awake()
    {
        instance = this;
    }

    public void ShowPopupWindow()
    {
        GameObject popupWindow = Instantiate(genericPopupWindowPrefab, popupParent);
    }
}
