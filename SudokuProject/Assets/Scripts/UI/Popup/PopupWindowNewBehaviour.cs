using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupWindowNewBehaviour : MonoBehaviour
{
    private PopupContentsBehaviour popupContentsBehaviour;

    [Header("Prefabs")]
    public Button ButtonPrefab;

    [Header("Assignables")]
    public TextMeshProUGUI Title;
    public RectTransform Root;
    public RectTransform ContentRoot;
    public RectTransform ButtonLayoutGroup;
    
    public UnityAction OnPopup;
    public UnityAction OnClose;
    
    private bool isPopped;



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

    public void Initialize(PopupContentsBehaviour popupContents)
    {
        GameStateManager.OnPopup();
        isPopped = true;
        OnPopup?.Invoke();
        
        popupContents.SetPopupWindow(this);
        
        Title.text = popupContents.Title;

        Root.sizeDelta = new Vector2(popupContents.Width, popupContents.Height);

        var buttonDatas  = popupContents.ButtonDatas;
        if (buttonDatas != null && buttonDatas.Any())
        {
            foreach (var buttonData in buttonDatas)
            {
                var button = Instantiate(ButtonPrefab, ButtonLayoutGroup);
                
                // todo: gör ny monobehaviour av den knappen?
                var textTransform = button.GetComponentInChildren<TextMeshProUGUI>();
                if (textTransform != null)
                    textTransform.text = buttonData.Text;
                
                button.onClick = buttonData.OnClick;
            }
        }

        var popupContent = Instantiate(popupContents.PopupContent, ContentRoot); 

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
