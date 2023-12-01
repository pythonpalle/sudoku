using UnityEngine;

public class PopupTextHandler : MonoBehaviour
{
    [Header("Popup texts")] 
    [SerializeField] private TextMeshContainer hoverExplanation;
    [SerializeField] private FloatingPopupBehaviour floatingPopup;
    
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
        hoverExplanation.gameObject.SetActive(true);
        hoverExplanation.transform.position = position;
        hoverExplanation.TextMesh.text = text;
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