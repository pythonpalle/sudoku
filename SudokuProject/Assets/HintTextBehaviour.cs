using TMPro;
using UnityEngine;

public class HintTextBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void SetHintText(HintText hintText)
    {
        titleText.text = hintText.Title;
        descriptionText.text = hintText.Description;
    }
}
