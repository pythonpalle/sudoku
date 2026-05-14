using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupWindowNewBehaviour : MonoBehaviour
{
    //[SerializeField] private RectTransform popupWindow;

    private PopupContentsBehaviour popupContentsBehaviour;
    
    public UnityAction OnPopup;
    public UnityAction OnClose;
    
    public Button ButtonPrefab;
    
    public TextMeshProUGUI Title;
    public RectTransform ContentRoot;
    public RectTransform ButtonLayoutGroup;
    
    
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
        
        Title.text = popupContents.Title;

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
