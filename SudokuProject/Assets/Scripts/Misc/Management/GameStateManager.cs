using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStateManager
{
    private static int activePopupCount;

    public static bool gameIsActive => activePopupCount <= 0;

    public static void OnPopup()
    {
        activePopupCount++;
    }
    
    public static void OnPopupClose()
    {
        activePopupCount--;
    }
}
