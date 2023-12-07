using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingPopupBehaviour : MonoBehaviour
{
    [SerializeField] GameObject popupObject; 
    [SerializeField] TextMeshProUGUI TextMesh;
    [SerializeField] Animator animator;
    [SerializeField] float displayTime;

    public void Popup(string text, Vector3 position)
    {
        StartCoroutine(PopupRoutine(text, position));
    }

    private IEnumerator PopupRoutine(string text, Vector3 position)
    {
        TextMesh.text = text;
        transform.position = position; 
        
        popupObject.SetActive(true);
        animator.SetTrigger("Play");

        yield return new WaitForSeconds(displayTime);
        
        animator.ResetTrigger("Play");
        popupObject.SetActive(false);
    } 
}