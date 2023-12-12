using System.Collections;
using System.Collections.Generic;
using Saving;
using UnityEngine;

public class GoBackToSelectBehaviour : MonoBehaviour
{
    [SerializeField] private GeneratorPort _generatorPort;
    [SerializeField] private ScenePort scenePort;
    [SerializeField] private SaveRequestPort _saveRequestPort;

    private SaveRequestLocation _location = SaveRequestLocation.ExitGameButton;
    
    public void OnGoBackButtonPressed()
    {
        _saveRequestPort.Location = _location;
        
        if (_generatorPort.isGenerating)
        {
            scenePort.OnCallLoadPuzzleSelectScene();
        }
        else
        {
            if (SaveManager.TrySave(_location, false))
            {
                scenePort.OnCallLoadPuzzleSelectScene();
            }
        }
    }
}
