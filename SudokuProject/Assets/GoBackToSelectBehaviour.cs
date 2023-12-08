using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class GoBackToSelectBehaviour : MonoBehaviour
{
    public void OnGoBackButtonPressed()
    {
        Debug.Log("Go Back button pressed!");
        SaveManager.TrySave(SaveRequestLocation.ExitGameButton, false);
    }
}
