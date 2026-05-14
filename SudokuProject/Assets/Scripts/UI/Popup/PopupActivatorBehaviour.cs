using UnityEngine;

public class PopupActivatorBehaviour : MonoBehaviour
{
    [SerializeField] private PopupContentsBehaviour popupContentsPrefab;

    public void ActivatePopup()
    {
        PopupWindowManager.instance.CreatePopupWindow(popupContentsPrefab);
    }
}
