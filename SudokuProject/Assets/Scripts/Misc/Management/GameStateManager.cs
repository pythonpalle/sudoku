using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStateManager
{
    private static int activePopupCount;

    public static bool gameIsActive { get; private set; } = true;

    public static void OnPopup()
    {
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
