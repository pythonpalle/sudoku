using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindowActivator2 : MonoBehaviour
{
    [SerializeField] private PopupDataObject poupData;

    private Button button;
    
    private void Awake()
    {
        button = gameObject.GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (button && button.onClick.GetPersistentEventCount() == 0) 
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }
    
    private void OnDisable()
    {
        if (button)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }

    public void OnButtonClick()
    {
        PopupWindowManager.instance.CreatePopupWindow(poupData);
    }
}
