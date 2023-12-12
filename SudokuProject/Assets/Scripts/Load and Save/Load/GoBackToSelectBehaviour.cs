using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class GoBackToSelectBehaviour : MonoBehaviour
{
    [SerializeField] private GeneratorPort _generatorPort;
    [SerializeField] private ScenePort scenePort;
    
    public void OnGoBackButtonPressed()
    {
        if (_generatorPort.isGenerating)
        {
            scenePort.OnCallLoadPuzzleSelectScene();
        }
        else
        {
            if (SaveManager.TrySave(SaveRequestLocation.ExitGameButton, false))
            {
                scenePort.OnCallLoadPuzzleSelectScene();
            }
        }
    }
}
