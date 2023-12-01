using UnityEngine;

public class PopupTextHandler : MonoBehaviour
{
    [Header("Popup texts")] 
    [SerializeField] private TextMeshContainer hoverExplanation;
    [SerializeField] private TextMeshContainer floatingPopup;
    
    private void OnEnable()
    {
        EventManager.OnDisplayHoverText += OnDisplayHoverText;
        EventManager.OnCancelHoverText += OnCancelHoverText;
    }
    
    private void OnDisable()
    {
        EventManager.OnDisplayHoverText -= OnDisplayHoverText;
        EventManager.OnCancelHoverText -= OnCancelHoverText;
    }

    private void OnDisplayHoverText(string text, Vector2 position)
    {
        hoverExplanation.gameObject.SetActive(true);
        hoverExplanation.transform.position = position;
        hoverExplanation.TextMesh.text = text;
    }
    
    private void OnCancelHoverText()
    {
        hoverExplanation.gameObject.SetActive(false);
    }
}
