using UnityEngine;
using UnityEngine.Events;

public class PopupWindowNewBehaviour : MonoBehaviour
{
    //[SerializeField] private RectTransform popupWindow;

    private bool isPopped;

    public UnityAction OnPopup;
    public UnityAction OnClose;

    public void PopUp()
    {
        //popupWindow.gameObject.SetActive(true);
        GameStateManager.OnPopup();
        isPopped = true;
        OnPopup?.Invoke();
    }

    public void Close()
    {
        //popupWindow.gameObject.SetActive(false);
        GameStateManager.OnPopupClose();
        isPopped = false;
        OnClose?.Invoke();
        Destroy(this.gameObject);
    }

    private void OnDisable()
    {
        if (isPopped)
        {
            Close();
        }
    }
}

/*
 
 PopupWindowData
 
 HEIGHT (enum?)
 
 TITLE
 - hasTitleText
 - titleText
 
 CONTENT
 
 BUTTON BUTTONS
 
 ButtonButton
 - Text
 - Action
 
 */
