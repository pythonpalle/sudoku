using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    public UnityAction ConfirmAction { get; set; }


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
        
        var popupContent = Instantiate(popupContents.PopupContent, ContentRoot); 


        // var buttonDatas  = popupContents.ButtonDatas;
        // if (buttonDatas != null && buttonDatas.Any())
        // {
        //     foreach (var buttonData in buttonDatas)
        //     {
        //         var button = Instantiate(ButtonPrefab, ButtonLayoutGroup);
        //         
        //         // todo: gör ny monobehaviour av den knappen?
        //         var textTransform = button.GetComponentInChildren<TextMeshProUGUI>();
        //         if (textTransform != null)
        //             textTransform.text = buttonData.Text;
        //         
        //         button.onClick = buttonData.OnClick;
        //     }
        // }


    }
    
    public void Initialize(PopupDataObject popupData, UnityAction confirmAction)
    {
        GameStateManager.OnPopup();
        isPopped = true;
        OnPopup?.Invoke();
        
        Title.text = popupData.Title;

        Root.sizeDelta = new Vector2(popupData.Width, popupData.Height);
        
        var popupContent = Instantiate(popupData.PopupContent, ContentRoot);
        if (!string.IsNullOrEmpty(popupData.ExplanationText))
        {
            TrySetChildText(popupContent, popupData.ExplanationText);
        }

        if (confirmAction != null)
        {
            if (popupData.UseCancelButton)
            {
                var cancelButton = Instantiate(ButtonPrefab, ButtonLayoutGroup);
                TrySetChildText(cancelButton.gameObject, "Cancel");
                cancelButton.onClick.AddListener(Close);
            }
            
            var confirmButton = Instantiate(ButtonPrefab, ButtonLayoutGroup);
            TrySetChildText(confirmButton.gameObject, popupData.ConfirmButtonText);
            confirmButton.onClick.AddListener(Close);
            confirmButton.onClick.AddListener(confirmAction);
        }
    }

    private static void TrySetChildText(GameObject gameObject, string text)
    {
        var textTransform = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (textTransform != null)
            textTransform.text = text;
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
