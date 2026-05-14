using UnityEngine;

public class RestartPopupContentsBehaviour : PopupContentsBehaviour
{
    public void Cancel()
    {
        Debug.Log("Cancel");
    }

    public void Reset()
    {
        Debug.Log("Reset");
    }
}
