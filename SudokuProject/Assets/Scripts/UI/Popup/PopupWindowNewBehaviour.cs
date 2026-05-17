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

    public void Close()
    {
        GameStateManager.OnPopupClose();
        PopupWindowManager.instance.OnPopupWindowClose(this);
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
            TrySetChildText(popupContent.gameObject, popupData.ExplanationText);
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

        if (popupData.ButtonDatas.Any())
        {
            foreach (var buttonData in popupData.ButtonDatas)
            {
                var button = Instantiate(ButtonPrefab, ButtonLayoutGroup);
                
                if (!string.IsNullOrEmpty(buttonData.Text))
                    TrySetChildText(button.gameObject, buttonData.Text);
            }
        }
    }

    private static void TrySetChildText(GameObject gameObject, string text)
    {
        var textTransform = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (textTransform != null)
            textTransform.text = text;
    }
}