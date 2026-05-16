using UnityEngine;

public class PopupTextHandler : MonoBehaviour
{
    [Header("Popup texts")] 
    [SerializeField] private TextMeshContainer hoverExplanation;
    [SerializeField] private FloatingPopupBehaviour floatingPopup;
    [SerializeField] private RectTransform canvasTransform;
    
    private void OnEnable()
    {
        EventManager.OnDisplayHoverText += OnDisplayHoverText;
        EventManager.OnCancelHoverText += OnCancelHoverText;
        
        EventManager.OnDisplayFloatingPopupText += OnDisplayFloatingPopupText;
    }
    
    private void OnDisable()
    {
        EventManager.OnDisplayHoverText -= OnDisplayHoverText;
        EventManager.OnCancelHoverText -= OnCancelHoverText;
        
        EventManager.OnDisplayFloatingPopupText -= OnDisplayFloatingPopupText;
    }

    private void OnDisplayHoverText(string text, Vector3 position)
    {
        if (canvasTransform == null)
        {
            Debug.LogWarning("No canvas transform assigned to PopupTextHandler. Using root transform as parent instead.");
            canvasTransform = transform.root.GetComponent<RectTransform>();
        }
        
        hoverExplanation.transform.SetParent(canvasTransform);
        hoverExplanation.transform.SetAsLastSibling();
        
        hoverExplanation.transform.position = position;
        hoverExplanation.TextMesh.text = text;
        hoverExplanation.gameObject.SetActive(true);
    }

    private void OnDisplayFloatingPopupText(string text, Vector3 position)
    {
        floatingPopup.Popup(text, position);
    }
    
    private void OnCancelHoverText()
    {
        hoverExplanation.gameObject.SetActive(false);
    }
}