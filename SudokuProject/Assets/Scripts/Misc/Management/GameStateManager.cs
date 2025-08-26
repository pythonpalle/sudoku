using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStateManager
{
    public static int activePopupCount { get; private set; }

    public static bool gameIsActive { get; private set; } = true;

    public static void OnPopup()
    {
        // TODO: ändra till lista med aktiva popups
        
        activePopupCount++;

        UpdateActive();
    }
    
    public static void OnPopupClose()
    {
        activePopupCount--;
        
        UpdateActive();
    }

    private static void UpdateActive()
    {
        gameIsActive = activePopupCount <= 0;
    }

    public static void SetActive(bool active)
    {
        gameIsActive = active;
    }
}
